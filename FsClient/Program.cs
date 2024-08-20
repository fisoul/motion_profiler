// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;

var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
serverSocket.Bind(new IPEndPoint(IPAddress.Any, 10086));
serverSocket.Listen(10);

while (true)
{
    var client = serverSocket.Accept();
    Console.WriteLine(client.RemoteEndPoint);
}
// Console.WriteLine("Start Async Connection");
// await clientSocket.ConnectAsync(new IPEndPoint(IPAddress.Loopback, 10086));

