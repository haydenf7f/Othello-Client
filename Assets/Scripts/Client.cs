using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 23960;
    public int clientID = 0;
    public TCP tcp;
    public Player player;

    private bool isConnected = false;

    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    // Awake function is called when the script instance is being loaded
    private void Awake() {
        // Check if an instance of this class already exists
        if (instance == null) {
            // If not, set the static instance variable to this object
            instance = this;
        } else if (instance != this) {
            // If an instance already exists and it's not this object, destroy this object
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    // Start function is called on the frame when a script is enabled
    private void Start() {
        // Create a new TCP object
        tcp = new TCP();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    // ConnectToServer function initializes client data and connects to server
    public void ConnectToServer() {
        // Initialize client data
        InitializeClientData();

        isConnected = true;
        
        // Connect to server using TCP object
        tcp.Connect();
    }

    // OnConnectedToServer function is called when the client has connected to the server
    public void OnConnectedToServer() {
        // Log message indicating successful connection
        Debug.Log("Connected to server!");
    }

    // Define TCP class for handling TCP connections
    public class TCP {
        // Declare TcpClient socket variable
        public TcpClient socket;

        // Declare NetworkStream, Packet, and receive buffer variables
        private NetworkStream stream;
        private Packet receivedData;
        private byte[] reveiveBuffer;

        // Connect function initializes socket and begins connection attempt
        public void Connect() {
            // Create new TcpClient socket with data buffer size
            socket = new TcpClient {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            // Initialize receive buffer with data buffer size
            reveiveBuffer = new byte[dataBufferSize];
            // Begin connection attempt with server using socket.BeginConnect method
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
        }

        // ConnectCallback function is called when the connection attempt is complete
        private void ConnectCallback(IAsyncResult _result) {
            // End connection attempt
            socket.EndConnect(_result);

            // Check if the socket is connected
            if (!socket.Connected) {
                return;
            }

            // Set network stream variable to socket's stream
            stream = socket.GetStream();

            // Initialize received data object
            receivedData = new Packet();

            // Begin reading data from stream using BeginRead method
            stream.BeginRead(reveiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        // SendData function attempts to send data to the server using the network stream object
        public void SendData(Packet _packet) {
            try {
                if (socket != null) {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex) {
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }

        // ReceiveCallback function is called when data is received from the server
        private void ReceiveCallback(IAsyncResult _result) {
            try {
                // Read the number of bytes received from the stream
                int _byteLength = stream.EndRead(_result);
                // If no bytes were received, return
                if (_byteLength <= 0) 
                {
                    instance.Disconnect();
                    return;
                }
                // Copy the received data into a new byte array
                byte[] _data = new byte[_byteLength];
                Array.Copy(reveiveBuffer, _data, _byteLength);

                // Reset received data object with the new data
                receivedData.Reset(HandleData(_data));
                // Begin reading from the stream again
                stream.BeginRead(reveiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception _ex) {
                Debug.Log($"Error receiving TCP data: {_ex}");
                Disconnect();
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            // Set received data object bytes to the received data
            receivedData.SetBytes(_data);

            Debug.Log($"Received data length: {receivedData.Length()}");

            // Check if received data contains more than 4 unread bytes (size of int)
            if (receivedData.UnreadLength() >= 4) {
                // Read the packet length from the received data
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0) {
                    return true;
                }
            }

            Debug.Log($"Packet length: {_packetLength}");

            // Loop through packets in received data and execute corresponding PacketHandlers
            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength()) {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() => {
                    using (Packet _packet = new Packet(_packetBytes)) {         
                        int _packetID = _packet.ReadInt();
                        Debug.Log($"Received packet with ID: {_packetID}");
                        packetHandlers[_packetID](_packet);
                    }
                });

                // Reset packet length variable
                _packetLength = 0;
                // Read packet length from received data
                if (receivedData.UnreadLength() >= 4) {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0) {
                        return true;
                    }
                }
            }

            // Return true if all data has been handled, false otherwise
            if (_packetLength <= 1) {
                return true;
            }

            return false;
        }

        private void Disconnect()
        {
            // Close socket and stream
            stream = null;
            socket.Close();

            // Reset received data object
            receivedData = null;
            // Reset receive buffer
            reveiveBuffer = null;
        }
    }

    private void InitializeClientData() {
        packetHandlers = new Dictionary<int, PacketHandler>() {
            { (int)ServerPackets.Welcome, ClientHandle.Welcome },
            { (int)ServerPackets.ServerMessage, ClientHandle.ServerMessage },
            { (int)ServerPackets.StartGame, ClientHandle.StartGame },
            { (int)ServerPackets.GameUpdate, ClientHandle.GameUpdate }
        };
        Debug.Log("Initialized packets.");
    }

    private void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();

            Debug.Log("Disconnected from server.");
        }
    }
}
