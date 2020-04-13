using System;
using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using System.Collections.Generic;
using UnityEngine.Assertions;
using Engine.Networking.Utility;
using Engine.Configuration;
using Engine.Logging;

namespace Engine.Networking
{
    public class ConnectionManager : Manager
    {

        /*
         * Override Functions
         */

        //Called on start
        internal override void init(params System.Object[] parameters)
        {
            ListenerType listenerType = (ListenerType)parameters[0];
            this._isServer = listenerType == ListenerType.SERVER;

            if (isServer)
                start();
        }

        //Called every frame on main thread
        internal override void update()
        {

            if (isServer)
                serverUpdateLoop();
            else if (networkDriver.IsCreated)
                clientUpdateLoop();
        }

        //Called on program shutdown
        internal override void shutdown()
        {
            close();
        }

        /*
         * Internal Variables
         */

        //Is instance running as server or client
        private bool _isServer;
        public virtual bool isServer { get => _isServer; }

        //Driver
        internal NetworkDriver networkDriver;


        //Pipelines
        private NetworkPipeline unreliablePipeline;
        private NetworkPipeline reliablePipeline;

        //Connections List
        private NativeList<NetworkConnection> connections;
        private NativeList<int> escapedConnections;

        //Event Delegates
        internal delegate void NotifyClientDisconnectedDelegate(NetworkConnection connection);
        internal delegate void NotifyClientConnectedDelegate(NetworkConnection connection);
        internal delegate void NotifyPacketReceivedDelegate(NetworkConnection connection, int packetId, byte[] rawPacket);
        internal delegate void NotifyFailedConnectDelegate();
        internal delegate void NotifyOnDisconnectedFromServerDelegate();
        internal delegate void NotifyOnConnectedToServerDelegate();

        //Events
        internal event NotifyClientDisconnectedDelegate NotifyClientDisconnected;
        internal event NotifyClientConnectedDelegate NotifyClientConnected;
        internal event NotifyPacketReceivedDelegate NotifyPacketReceived;
        internal event NotifyFailedConnectDelegate NotifyFailedConnect;
        internal event NotifyOnDisconnectedFromServerDelegate NotifyOnDisconnectedFromServer;
        internal event NotifyOnConnectedToServerDelegate NotifyOnConnectedToServer;

        //Client connected boolean
        private bool connected;

        internal enum ListenerType
        {
            SERVER,
            CLIENT
        }

        /*
         * Internal Functions
         */

        //Start method
        internal void start()
        {

            connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
            escapedConnections = new NativeList<int>(16, Allocator.Persistent);

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

        //Update Loops
        private void serverUpdateLoop()
        {
            networkDriver.ScheduleUpdate().Complete();

            // Clean Up Connections
            for (int i = 0; i < connections.Length; i++)
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

            for (int index = 0; index < connections.Length; index++)
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
                                    escapedConnections.RemoveAtSwapBack(connections[index].InternalId);

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
                    }
                }
            }

        }

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

        //Close and reset
        private void close()
        {

            Log.LogMsg("Closed connection.");

            if(connections.IsCreated)
                connections.Dispose();

            if (escapedConnections.IsCreated)
                escapedConnections.Dispose();

            if (networkDriver.IsCreated)
                networkDriver.Dispose();

            //Clear variables
            connected = false;

        }

        //Public Gateway method for sending packet to server. 
        internal void sendPacketToServer(Packet packet)
        {
            if (connections.Length != 1)
            {
                Log.LogError("Attempted sending packet to server when there is either 0 or more than 1 connection active, indicating this is the server.");
                return;
            }

            sendPacket(connections[0], packet);
        }

        //Public Gateway method for sending packet to client
        internal void sendPacketToClient(NetworkConnection client, Packet packet)
        {
            sendPacket(client, packet);
        }

        //Internal Method for sending packets
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

        //Escape our bytes and send over network
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


        //Client disconnect
        internal void Disconnect()
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

        //Triggers event for packet receiving
        private void OnPacketReceived(NetworkConnection c, byte[] bytes)
        {

            int packetId = PacketReader.ReadInt(ref bytes);
            NotifyPacketReceived?.Invoke(c, packetId, bytes);

        }

        //Triggers event for a client connecting
        private void OnClientConnected(NetworkConnection client)
        {
            NotifyClientConnected?.Invoke(client);

            Log.LogMsg("New connection! Server: " + isServer + ".");
        }

        //Triggers event for a client disconnecting
        private void OnClientDisconnected(NetworkConnection client)
        {
            NotifyClientDisconnected?.Invoke(client);
        }

        //Triggers event for a client failing to connect
        internal void OnFailedConnect()
        {
            NotifyFailedConnect?.Invoke();
        }

        //Triggers event when disconnected from server
        private void OnDisconnectedFromServer()
        {
            NotifyOnDisconnectedFromServer?.Invoke();
        }

        //Triggers event when connected to server
        private void OnConnectedToServer()
        {
            NotifyOnConnectedToServer?.Invoke();
        }
    }
}
