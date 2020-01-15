using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpClient1
{
	class Program
	{
		static void Main(string[] args)
		{
			Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			try
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

				IPEndPoint serverIpEndPoint = new IPEndPoint(serverIpAddress, serverPortNumber);
				Console.WriteLine("Connection Starting... EndPoint: {0}", serverIpEndPoint.ToString());
				clientSocket.Connect(serverIpEndPoint);

				Console.WriteLine("Connected to the Server");

				while (true)
				{
					string inputCommand = Console.ReadLine();

					if (inputCommand == "<EXIT>")
					{
						break;
					}

					var buffSend = Encoding.UTF8.GetBytes(inputCommand);
					clientSocket.Send(buffSend);

					var buffReceived = new byte[128];
					int nRecv = clientSocket.Receive(buffReceived);

					Console.WriteLine("Data Received: {0}", Encoding.UTF8.GetString(buffReceived, 0, nRecv));
				}
			}
			catch (Exception ex)
			{
				Console.Write(ex.ToString());
			}
			finally
			{
				if (clientSocket != null)
				{
					if (clientSocket.Connected)
					{
						clientSocket.Shutdown(SocketShutdown.Both);
					}
					clientSocket.Close();
					clientSocket.Dispose();
				}
			}

			Console.WriteLine("Press a key to exit...");
			Console.ReadKey();
		}
	}
}
