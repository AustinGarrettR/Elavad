using System;
using Unity.Collections;
using Unity.Networking.Transport;
using System.Collections.Generic;
using UnityEngine.Assertions;
using Engine.Networking.Utility;
using Engine.Configuration;
using Engine.Logging;
using Engine.Factory;

namespace Engine.Networking
{
    /// <summary>
    /// Manages connections for server and client
    /// </summary>
    public class ConnectionManager : Manager
    {

        /*
         * Constructor
         */

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="listenerType">Whether the connection manager is the server or not</param>
        public ConnectionManager(ListenerType listenerType)
        {
            this._isServer = listenerType == ListenerType.SERVER;
        }

        /*
         * Override Functions
         */

        /// <summary>
        /// Called on start
        /// </summary>
        public override void Init()
        {

            if (isServer)
                Start();
        }

        /// <summary>
        /// Called every frame on main thread
        /// </summary>
        public override void Process()
        {

            if (isServer)
                serverUpdateLoop();
            else if (networkDriver.IsCreated)
                clientUpdateLoop();
        }

        /// <summary>
        /// Called on program shutdown
        /// </summary>
        public override void Shutdown()
        {
            close();
        }

        /*
         * Internal Variables
         */

        /// <summary>
        /// Instance running as server or client
        /// </summary>
        private bool _isServer;

        /// <summary>
        /// Readonly if instance is running server or client
        /// </summary>
        public virtual bool isServer { get => _isServer; }

        /// <summary>
        /// Networking driver
        /// </summary>
        internal NetworkDriver networkDriver;


        /// <summary>
        /// Sends UDP data unreliably
        /// </summary>
        private NetworkPipeline unreliablePipeline;

        /// <summary>
        /// Sends UDP data reliably (similar to TCP)
        /// </summary>
        private NetworkPipeline reliablePipeline;

        /// <summary>
        /// List of connections
        /// </summary>
        private List<NetworkConnection> connections;

        /// <summary>
        /// List of connections which last read an escaped byte
        /// </summary>
        private List<int> escapedConnections;

        /// <summary>
        /// Event delegate called when client is disconnected
        /// </summary>
        /// <param name="connection">The client that disconnected</param>
        public delegate void NotifyClientDisconnectedDelegate(NetworkConnection connection);

        /// <summary>
        /// Event delegate when a client connects
        /// </summary>
        /// <param name="connection">The connected client</param>
        public delegate void NotifyClientConnectedDelegate(NetworkConnection connection);

        /// <summary>
        /// Event delegate for a packet receive
        /// </summary>
        /// <param name="connection">The source of the packet</param>
        /// <param name="packetId">The packet id</param>
        /// <param name="rawPacket">The packet data in byte array format</param>
        public delegate void NotifyPacketReceivedDelegate(NetworkConnection connection, int packetId, byte[] rawPacket);

        /// <summary>
        /// Event delegate for failing to connect
        /// </summary>
        public delegate void NotifyFailedConnectDelegate();

        /// <summary>
        /// Event delegate for disconnecting from the server
        /// </summary>
        public delegate void NotifyOnDisconnectedFromServerDelegate();

        /// <summary>
        /// Event delegate for the client connecting to the server
        /// </summary>
        public delegate void NotifyOnConnectedToServerDelegate();

        /// <summary>
        /// Event for client disconnection
        /// </summary>
        public event NotifyClientDisconnectedDelegate NotifyClientDisconnected;

        /// <summary>
        /// Event for a client connection
        /// </summary>
        public event NotifyClientConnectedDelegate NotifyClientConnected;

        /// <summary>
        /// Event for a packet being received
        /// </summary>
        public event NotifyPacketReceivedDelegate NotifyPacketReceived;

        /// <summary>
        /// Event for failing to connect to server
        /// </summary>
        public event NotifyFailedConnectDelegate NotifyFailedConnect;

        /// <summary>
        /// Event for being disconnected from server
        /// </summary>
        public event NotifyOnDisconnectedFromServerDelegate NotifyOnDisconnectedFromServer;

        /// <summary>
        /// Event for connecting to the server
        /// </summary>
        public event NotifyOnConnectedToServerDelegate NotifyOnConnectedToServer;

        /// <summary>
        /// Is the client connected
        /// </summary>
        private bool connected;

        /// <summary>
        /// Server or Client
        /// </summary>
        public enum ListenerType
        {
            /// <summary>
            /// If the connection manager is the server instance
            /// </summary>
            SERVER,

            /// <summary>
            /// If the connection manager is a client instance
            /// </summary>
            CLIENT
        }

        /*
         * Internal Functions
         */

        /// <summary>
        /// Start method
        /// </summary>
        public void Start()
        {

            connections = new List<NetworkConnection>();
            escapedConnections = new List<int>();

            networkDriver = NetworkDriver.Create();

            unreliablePipeline = networkDriver.CreatePipeline(typeof(UnreliableSequencedPipelineStage));
            reliablePipeline = networkDriver.CreatePipeline(typeof(ReliableSequencedPipelineStage));

            var endpoint = isServer ? NetworkEndPoint.AnyIpv4 : NetworkEndPoint.LoopbackIpv4;
            endpoint.Port = SharedConfig.PORT;

            //Bind to port if server
            if (isServer)
            {
                if (networkDriver.Bind(endpoint) != 0)
                    Log.LogError("Server failed to bind to port " + SharedConfig.PORT);
                else
                {
                    networkDriver.Listen();
                    Log.LogMsg("Server listening on port " + SharedConfig.PORT);
                }
            }
            else
            {
                //Connect if client
                NetworkConnection connection = networkDriver.Connect(endpoint);
                connections.Add(connection);
            }

        }

        /// <summary>
        /// Server update loop
        /// </summary>
        private void serverUpdateLoop()
        {
            networkDriver.ScheduleUpdate().Complete();

            // Clean Up Connections
            for (int i = 0; i < connections.Count; i++)
            {
                if (!connections[i].IsCreated)
                {
                    connections.RemoveAtSwapBack(i);
                    --i;
                }
            }
            // AcceptNewConnections
            NetworkConnection c;
            while ((c = networkDriver.Accept()) != default)
            {
                connections.Add(c);
                OnClientConnected(c);
            }

            //Update Block

            DataStreamReader stream;

            //Queued Packet buffer
            List<byte> queuedMsg = new List<byte>();

            for (int index = 0; index < connections.Count; index++)
            {
                if (!connections[index].IsCreated)
                    Assert.IsTrue(true);

                NetworkEvent.Type cmd;
                while ((cmd = networkDriver.PopEventForConnection(connections[index], out stream)) !=
                       NetworkEvent.Type.Empty)
                {

                    if (cmd == NetworkEvent.Type.Data)
                    {
                        int streamMaxLength = stream.Length;
                        while (stream.GetBytesRead() < streamMaxLength)
                        {
                            byte nextByte = stream.ReadByte();
                            if (nextByte == SharedConfig.ESCAPE)
                            {
                                if (escapedConnections.Contains(connections[index].InternalId) == false)
                                    escapedConnections.Add(connections[index].InternalId);
                            }
                            else
                            {
                                queuedMsg.Add(nextByte);
                            }

                            if (escapedConnections.Contains(connections[index].InternalId))
                            {
                                if (stream.GetBytesRead() < streamMaxLength)
                                {
                                    escapedConnections[index] = default;

                                    nextByte = stream.ReadByte();
                                    if (nextByte == SharedConfig.DELIMITER)
                                    {
                                        if (queuedMsg.Count > 0)
                                        {
                                            byte[] bytes = queuedMsg.ToArray();
                                            queuedMsg.Clear();
                                            OnPacketReceived(connections[index], bytes);
                                        }
                                    }
                                    else
                                    {
                                        queuedMsg.Add(nextByte);
                                    }
                                }
                            }
                        }
                    }
                    else if (cmd == NetworkEvent.Type.Disconnect)
                    {
                        connections[index] = default;                       
                        OnClientDisconnected(connections[index]);
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// Client update loop
        /// </summary>
        private void clientUpdateLoop()
        {
            networkDriver.ScheduleUpdate().Complete();

            if (!connections[0].IsCreated)
            {
                OnFailedConnect();
                return;
            }

            DataStreamReader stream;

            //Queued Packet buffer
            List<byte> queuedMsg = new List<byte>();

            NetworkEvent.Type cmd;

            while (networkDriver.IsCreated && (cmd = connections[0].PopEvent(networkDriver, out stream)) !=
                   NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Connect)
                {
                    OnConnectedToServer();
                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    int streamMaxLength = stream.Length;
                    while (stream.GetBytesRead() < streamMaxLength)
                    {
                        byte nextByte = stream.ReadByte();
                        if (nextByte == SharedConfig.ESCAPE)
                        {
                            if (escapedConnections.Contains(connections[0].InternalId) == false)
                                escapedConnections.Add(connections[0].InternalId);
                        }
                        else
                        {
                            queuedMsg.Add(nextByte);
                        }

                        if (escapedConnections.Contains(connections[0].InternalId))
                        {
                            if (stream.GetBytesRead() < streamMaxLength)
                            {
                                escapedConnections.RemoveAtSwapBack(connections[0].InternalId);

                                nextByte = stream.ReadByte();
                                if (nextByte == SharedConfig.DELIMITER)
                                {
                                    if (queuedMsg.Count > 0)
                                    {
                                        byte[] bytes = queuedMsg.ToArray();
                                        queuedMsg.Clear();
                                        OnPacketReceived(connections[0], bytes);
                                    }
                                }
                                else
                                {
                                    queuedMsg.Add(nextByte);
                                }
                            }
                        }

                    }
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Log.LogMsg("Client got disconnected from server");
                    close();
                }

            }

        }

        /// <summary>
        /// Close and reset
        /// </summary>
        private void close()
        {

            Log.LogMsg("Closed connection.");

            if (connections != null)
                connections = null;

            if (escapedConnections != null)
                escapedConnections = null;

            if (networkDriver.IsCreated)
                networkDriver.Dispose();

            //Clear variables
            connected = false;

        }

        /// <summary>
        /// Method for sending packet to server. 
        /// </summary>
        /// <param name="packet">The packet</param>
        public void sendPacketToServer(Packet packet)
        {
            if (connections.Count != 1)
            {
                Log.LogError("Attempted sending packet to server when there is either 0 or more than 1 connection active, indicating this is the server.");
                return;
            }

            sendPacket(connections[0], packet);
        }

        /// <summary>
        /// Method for sending packet to client
        /// </summary>
        /// <param name="client">Client connection to receive the packet</param>
        /// <param name="packet">The packet</param>
        public void sendPacketToClient(NetworkConnection client, Packet packet)
        {
            sendPacket(client, packet);
        }

        /// <summary>
        /// Private method for sending packets to a connection
        /// </summary>
        /// <param name="c">The connection</param>
        /// <param name="packet">The Packet</param>
        private void sendPacket(NetworkConnection c, Packet packet)
        {
            //Ensure connection is valid
            if (c.IsCreated == false) return;

            byte[] packetBytes;

            try
            {
                packetBytes = packet.getBytes();
            }
            catch (Exception e)
            {
                throw e;
            }

            //Choose pipeline
            NetworkPipeline pipeline = packet.packetReliabilityScheme == ReliabilityScheme.RELIABLE ? this.reliablePipeline : this.unreliablePipeline;

            //Create writer for sending the data
            DataStreamWriter writer = networkDriver.BeginSend(pipeline, c);

            //Write packetID
            writeEscapedBytes(ref writer, ByteConverter.getBytes<int>(packet.packetId));

            //Write packet data
            writeEscapedBytes(ref writer, packetBytes);

            //Write delimiter to indiciate the end of packet
            writer.WriteByte(SharedConfig.ESCAPE);
            writer.WriteByte(SharedConfig.DELIMITER);

            //Send writer
            networkDriver.EndSend(writer);

        }

        /// <summary>
        /// Escape our bytes and send over network
        /// </summary>
        /// <param name="writer">Data stream writer</param>
        /// <param name="bytes">Bytes to write</param>
        private void writeEscapedBytes(ref DataStreamWriter writer, byte[] bytes)
        {
            foreach (byte b in bytes)
            {
                if (b == SharedConfig.ESCAPE)
                {
                    writer.WriteByte(SharedConfig.ESCAPE);
                    writer.WriteByte(SharedConfig.ESCAPE);
                }
                else
                    writer.WriteByte(b);
            }
        }


        /// <summary>
        /// Client disconnect function
        /// </summary>
        public void Disconnect()
        {
            if (isServer)
            {
                Log.LogError("Server is attempting to disconnect as a client.");
                return;
            }

            if (connected == true)
            {
                connected = false;
                OnDisconnectedFromServer();
            }

            close();
        }

        /// <summary>
        /// Triggers event for packet receiving
        /// </summary>
        /// <param name="c">The connection</param>
        /// <param name="bytes">The packet data</param>
        private void OnPacketReceived(NetworkConnection c, byte[] bytes)
        {

            int packetId = PacketReader.ReadInt(ref bytes);
            NotifyPacketReceived?.Invoke(c, packetId, bytes);

        }

        /// <summary>
        /// Triggers event for a client connecting
        /// </summary>
        /// <param name="client">The client connection</param>
        private void OnClientConnected(NetworkConnection client)
        {
            NotifyClientConnected?.Invoke(client);

            Log.LogMsg("New connection! Server: " + isServer + ".");
        }

        /// <summary>
        /// Triggers event for a client disconnecting
        /// </summary>
        /// <param name="client">The client connection</param>
        private void OnClientDisconnected(NetworkConnection client)
        {
            NotifyClientDisconnected?.Invoke(client);
        }

        /// <summary>
        /// Triggers event for a client failing to connect
        /// </summary>
        internal void OnFailedConnect()
        {
            NotifyFailedConnect?.Invoke();
        }

        /// <summary>
        /// Triggers event when disconnected from server
        /// </summary>
        private void OnDisconnectedFromServer()
        {
            NotifyOnDisconnectedFromServer?.Invoke();
        }

        /// <summary>
        /// Triggers event when disconnected from server
        /// </summary>
        private void OnConnectedToServer()
        {
            NotifyOnConnectedToServer?.Invoke();
        }
    }
}
