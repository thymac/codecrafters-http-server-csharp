using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

// 
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();
Console.WriteLine($"Started server");

try
{
  while (true)
  {
    // Accept a client connection
    TcpClient client = await server.AcceptTcpClientAsync();
    Console.WriteLine("Accepted TcpClient");

    // Handle the client connection asynchronously
    _ = HandleClientAsync(client);
  }
}
finally
{
  Console.WriteLine($"Stopping server");
  server.Stop();
}

static async Task HandleClientAsync(TcpClient client)
{
  try
  {
    using NetworkStream stream = client.GetStream();
    using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
    using StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)) { NewLine = "\r\n", AutoFlush = true };

    // Read the HTTP request line
    string requestLine = await reader.ReadLineAsync();
    if (string.IsNullOrEmpty(requestLine))
    {
      return;
    }
    Console.WriteLine($"Received request: {requestLine}");

    // Parse the request line
    string[] requestParts = requestLine.Split(' ');

    // Validate the request
    if (requestParts.Length != 3)
    {
      await SendResponseAsync(writer, "400 Bad Request");
      return;
    }

    string method = requestParts[0];
    string path = requestParts[1];
    string httpVersion = requestParts[2];

    if (method != "GET" || (httpVersion != "HTTP/1.1" && httpVersion != "HTTP/1.0"))
    {
      await SendResponseAsync(writer, "400 Bad Request");
      return;
    }

    // Send appropriate responses
    if (path == "/")
    {
      await SendResponseAsync(writer, "200 OK");
    }
    else
    {
      await SendResponseAsync(writer, "404 Not Found");
    }
  }
  catch (Exception ex)
  {
    Console.WriteLine($"Exception in HandleClientAsync: {ex}");
  }
  finally
  {
    client.Close();
  }
}

static async Task SendResponseAsync(StreamWriter writer, string statusCode)
{
  await writer.WriteLineAsync($"HTTP/1.1 {statusCode}\r\n");
}