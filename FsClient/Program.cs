// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;

var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
Console.WriteLine("Start Async Connection");
await clientSocket.ConnectAsync(new IPEndPoint(IPAddress.Loopback, 10086));


Console.WriteLine("Hello, World!");