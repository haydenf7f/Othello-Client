// using Newtonsoft.Json;
// using System;
// using System.Collections.Generic;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;

// public class OthelloServer {
//     private const int BufferSize = 1024;
//     private readonly byte[] receiveBuffer = new byte[BufferSize];
//     private readonly List<Socket> clientSockets = new List<Socket>();
//     private readonly Socket serverSocket;

//     public OthelloServer(string host, int port) {
//         // Create a new socket and bind it to the specified host and port
//         serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//         serverSocket.Bind(new IPEndPoint(IPAddress.Parse(host), port));

//         // Listen for incoming connections
//         serverSocket.Listen(10);
//         Console.WriteLine($"Server listening on {host}:{port}");

//         // Start accepting incoming connections in a separate thread
//         serverSocket.BeginAccept(AcceptCallback, null);
//     }

//     private void AcceptCallback(IAsyncResult ar) {
//         // End the asynchronous accept operation and get the new socket for the client connection
//         Socket clientSocket = serverSocket.EndAccept(ar);

//         // Add the new client socket to the list of client sockets
//         clientSockets.Add(clientSocket);
//         Console.WriteLine($"Client connected: {clientSocket.RemoteEndPoint}");

//         // Start receiving messages from the client in a separate thread
//         clientSocket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallback, clientSocket);

//         // Start accepting incoming connections again
//         serverSocket.BeginAccept(AcceptCallback, null);
//     }

//     private void ReceiveCallback(IAsyncResult ar) {
//         // Get the socket for the client connection and end the asynchronous receive operation
//         Socket clientSocket = (Socket)ar.AsyncState;
//         int bytesRead = clientSocket.EndReceive(ar);

//         if (bytesRead > 0) {
//             // Convert the received byte array to a string
//             string message = Encoding.ASCII.GetString(receiveBuffer, 0, bytesRead);

//             // Deserialize the JSON message to a GameState object
//             GameState gameState = JsonConvert.DeserializeObject<GameState>(message);

//             // Do something with the received game state
//             // ...

//             // Broadcast the game state to all connected clients
//             BroadcastGameState(gameState);

//             // Start receiving messages from the client again
//             clientSocket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallback, clientSocket);
//         }
//     }

//     private void BroadcastGameState(GameState gameState) {
//         // Serialize the game state to a JSON message
//         string message = JsonConvert.SerializeObject(gameState);

//         // Convert the message to a byte array and send it to all connected clients
//         byte[] buffer = Encoding.ASCII.GetBytes(message);

//         foreach (Socket clientSocket in clientSockets) {
//             clientSocket.Send(buffer);
//         }
//     }
// }