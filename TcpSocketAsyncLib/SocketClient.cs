using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpSocketAsyncLib
{
	public class SocketClient
	{
		private readonly TcpClient _tcpClient = new TcpClient();
		private readonly IPAddress _serverIpAddress;
		private readonly int _serverPort;
		public EventHandler<TextReceivedEventArgs> RaiseTextReceivedEvent;

		public SocketClient(IPEndPoint serverIpEndPoint)
		{
			_serverIpAddress = serverIpEndPoint.Address;
			_serverPort = serverIpEndPoint.Port;
		}

		public SocketClient(string serverIpAddressStr, string serverPortStr)
		{
			ReturnIpAddress(serverIpAddressStr, out IPAddress serverIpAddress);
			ReturnPort(serverPortStr, out int serverPort);
			_serverIpAddress = serverIpAddress;
			_serverPort = serverPort;
		}

		private void ReturnIpAddress(string ipAddressStr, out IPAddress ipAddress)
		{
			if (!IPAddress.TryParse(ipAddressStr, out ipAddress))
			{
				throw new InvalidCastException("Invalid server ip supplied!");
			}
		}

		private void ReturnPort(string portStr, out int port)
		{
			if (!int.TryParse(portStr, out port) || port < 0 || port > 65535)
			{
				throw new Exception("Server port number is invalid.");
			}
		}

		public void CloseAndDisconnect()
		{
			if (_tcpClient != null && _tcpClient.Connected)
			{
				_tcpClient.Close();
			}
		}

		public async Task SendToServer(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return;
			}

			if (_tcpClient != null && _tcpClient.Connected)
			{
				var streamWriter = new StreamWriter(_tcpClient.GetStream(), Encoding.UTF8);
				streamWriter.AutoFlush = true;

				await streamWriter.WriteAsync(input);
				Console.WriteLine("Data sending...");
			}
		}

		public async Task ConnectToServer()
		{
			try
			{
				await _tcpClient.ConnectAsync(_serverIpAddress, _serverPort);
				Console.WriteLine("Connected to the server {0}:{1}", _serverIpAddress, _serverPort);

				await ReadDataAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public async Task ReadDataAsync()
		{
			try
			{
				var streamReader = new StreamReader(_tcpClient.GetStream(), Encoding.UTF8);

				while (true)
				{
					var buffer = new char[64];
					int readByteCount = await streamReader.ReadAsync(buffer, 0, buffer.Length);
					if (readByteCount <= 0)
					{
						Console.WriteLine("Disconnected from server.");
						_tcpClient.Close();
						break;
					}
					Console.WriteLine($"Received Bytes {readByteCount}, Message: {new string(buffer)}");

					OnRaiseTextReceivedEvent(new TextReceivedEventArgs(_tcpClient.Client.RemoteEndPoint.ToString(), new string(buffer)));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		private void OnRaiseTextReceivedEvent(TextReceivedEventArgs textReceivedEventArgs)
		{
			RaiseTextReceivedEvent?.Invoke(this, textReceivedEventArgs);
		}

		public IPAddress ResolveHostNameToIPAddress(string hostNameOrAddress)
		{
			try
			{
				var ipAddressList = Dns.GetHostAddresses(hostNameOrAddress);
				foreach (var ipAddress in ipAddressList)
				{
					if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
					{
						return ipAddress;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			return default;
		}
	}
}
