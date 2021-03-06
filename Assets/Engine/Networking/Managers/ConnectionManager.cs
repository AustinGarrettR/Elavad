﻿using System;
using Unity.Collections;
using Unity.Networking.Transport;
using System.Collections.Generic;
using UnityEngine.Assertions;
using Engine.Networking.Utility;
using Engine.Configuration;
using Engine.Logging;
using Engine.Factory;
using Engine.Utility;
using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

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
            {
                ServerUpdateLoop();
            } else {
                
                if (clientConnecting || connected)
                {
                    ClientUpdateLoop();
                }

                if (connected)
                {
                    KeepAlive();
                }
            }
        }

        /// <summary>
        /// Called on program shutdown
        /// </summary>
        public override void Shutdown()
        {
            Close();
        }



        /*
         * Internal Variables
         */

        /// <summary>
        /// A hastable of packet instances
        /// </summary>
        private Dictionary<Type, Packet> packetInstances = new Dictionary<Type, Packet>();

        /// <summary>
        /// If the client is trying to connect
        /// </summary>
        private bool clientConnecting;

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
        /// Timestamp in milliseconds of the last keep alive packet sent
        /// </summary>
        private long lastKeepAlive;

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

            //Config
            NetworkConfigParameter config = new NetworkConfigParameter
            {
                connectTimeoutMS = 5000,
                disconnectTimeoutMS = 5000
            };
            networkDriver = NetworkDriver.Create(config);

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

                clientConnecting = true;
            }

        }

        /// <summary>
        /// Sends a keep alive packet to the server to prevent timeout
        /// </summary>
        private void KeepAlive()
        {
            //Make sure it's been enough time
            if (TimeHandler.getTimeInMilliseconds() - lastKeepAlive < SharedConfig.KEEP_ALIVE_PACKET_INTERVAL_IN_MILLISECONDS)
                return;

            lastKeepAlive = TimeHandler.getTimeInMilliseconds();

            //Create the packet
            KeepAlive_0 packet = new KeepAlive_0();

            //Packet is empty, do nothing to it

            //Send packet
            this.SendPacketToServer(packet);
        }

        /// <summary>
        /// Server update loop
        /// </summary>
        private void ServerUpdateLoop()
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
            for (int index = 0; index < connections.Count; index++)
            {

                if (!connections[index].IsCreated)
                {
                    continue;
                }

                //Queued Packet buffer
                List<byte> queuedMsg = new List<byte>();

                NetworkEvent.Type cmd;
                while (connections != null && (cmd = networkDriver.PopEventForConnection(connections[index], out DataStreamReader stream)) !=
                       NetworkEvent.Type.Empty)
                {

                    if (cmd == NetworkEvent.Type.Data)
                    {
                        int streamMaxLength = stream.Length;
                        while (networkDriver.IsCreated && connections != null && stream.GetBytesRead() < streamMaxLength)
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
                                    escapedConnections.Remove(connections[index].InternalId);

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
                        OnClientDisconnected(connections[index]);
                        connections.RemoveAt(index);
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// Client update loop
        /// </summary>
        private void ClientUpdateLoop()
        {
            networkDriver.ScheduleUpdate().Complete();

            if (!connections[0].IsCreated)
            {
                OnFailedConnect();
                return;
            }
            //Queued Packet buffer
            List<byte> queuedMsg = new List<byte>();

            NetworkEvent.Type cmd;

            while (networkDriver.IsCreated && connections != null && connections[0] != null && (cmd = connections[0].PopEvent(networkDriver, out DataStreamReader stream)) !=
                   NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Connect)
                {
                    OnConnectedToServer();
                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    int streamMaxLength = stream.Length;
                    while (networkDriver.IsCreated && stream.GetBytesRead() < streamMaxLength)
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
                                escapedConnections.Remove(connections[0].InternalId);

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
                    OnDisconnectedFromServer();
                    Close();
                }

            }

        }

        /// <summary>
        /// Close and reset
        /// </summary>
        private void Close()
        {
            if (isServer)
            {
                Log.LogMsg("Closed server connection manager.");
            } 
            else
            {
                Log.LogMsg("Closed connection to the server.");
            }

            //Disconnect connections
            if (connections != null)
            {
                foreach (NetworkConnection connection in connections)
                {
                    networkDriver.Disconnect(connection);
                }

                connections = null;
            }

            if (clientConnecting || connected || networkDriver.IsCreated)
            {                
                networkDriver.Dispose();
            }

            //Clear variables
            connected = false;
            clientConnecting = false;

        }


        /*
         * Public Functions
         */

        /// <summary>
        /// Get packet instance by type
        /// </summary>
        /// <typeparam name="T">The packet type</typeparam>
        /// <returns></returns>
        public T GetPacket<T>() where T : Packet
        {
            Type type = typeof(T);

            if(packetInstances.ContainsKey(type) == false)
            {
                packetInstances.Add(type, (T) FormatterServices.GetUninitializedObject(type));
            }
            
            return (T) packetInstances[type];

        }

        /// <summary>
        /// Method for sending packet to server. 
        /// </summary>
        /// <param name="packet">The packet</param>
        public void SendPacketToServer(Packet packet)
        {
            if (connections.Count != 1)
            {
                Log.LogError("Attempted sending packet to server when there is either 0 or more than 1 connection active, indicating this is the server.");
                return;
            }

            SendPacket(connections[0], packet);
        }

        /// <summary>
        /// Method for sending packet to client
        /// </summary>
        /// <param name="client">Client connection to receive the packet</param>
        /// <param name="packet">The packet</param>
        public void SendPacketToClient(NetworkConnection client, Packet packet)
        {
            SendPacket(client, packet);
        }

        /// <summary>
        /// Private method for sending packets to a connection
        /// </summary>
        /// <param name="c">The connection</param>
        /// <param name="packet">The Packet</param>
        private void SendPacket(NetworkConnection c, Packet packet)
        {
            //Ensure connection is valid
            if (c.IsCreated == false) return;

            byte[] packetBytes = packet.getBytes();
            
            if(packetBytes == null)
            {
                Log.LogError("Unable to retrieve packet bytes from packet: " + packet.packetId+". Bytes are null.");
                return;
            }

            //Choose pipeline
            NetworkPipeline pipeline = packet.packetReliabilityScheme == ReliabilityScheme.RELIABLE ? this.reliablePipeline : this.unreliablePipeline;

            //Create writer for sending the data
            DataStreamWriter writer = networkDriver.BeginSend(pipeline, c);           

            //Write packetID
            WriteEscapedBytes(ref writer, ByteConverter.getBytes<int>(packet.packetId));

            //Write packet data
            WriteEscapedBytes(ref writer, packetBytes);

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
        private void WriteEscapedBytes(ref DataStreamWriter writer, byte[] bytes)
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

            Close();
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
            clientConnecting = false;
            NotifyFailedConnect?.Invoke();
        }

        /// <summary>
        /// Triggers event when disconnected from server
        /// </summary>
        private void OnDisconnectedFromServer()
        {
            clientConnecting = false;
            NotifyOnDisconnectedFromServer?.Invoke();
        }

        /// <summary>
        /// Triggers event when disconnected from server
        /// </summary>
        private void OnConnectedToServer()
        {
            clientConnecting = false;
            connected = true;
            NotifyOnConnectedToServer?.Invoke();
        }
    }
}
