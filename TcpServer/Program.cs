using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpServer1
{
	class Program
	{
		static void Main(string[] args)
		{
			var listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			var ipEndPoint = new IPEndPoint(IPAddress.Any, 23000);

			try
			{
				listenerSocket.Bind(ipEndPoint);
				listenerSocket.Listen(5);

				Console.WriteLine("Server is up. Listening on {0}...", ipEndPoint.ToString());

				var connectionSocket = listenerSocket.Accept();
				Console.WriteLine($"Client connected. {connectionSocket.ToString()} - IP End Point: {connectionSocket.RemoteEndPoint.ToString()}");

				while (true)
				{
					var buffer = new byte[128];
					int numberOfReceivedBytes = connectionSocket.Receive(buffer);
					Console.WriteLine("Number of received bytes: {0}", numberOfReceivedBytes);

					var receivedString = Encoding.UTF8.GetString(buffer, 0, numberOfReceivedBytes);
					Console.WriteLine("Data send by client is {0}", receivedString);

					//Send back same data
					connectionSocket.Send(buffer);

					if (receivedString == "x")
					{
						break;
					}

					Array.Clear(buffer, 0, buffer.Length);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
