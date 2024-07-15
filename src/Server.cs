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

try
{
  // Accept a client connection
  TcpClient client = await server.AcceptTcpClientAsync();
  Console.WriteLine("Accepted TcpClient");

  using (NetworkStream stream = client.GetStream())
  {
    // Prepare the HTTP response
    string message = "HTTP/1.1 200 OK\r\nContent-Length: 0\r\n\r\n";
    byte[] encodedMessage = Encoding.UTF8.GetBytes(message);
    // Send the HTTP response to the client
    await stream.WriteAsync(encodedMessage, 0, encodedMessage.Length);
    Console.WriteLine($"Responded message: \"{message}\"");
  }

  client.Close();
}
finally
{
  Console.WriteLine($"Stopping server");
  server.Stop();
}