using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
// Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();
// server.AcceptSocket(); // wait for client
Console.WriteLine($"Started server");

try {
  // server.AcceptSocket(); // wait for client
  var client = await server.AcceptTcpClientAsync();
  Console.WriteLine($"Accepted TcpClient");
  var stream = client.GetStream();
  var message = $"HTTP/1.1 200 OK\r\n\r\n";
  var encodedMessage = Encoding.UTF8.GetBytes(message);
  await stream.WriteAsync(encodedMessage);
  Console.WriteLine($"Responded message: \"{message}\"");
} finally {
  Console.WriteLine($"Stopping server");
  server.Stop();
}