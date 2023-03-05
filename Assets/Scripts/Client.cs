// using Newtonsoft.Json;
// using System.Net.Sockets;
// using System.Text;

// public class OthelloClient {
//     private const int BufferSize = 1024;
//     private readonly byte[] receiveBuffer = new byte[BufferSize];
//     private readonly Socket socket;

//     public OthelloClient() {
//         // Create a new socket
//         socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//     }

//     public void Connect(string host, int port) {
//         // Connect to the server
//         socket.Connect(host, port);

//         // Start receiving messages in a separate thread
//         socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallback, null);
//     }

//     private void ReceiveCallback(IAsyncResult ar) {
//         // End the asynchronous receive operation
//         int bytesRead = socket.EndReceive(ar);

//         if (bytesRead > 0) {
//             // Convert the received byte array to a string
//             string message = Encoding.ASCII.GetString(receiveBuffer, 0, bytesRead);

//             // Deserialize the JSON message to a GameState object
//             GameState gameState = JsonConvert.DeserializeObject<GameState>(message);

//             // Do something with the received game state
//             // ...

//             // Start receiving messages again
//             socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallback, null);
//         }
//     }

//     public void SendGameState(GameState gameState) {
//         // Serialize the game state to a JSON message
//         string message = JsonConvert.SerializeObject(gameState);

//         // Convert the message to a byte array and send it over the socket
//         byte[] buffer = Encoding.ASCII.GetBytes(message);
//         socket.Send(buffer);
//     }
// }
