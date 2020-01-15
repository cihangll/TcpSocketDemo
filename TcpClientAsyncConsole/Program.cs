using System;
using System.Net;
using TcpSocketAsyncLib;

namespace TcpClientAsyncConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Server IP Address:");

			IPAddress serverIpAddress;
			while (!IPAddress.TryParse(Console.ReadLine().Trim(), out serverIpAddress))
			{
				Console.WriteLine("Server Ip Address Is Invalid. Server IP Address:");
			}

			Console.WriteLine("Server Port Number:");

			int serverPortNumber;
			while (!int.TryParse(Console.ReadLine().Trim(), out serverPortNumber) || serverPortNumber < 0 || serverPortNumber > 65535)
			{
				Console.WriteLine("Server Port Number Is Invalid. Server Port Number:");
			}

			var serverIpEndPoint = new IPEndPoint(serverIpAddress, serverPortNumber);
			Console.WriteLine("Connection Starting... EndPoint: {0}", serverIpEndPoint.ToString());

			var socketClient = new SocketClient(serverIpEndPoint);
			socketClient.RaiseTextReceivedEvent += HandleTextReceived;
			socketClient.ConnectToServer().ConfigureAwait(false);

			while (true)
			{
				var input = Console.ReadLine().Trim();
				if (input == "<EXIT>")
				{
					socketClient.CloseAndDisconnect();
					break;
				}

				socketClient.SendToServer(input).ConfigureAwait(false);
			}

			Console.ReadKey();
		}

		private static void HandleTextReceived(object sender, TextReceivedEventArgs textReceivedEventArgs)
		{
			Console.WriteLine(Environment.NewLine);
			Console.WriteLine($"{DateTime.Now} - Received: {textReceivedEventArgs.ClientWhoSentText}, {textReceivedEventArgs.TextReceived}");
		}
	}
}
