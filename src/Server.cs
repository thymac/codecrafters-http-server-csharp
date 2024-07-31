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

// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Threading.Tasks;

// // You can use print statements as follows for debugging, they'll be visible
// // when running tests.
// Console.WriteLine("Logs from your program will appear here!");

// // wait for client
// TcpListener server = new TcpListener(IPAddress.Any, 4221);
// server.Start();

// while (true)
// {
//   var socket = await server.AcceptSocketAsync();
//   _ = ProcessRequestAsync(socket); // Fire and forget
// }

// static async Task ProcessRequestAsync(Socket socket)
// {
//   var buffer = new byte[1024];
//   int received;

//   using (socket)
//   {
//     try
//     {
//       received = await socket.ReceiveAsync(buffer, SocketFlags.None);
//       var requestMessage = new HttpRequestMessage(Encoding.UTF8.GetString(buffer, 0, received));

//       HttpResponseMessage responseMessage;

//       if (requestMessage.Path == "/")
//       {
//         responseMessage = new HttpResponseMessage("200", "OK");
//       }
//       else if (requestMessage.Path.StartsWith(Routes.Echo))
//       {
//         var content = requestMessage.Path.Length == Routes.Echo.Length
//             ? null
//             : requestMessage.Path.Substring(Routes.Echo.Length + 1);
//         responseMessage = new HttpResponseMessage("200", "OK", "text/plain", content);
//       }
//       else if (requestMessage.Path.StartsWith(Routes.UserAgent))
//       {
//         responseMessage = new HttpResponseMessage("200", "OK", "text/plain", requestMessage.UserAgent);
//       }
//       else if (requestMessage.Path.StartsWith(Routes.Files))
//       {
//         var directory = Environment.GetCommandLineArgs()[2];
//         var fileName = requestMessage.Path.Split("/")[2];
//         var pathFile = Path.Combine(directory, fileName);

//         if (File.Exists(pathFile))
//         {
//           var contentFile = await File.ReadAllTextAsync(pathFile);
//           responseMessage = new HttpResponseMessage("200", "OK", "application/octet-stream", contentFile);
//         }
//         else
//         {
//           responseMessage = new HttpResponseMessage("404", "Not Found");
//         }
//       }
//       else if (requestMessage.Path.StartsWith(Routes.Files) && requestMessage.Method == "Post")
//       {
//         try
//         {
//           var directory = Environment.GetCommandLineArgs()[2];
//           Console.WriteLine($"Directory: {directory}");

//           var fileName = requestMessage.Path.Split("/")[2];
//           Console.WriteLine($"FileName: {fileName}");

//           var pathFile = Path.Combine(directory, fileName);
//           Console.WriteLine($"PathFile: {pathFile}");

//           var body = requestMessage.Body;
//           Console.WriteLine($"Body: {body}");

//           await File.WriteAllTextAsync(pathFile, body);

//           responseMessage = new HttpResponseMessage("201", "Created");
//         }
//         catch (Exception ex)
//         {
//           Console.WriteLine($"Error creating a new file with fetched text: {ex.Message}");
//           responseMessage = new HttpResponseMessage("500", "Internal Server Error");
//         }
//       }
//       else
//       {
//         responseMessage = new HttpResponseMessage("404", "Not Found");
//       }

//       await socket.SendAsync(responseMessage.GetBytes(), SocketFlags.None);
//     }
//     catch (Exception ex)
//     {
//       Console.WriteLine($"Error processing request: {ex.Message}");
//     }
//   }
// }

// public static class Routes
// {
//   public static string UserAgent = "/user-agent";
//   public static string Echo = "/echo";
//   public static string Files = "/files";
// }

// class HttpRequestMessage
// {
//   public string Method { get; }
//   public string Path { get; }
//   public string UserAgent { get; }
//   public string Host { get; }
//   public string Body { get; }

//   public HttpRequestMessage(string message)
//   {
//     var lines = message.Split("\r\n");
//     Method = lines[0].Split(' ')[0];
//     Path = lines[0].Split(' ')[1];
//     Host = lines[1].Replace("Host: ", string.Empty);
//     UserAgent = lines.Length > 2 ? lines[2].Replace("User-Agent: ", string.Empty) : string.Empty;
//     Body = lines.Length > 3 ? lines[lines.Length - 1] : string.Empty;
//   }
// }

// class HttpResponseMessage
// {
//   private const string ProtocolVersion = "HTTP/1.1";
//   private readonly string _statusCode;
//   private readonly string _statusDescription;
//   private readonly string? _contentType;
//   private readonly string? _content;
//   private bool _hasContent => !string.IsNullOrWhiteSpace(_contentType);
//   private int _contentLength => string.IsNullOrWhiteSpace(_content) ? 0 : _content.Length;

//   public HttpResponseMessage(string statusCode, string statusDescription, string? contentType = null, string? content = null)
//   {
//     _statusCode = statusCode;
//     _statusDescription = statusDescription;
//     _contentType = contentType;
//     _content = content;
//   }

//   public override string ToString()
//   {
//     var stringBuilder = new StringBuilder();
//     stringBuilder.Append($"{ProtocolVersion} {_statusCode} {_statusDescription}\r\n");
//     if (_hasContent)
//     {
//       stringBuilder.Append($"Content-Type: {_contentType}\r\n");
//       stringBuilder.Append($"Content-Length: {_contentLength}\r\n");
//       stringBuilder.Append("\r\n");
//       stringBuilder.Append(_content);
//       return stringBuilder.ToString();
//     }
//     stringBuilder.Append("\r\n");
//     return stringBuilder.ToString();
//   }

//   public byte[] GetBytes()
//   {
//     return Encoding.UTF8.GetBytes(ToString());
//   }
// }

// 

// using System;
// using System.IO;
// using System.IO.Compression;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Threading.Tasks;

// public static class Routes
// {
//   public static string UserAgent = "/user-agent";
//   public static string Echo = "/echo";
//   public static string Files = "/files";
// }

// class Program
// {
//   public static async Task Main()
//   {
//     // You can use print statements as follows for debugging, they'll be visible
//     // when running tests.
//     Console.WriteLine("Logs from your program will appear here!");

//     // wait for client
//     TcpListener server = new TcpListener(IPAddress.Any, 4221);
//     server.Start();

//     while (true)
//     {
//       var socket = await server.AcceptSocketAsync();
//       _ = ProcessRequestAsync(socket); // Fire and forget
//     }
//   }

//   static async Task ProcessRequestAsync(Socket socket)
//   {
//     var buffer = new byte[1024];
//     int received;

//     using (socket)
//     {
//       try
//       {
//         received = await socket.ReceiveAsync(buffer, SocketFlags.None);
//         var requestMessage = new HttpRequestMessage(Encoding.UTF8.GetString(buffer, 0, received));

//         HttpResponseMessage responseMessage;

//         if (requestMessage.Path == "/")
//         {
//           responseMessage = new HttpResponseMessage("200", "OK");
//         }
//         else if (requestMessage.Path.StartsWith(Routes.Echo))
//         {
//           var content = requestMessage.Path.Length == Routes.Echo.Length
//               ? null
//               : requestMessage.Path.Substring(Routes.Echo.Length + 1);

//           if (requestMessage.AcceptEncoding?.Contains("gzip") == true)
//           {
//             var compressedContent = compressContent(content);
//             responseMessage = new HttpResponseMessage("200", "OK", "text/plain", "gzip", compressedContent);
//           }
//           else
//           {
//             responseMessage = new HttpResponseMessage("200", "OK", "text/plain", null, content);
//           }
//         }
//         else if (requestMessage.Path.StartsWith(Routes.UserAgent))
//         {
//           responseMessage = new HttpResponseMessage("200", "OK", "text/plain", null, requestMessage.UserAgent);
//         }
//         else if (requestMessage.Path.StartsWith(Routes.Files))
//         {
//           var directory = Environment.GetCommandLineArgs()[2];
//           var fileName = requestMessage.Path.Split("/")[2];
//           var pathFile = Path.Combine(directory, fileName);

//           if (File.Exists(pathFile))
//           {
//             var contentFile = await File.ReadAllTextAsync(pathFile);
//             responseMessage = new HttpResponseMessage("200", "OK", "application/octet-stream", null, contentFile);
//           }
//           else if (requestMessage.Method == "POST")
//           {
//             try
//             {
//               var body = requestMessage.Body;
//               await File.WriteAllTextAsync(pathFile, body);
//               responseMessage = new HttpResponseMessage("201", "Created");
//             }
//             catch (Exception ex)
//             {
//               Console.WriteLine($"Error creating a new file with fetched text: {ex.Message}");
//               responseMessage = new HttpResponseMessage("500", "Internal Server Error");
//             }
//           }
//           else
//           {
//             responseMessage = new HttpResponseMessage("404", "Not Found");
//           }
//         }
//         else
//         {
//           responseMessage = new HttpResponseMessage("404", "Not Found");
//         }

//         await socket.SendAsync(responseMessage.GetBytes(), SocketFlags.None);
//       }
//       catch (Exception ex)
//       {
//         Console.WriteLine($"Error processing request: {ex.Message}");
//       }
//     }
//   }

//   private static byte[] compressContent(string? content)
//   {
//     if (string.IsNullOrEmpty(content))
//       return Array.Empty<byte>();

//     using var compressedStream = new MemoryStream();
//     using (var gzipStream = new GzipStream(compressedStream, CompressionMode.Compress))
//     using (var writer = new StreamWriter(gzipStream, Encoding.UTF8))
//     {
//       writer.Write(content);
//     }

//     return compressedStream.ToArray();
//   }
// }

// class HttpRequestMessage
// {
//   public string Method { get; }
//   public string Path { get; }
//   public string UserAgent { get; }
//   public string Host { get; }
//   public string AcceptEncoding { get; }
//   public string Body { get; }

//   public HttpRequestMessage(string message)
//   {
//     var lines = message.Split("\r\n");
//     Method = lines[0].Split(' ')[0];
//     Path = lines[0].Split(' ')[1];
//     Host = lines.FirstOrDefault(line => line.StartsWith("Host: "))?.Replace("Host: ", string.Empty) ?? string.Empty;
//     UserAgent = lines.FirstOrDefault(line => line.StartsWith("User-Agent: "))?.Replace("User-Agent: ", string.Empty) ?? string.Empty;
//     AcceptEncoding = lines.FirstOrDefault(line => line.StartsWith("Accept-Encoding: "))?.Replace("Accept-Encoding: ", string.Empty) ?? string.Empty;
//     var bodyIndex = message.IndexOf("\r\n\r\n");
//     Body = bodyIndex >= 0 ? message.Substring(bodyIndex + 4) : string.Empty;
//   }
// }

// class HttpResponseMessage
// {
//   private const string ProtocolVersion = "HTTP/1.1";
//   private readonly string _statusCode;
//   private readonly string _statusDescription;
//   private readonly string? _contentType;
//   private readonly string? _contentEncoding;
//   private readonly string? _content;
//   private bool _hasContent => !string.IsNullOrWhiteSpace(_content);
//   private bool _hasEncoding => !string.IsNullOrWhiteSpace(_contentEncoding);
//   private int _contentLength => string.IsNullOrWhiteSpace(_content) ? 0 : Encoding.UTF8.GetByteCount(_content);

//   public HttpResponseMessage(string statusCode, string statusDescription, string? contentType = null, string? contentEncoding = null, string? content = null)
//   {
//     _statusCode = statusCode;
//     _statusDescription = statusDescription;
//     _contentType = contentType;
//     _contentEncoding = contentEncoding;
//     _content = content;
//   }

//   public override string ToString()
//   {
//     var stringBuilder = new StringBuilder();
//     stringBuilder.Append($"{ProtocolVersion} {_statusCode} {_statusDescription}\r\n");
//     if (!string.IsNullOrWhiteSpace(_contentType))
//     {
//       stringBuilder.Append($"Content-Type: {_contentType}\r\n");
//     }
//     if (_hasContent)
//     {
//       stringBuilder.Append($"Content-Length: {_contentLength}\r\n");
//       if (_hasEncoding)
//         stringBuilder.Append($"Content-Encoding: {_contentEncoding}\r\n");
//       stringBuilder.Append("\r\n");
//       stringBuilder.Append(_content);
//       return stringBuilder.ToString();
//     }
//     stringBuilder.Append("\r\n");
//     return stringBuilder.ToString();
//   }

//   public byte[] GetBytes()
//   {
//     return Encoding.UTF8.GetBytes(ToString());
//   }
// }

using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
  private static readonly byte[] okHttpResponse = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n\r\n");
  private static readonly byte[] createdHttpResponse = Encoding.UTF8.GetBytes("HTTP/1.1 201 Created\r\n\r\n");
  private static readonly byte[] notFoundHttpResponse = Encoding.UTF8.GetBytes("HTTP/1.1 404 Not Found\r\n\r\n");
  private static string directory = string.Empty;
  private const int PORT = 4221;

  static async Task Main(string[] args)
  {

    directory = args.Length >= 2 ? args[1] : Environment.CurrentDirectory;

    var server = new TcpListener(IPAddress.Any, PORT);
    try
    {
      server.Start();
      Console.WriteLine("Server started on {0}:{1}", IPAddress.Any, PORT);
      while (true)
      {
        var socket = await server.AcceptSocketAsync();
        _ = Task.Run(() => HandleSocket(socket));
      }
    }
    finally
    {
      server.Stop();
    }
  }

  static async Task HandleSocket(Socket socket)
  {
    Console.WriteLine("New connection from {0}", socket.RemoteEndPoint);
    var buffer = new byte[1024];
    var n = await socket.ReceiveAsync(buffer, SocketFlags.None);
    var request = Encoding.UTF8.GetString(buffer, 0, n);
    var httpContext = HttpContext.Parse(request);

    Func<HttpContext, byte[]> handler = httpContext switch
    {
      { Method: "GET", Path: "/" } => HandleHome,
      { Method: "GET", Path: var path } when path.StartsWith("/files/") => HandleGetFile,
      { Method: "POST", Path: var path } when path.StartsWith("/files/") => HandlePostFile,
      { Method: "GET", Path: var path } when path.StartsWith("/user-agent") => HandleUserAgent,
      { Method: "GET", Path: var path } when path.StartsWith("/echo/") => HandleEcho,
      _ => HandleDefault
    };

    var response = handler(httpContext);
    Console.WriteLine("{0} requests {1} responded with {2}",
                      socket.RemoteEndPoint, httpContext.Path,
                      Encoding.UTF8.GetString(response));

    await socket.SendAsync(response, SocketFlags.None);
    socket.Shutdown(SocketShutdown.Both);
    socket.Close();
  }

  static byte[] HandleHome(HttpContext httpContext) => okHttpResponse;

  static byte[] HandleDefault(HttpContext httpContext) => notFoundHttpResponse;

  static byte[] HandleEcho(HttpContext httpContext)
  {
    var value = httpContext.Path.Split('/', StringSplitOptions.RemoveEmptyEntries)[1];
    var encodingHeader = string.Empty;
    byte[] responseBody;

    if (httpContext.Headers.TryGetValue("Accept-Encoding", out var headerValue) &&
        headerValue.Split(',', StringSplitOptions.TrimEntries).Contains("gzip"))
    {
      var bytes = Encoding.UTF8.GetBytes(value);
      using var memoryStream = new MemoryStream();
      using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
      {
        gzipStream.Write(bytes, 0, bytes.Length);
      }
      // Ensure the GZipStream is flushed and completed
      var compressed = memoryStream.ToArray();

      encodingHeader = "Content-Encoding: gzip\r\n";
      responseBody = compressed;
    }
    else
    {
      responseBody = Encoding.UTF8.GetBytes(value);
    }

    var responseHeader = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n{encodingHeader}Content-Length: {responseBody.Length}\r\n\r\n";
    var headerBytes = Encoding.UTF8.GetBytes(responseHeader);

    // Combine header and body into a single response
    return headerBytes.Concat(responseBody).ToArray();
  }


  static byte[] HandleUserAgent(HttpContext httpContext)
  {
    var userAgent = httpContext.Headers.TryGetValue("User-Agent", out var userAgentValue) ? userAgentValue : string.Empty;
    var response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {userAgent.Length}\r\n\r\n{userAgent}";
    return Encoding.UTF8.GetBytes(response);
  }

  static byte[] HandleGetFile(HttpContext httpContext)
  {
    var fileName = httpContext.Path.Split('/', StringSplitOptions.RemoveEmptyEntries)[1];
    var absolutePath = Path.GetFullPath($"{directory}{Path.DirectorySeparatorChar}{fileName}");
    if (!File.Exists(absolutePath))
    {
      return notFoundHttpResponse;
    }

    var content = File.ReadAllText(absolutePath);
    var response = $"HTTP/1.1 200 OK\r\nContent-Type: application/octet-stream\r\nContent-Length: {content.Length}\r\n\r\n{content}";
    return Encoding.UTF8.GetBytes(response);
  }

  static byte[] HandlePostFile(HttpContext httpContext)
  {
    var fileName = httpContext.Path.Split('/', StringSplitOptions.RemoveEmptyEntries)[1];
    var absolutePath = Path.GetFullPath($"{directory}{Path.DirectorySeparatorChar}{fileName}");
    if (!File.Exists(absolutePath))
    {
      File.WriteAllText(absolutePath, httpContext.Body);
      return createdHttpResponse;
    }

    return notFoundHttpResponse;
  }
}

class HttpContext
{
  public string Method { get; }
  public string Path { get; }
  public Dictionary<string, string> Headers { get; }
  public string Body { get; }

  private HttpContext(string method, string path, Dictionary<string, string> headers, string body)
  {
    Method = method;
    Path = path;
    Headers = headers;
    Body = body;
  }

  public static HttpContext Parse(string request)
  {
    var lines = request.Split("\r\n");
    var firstLine = lines[0].Split(' ');
    var method = firstLine[0];
    var path = firstLine[1];
    var headers = lines.Skip(1)
                        .TakeWhile(x => x != string.Empty)
                        .Select(x => x.Split(": ", 2))
                        .ToDictionary(x => x[0], x => x[1]);
    var body = request.Split("\r\n\r\n").Length > 1 ? request.Split("\r\n\r\n")[1] : string.Empty;
    return new HttpContext(method, path, headers, body);
  }
}