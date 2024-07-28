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

// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Threading.Tasks;
// class Program
// {
//   static async Task Main(string[] args)
//   {
//     TcpListener server = new TcpListener(IPAddress.Any, 4221);
//     server.Start();
//     Console.WriteLine("Server started");

//     try
//     {
//       while (true)
//       {
//         TcpClient client = await server.AcceptTcpClientAsync();
//         _ = Task.Run(() => HandleClientAsync(client));
//       }
//     }
//     catch (Exception ex)
//     {
//       Console.WriteLine($"Error encountered: {ex.Message}");
//     }
//   }

//   static async Task HandleClientAsync(TcpClient client)
//   {
//     try
//     {
//       NetworkStream stream = client.GetStream();

//       byte[] buffer = new byte[1024];
//       int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

//       string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
//       string[] lines = request.Split(new string[] { "\r\n" }, StringSplitOptions.None);
//       string[] startLineParts = lines[0].Split(' ');
//       string? response;

//       if (startLineParts[1] == "/")
//       {
//         response = "HTTP/1.1 200 OK\r\n\r\n";
//       }

//       else if (startLineParts[1].StartsWith("/echo/"))
//       {
//         string message = startLineParts[1].Substring("/echo/".Length);
//         response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {message.Length}\r\n\r\n{message}";
//       }

//       else if (startLineParts[1].StartsWith("/user-agent"))
//       {
//         Dictionary<string, string> headers = new Dictionary<string, string>();
//         for (int i = 1; i < lines.Length; i++)
//         {
//           if (string.IsNullOrEmpty(lines[i]))
//             break;

//           int colonIndex = lines[i].IndexOf(":");
//           if (colonIndex > 0)
//           {
//             string headerKey = lines[i].Substring(0, colonIndex).Trim();
//             string headerValue = lines[i].Substring(colonIndex + 1).Trim();
//             headers[headerKey] = headerValue;
//           }
//         }

//         if (headers.TryGetValue("User-Agent", out string? userAgent))
//         {
//           response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {userAgent.Length}\r\n\r\n{userAgent}";
//         }
//         else
//         {
//           response = "HTTP/1.1 404 NOT FOUND\r\n\r\n";
//         }
//       }

//       else if (startLineParts[1].StartsWith("/files/"))
//       {
//         string directory = "/files/";
//         string fileName = startLineParts[1].Substring(directory.Length);

//         if (string.IsNullOrEmpty(fileName))
//         {
//           response = "HTTP/1.1 400 Bad Request\r\n\r\n";
//         }
//         else
//         {
//           string filePath = Path.Combine(directory, fileName);
//           Console.WriteLine($"Requested file: {filePath}");

//           if (!File.Exists(filePath))
//           {
//             response = "HTTP/1.1 404 Not Found\r\n\r\n";
//           }
//           else
//           {
//             try
//             {
//               string fileContent;
//               using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
//               using (StreamReader reader = new StreamReader(fs, Encoding.UTF8))
//               {
//                 fileContent = await reader.ReadToEndAsync();
//               }

//               int contentLength = Encoding.UTF8.GetByteCount(fileContent);
//               response = $"HTTP/1.1 200 OK\r\nContent-Type: application/octet-stream\r\nContent-Length: {contentLength}\r\n\r\n{fileContent}";
//             }
//             catch (Exception ex)
//             {
//               response = "HTTP/1.1 500 Internal Server Error\r\n\r\n";
//               Console.WriteLine($"Error reading file: {ex.Message}");
//             }
//           }
//         }
//       }
//       else
//       {
//         response = "HTTP/1.1 404 Not Found\r\n\r\n";
//       }

//       byte[] responseBytes = Encoding.ASCII.GetBytes(response);
//       await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
//       client.Close();
//     }
//     catch (Exception ex)
//     {
//       Console.WriteLine($"Error encountered while handling client: {ex.Message}");
//     }
//   }
// }

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

// You can use print statements as follows for debugging, they'll be visible
// when running tests.
Console.WriteLine("Logs from your program will appear here!");

// wait for client
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();

while (true)
{
  var socket = await server.AcceptSocketAsync();
  _ = ProcessRequestAsync(socket); // Fire and forget
}

static async Task ProcessRequestAsync(Socket socket)
{
  var buffer = new byte[1024];
  int received;

  using (socket)
  {
    try
    {
      received = await socket.ReceiveAsync(buffer, SocketFlags.None);
      var requestMessage = new HttpRequestMessage(Encoding.UTF8.GetString(buffer, 0, received));

      HttpResponseMessage responseMessage;

      if (requestMessage.Path == "/")
      {
        responseMessage = new HttpResponseMessage("200", "OK");
      }
      else if (requestMessage.Path.StartsWith(Routes.Echo))
      {
        var content = requestMessage.Path.Length == Routes.Echo.Length
            ? null
            : requestMessage.Path.Substring(Routes.Echo.Length + 1);
        responseMessage = new HttpResponseMessage("200", "OK", "text/plain", content);
      }
      else if (requestMessage.Path.StartsWith(Routes.UserAgent))
      {
        responseMessage = new HttpResponseMessage("200", "OK", "text/plain", requestMessage.UserAgent);
      }
      else if (requestMessage.Path.StartsWith(Routes.Files))
      {
        var directory = Environment.GetCommandLineArgs()[2];
        var fileName = requestMessage.Path.Split("/")[2];
        var pathFile = Path.Combine(directory, fileName);
        if (File.Exists(pathFile))
        {
          var contentFile = await File.ReadAllTextAsync(pathFile);
          responseMessage = new HttpResponseMessage("200", "OK", "application/octet-stream", contentFile);
        }
        else
        {
          responseMessage = new HttpResponseMessage("404", "Not Found");
        }
      }
      else
      {
        responseMessage = new HttpResponseMessage("404", "Not Found");
      }

      await socket.SendAsync(responseMessage.GetBytes(), SocketFlags.None);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error processing request: {ex.Message}");
    }
  }
}

public static class Routes
{
  public static string UserAgent = "/user-agent";
  public static string Echo = "/echo";
  public static string Files = "/files";
}

class HttpRequestMessage
{
  public string Path { get; }
  public string UserAgent { get; }
  public string Host { get; }

  public HttpRequestMessage(string message)
  {
    var lines = message.Split("\r\n");
    Path = lines[0].Split(' ')[1];
    Host = lines[1].Replace("Host: ", string.Empty);
    UserAgent = lines.Length > 2 ? lines[2].Replace("User-Agent: ", string.Empty) : string.Empty;
  }
}

class HttpResponseMessage
{
  private const string ProtocolVersion = "HTTP/1.1";
  private readonly string _statusCode;
  private readonly string _statusDescription;
  private readonly string _contentType;
  private readonly string _content;
  private bool _hasContent => !string.IsNullOrWhiteSpace(_contentType);
  private int _contentLength => string.IsNullOrWhiteSpace(_content) ? 0 : _content.Length;

  public HttpResponseMessage(string statusCode, string statusDescription, string? contentType = null, string? content = null)
  {
    _statusCode = statusCode;
    _statusDescription = statusDescription;
    _contentType = contentType;
    _content = content;
  }

  public override string ToString()
  {
    var stringBuilder = new StringBuilder();
    stringBuilder.Append($"{ProtocolVersion} {_statusCode} {_statusDescription}\r\n");
    if (_hasContent)
    {
      stringBuilder.Append($"Content-Type: {_contentType}\r\n");
      stringBuilder.Append($"Content-Length: {_contentLength}\r\n");
      stringBuilder.Append("\r\n");
      stringBuilder.Append(_content);
      return stringBuilder.ToString();
    }
    stringBuilder.Append("\r\n");
    return stringBuilder.ToString();
  }

  public byte[] GetBytes()
  {
    return Encoding.UTF8.GetBytes(ToString());
  }
}

