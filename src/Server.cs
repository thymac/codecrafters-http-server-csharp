// using System;
// using System.IO;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Threading.Tasks;

// // 
// TcpListener server = new TcpListener(IPAddress.Any, 4221);
// server.Start();
// Console.WriteLine($"Started server");

// try
// {
//   while (true)
//   {
//     // Accept a client connection
//     TcpClient client = await server.AcceptTcpClientAsync();
//     Console.WriteLine("Accepted TcpClient");

//     // Handle the client connection asynchronously
//     _ = HandleClientAsync(client);
//   }
// }
// finally
// {
//   Console.WriteLine($"Stopping server");
//   server.Stop();
// }

// async Task HandleClientAsync(TcpClient client)
// {
//   try
//   {
//     using NetworkStream stream = client.GetStream();
//     using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
//     using StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)) { NewLine = "\r\n", AutoFlush = true };

//     // Read the HTTP request line
//     string? requestLine = await reader.ReadLineAsync();
//     if (string.IsNullOrEmpty(requestLine))
//     {
//       return;
//     }
//     Console.WriteLine($"Received request: {requestLine}");

//     // Read and discard the headers
//     string? headerLine;
//     while (!string.IsNullOrEmpty(headerLine = await reader.ReadLineAsync())) { }

//     // Parse the request line
//     string[] requestParts = requestLine.Split(' ');

//     // Validate the request
//     if (requestParts.Length != 3)
//     {
//       await SendResponseAsync(writer, "400 Bad Request", "Bad request");
//       return;
//     }

//     string method = requestParts[0];
//     string path = requestParts[1];
//     string httpVersion = requestParts[2];

//     if (method != "GET" || (httpVersion != "HTTP/1.1" && httpVersion != "HTTP/1.0"))
//     {
//       await SendResponseAsync(writer, "400 Bad Request", "Bad request");
//       return;
//     }

//     // Send appropriate responses
//     if (path == "/")
//     {
//       await SendResponseAsync(writer, "200 OK", "Welcome to root path");
//     }
//     else if (path.StartsWith("/echo/") && path.Length > "/echo/".Length)
//     {
//       string echoMessage = path.Substring("/echo/".Length);
//       await SendResponseAsync(writer, "200 OK", echoMessage);
//     }
//     else
//     {
//       await SendResponseAsync(writer, "404 Not Found", "The requested resource was not found.");
//     }
//   }
//   catch (Exception ex)
//   {
//     Console.WriteLine($"Exception in HandleClientAsync: {ex}");
//   }
//   finally
//   {
//     client.Close();
//   }
// }

// async Task SendResponseAsync(StreamWriter writer, string statusCode, string? content)
// {
//   StringBuilder responseBuilder = new StringBuilder();

//   responseBuilder.AppendLine($"HTTP/1.1 {statusCode}");
//   responseBuilder.AppendLine();
//   responseBuilder.AppendLine("Content-Type: text/plain; charset=UTF-8");
//   if (content == null)
//   {
//     responseBuilder.AppendLine("Content-Length: 0");
//     responseBuilder.AppendLine();
//   }
//   else
//   {
//     int contentLength = Encoding.UTF8.GetByteCount(content);
//     responseBuilder.AppendLine($"Content-Length: {contentLength}");
//     responseBuilder.AppendLine();
//     responseBuilder.AppendLine(content);
//   }

//   string response = responseBuilder.ToString();
//   await writer.WriteAsync(response);
//   Console.WriteLine($"Responded with: {response}");
// }


// using System.Net;
// using System.Net.Sockets;
// using System.Text;

// TcpListener server = new TcpListener(IPAddress.Any, 4221);
// server.Start();

// try
// {
//   while (true)
//   {
//     TcpClient client = server.AcceptTcpClient();

//     byte[] buffer = new byte[1024];
//     int bytesRead = stream.Read(buffer, 0, buffer.Length);

//     string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
//     string[] lines = request.Split(new string[] { "\r\n" }, StringSplitOptions.None);
//     string[] startLineParts = lines[0].Split(' ');

//     string? response;
//     if (startLineParts[1] == "/")
//     {
//       response = "HTTP/1.1 200 OK\r\n\r\n";
//     }
//     else if (startLineParts[1].StartsWith("/echo/"))
//     {
//       string message = startLineParts[1].Substring("/echo/".Length);
//       response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {message.Length}\r\n\r\n{message}";
//     }
//     else if (startLineParts[1].StartsWith("/user-agent"))
//     {
//       Dictionary<string, string> headers = new Dictionary<string, string>();
//       for (int i = 1; i < lines[1].Length; i++)
//       {
//         if (string.IsNullOrEmpty(lines[i]))
//           break;

//         int colonIndex = lines[i].IndexOf(":");
//         if (colonIndex > 0)
//         {
//           string headerKey = lines[i].Substring(0, colonIndex).Trim();
//           string headerValue = lines[i].Substring(colonIndex + 1).Trim();
//           headers[headerKey] = headerValue;
//         }
//       }

//       if (headers.TryGetValue("User-Agent", out string? userAgent))
//       {
//         response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {userAgent.Length}\r\n\r\n{userAgent}";
//       }
//       else
//       {
//         response = "HTTP/1.1 404 NOT FOUND\r\n\r\n";
//       }
//     }
//     else
//     {
//       response = "HTTP/1.1 404 Not Found\r\n\r\n";
//     }

//     byte[] responseBytes = Encoding.ASCII.GetBytes(response);
//     stream.Write(responseBytes, 0, responseBytes.Length);
//     client.Close();
//   }
// }
// catch (Exception ex)
// {
//   Console.WriteLine($"Error encountered: {ex.Message}");
// }
// finally
// {
//   Console.WriteLine("Stopping the server");
//   server.Stop();
// }

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
  static async Task Main(string[] args)
  {
    TcpListener server = new TcpListener(IPAddress.Any, 4221);
    server.Start();
    Console.WriteLine("Server started");

    try
    {
      while (true)
      {
        TcpClient client = await server.AcceptTcpClientAsync();
        _ = Task.Run(() => HandleClientAsync(client));
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error encountered: {ex.Message}");
    }
  }

  static async Task HandleClientAsync(TcpClient client)
  {
    try
    {
      NetworkStream stream = client.GetStream();

      byte[] buffer = new byte[1024];
      int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

      string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
      string[] lines = request.Split(new string[] { "\r\n" }, StringSplitOptions.None);
      string[] startLineParts = lines[0].Split(' ');
      string? response;
      if (startLineParts[1] == "/")
      {
        response = "HTTP/1.1 200 OK\r\n\r\n";
      }
      else if (startLineParts[1].StartsWith("/echo/"))
      {
        string message = startLineParts[1].Substring("/echo/".Length);
        response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {message.Length}\r\n\r\n{message}";
      }
      else if (startLineParts[1].StartsWith("/user-agent"))
      {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        for (int i = 1; i < lines.Length; i++)
        {
          if (string.IsNullOrEmpty(lines[i]))
            break;
          int colonIndex = lines[i].IndexOf(":");
          if (colonIndex > 0)
          {
            string headerKey = lines[i].Substring(0, colonIndex).Trim();
            string headerValue = lines[i].Substring(colonIndex + 1).Trim();
            headers[headerKey] = headerValue;
          }
        }
        if (headers.TryGetValue("User-Agent", out string? userAgent))
        {
          response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {userAgent.Length}\r\n\r\n{userAgent}";
        }
        else
        {
          response = "HTTP/1.1 404 NOT FOUND\r\n\r\n";
        }
      }
      else
      {
        response = "HTTP/1.1 404 Not Found\r\n\r\n";
      }
      byte[] responseBytes = Encoding.ASCII.GetBytes(response);
      await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
      client.Close();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error encountered while handling client: {ex.Message}");
    }
  }
}