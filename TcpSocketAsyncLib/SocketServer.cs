using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpSocketAsyncLib
{
	public class SocketServer
	{
		public TcpListener tcpListener;
		public List<TcpClient> tcpClients = new List<TcpClient>();
		public EventHandler<ClientConnectedEventArgs> RaiseClientConnectedEvent;
		public EventHandler<TextReceivedEventArgs> RaiseTextReceivedEvent;

		public bool KeepRunning { get; set; }
		public async Task StartListeningForIncomingConnection(IPAddress ipAddress = null, int port = 23000)
		{
			if (ipAddress == null)
			{
				ipAddress = IPAddress.Any;
			}

			if (port < 0 || port > 65535)
			{
				port = 23000;
			}

			var ipEndPoint = new IPEndPoint(ipAddress, port);
			Debug.WriteLine($"Server Endpoint is {ipEndPoint.ToString()}");
			tcpListener = new TcpListener(ipEndPoint);

			try
			{
				tcpListener.Start();

				KeepRunning = true;
				while (KeepRunning)
				{
					var tcpClient = await tcpListener.AcceptTcpClientAsync();
					tcpClients.Add(tcpClient);
					Debug.WriteLine($"Client Connected Succesfully.Total Client Count: {tcpClients.Count} Endpoint is {tcpClient.Client.RemoteEndPoint.ToString()}");

					TakeCareOfTcpClient(tcpClient);

					OnRaiseClientConnectedEvent(new ClientConnectedEventArgs(tcpClient.Client.RemoteEndPoint.ToString()));
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			finally
			{
				tcpListener.Stop();
			}
		}

		public void StopServer()
		{
			try
			{
				if (tcpListener != null)
				{
					tcpListener.Stop();
				}

				tcpClients.ForEach(tcpClient =>
				{
					tcpClient.Close();
				});

				tcpClients.Clear();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		private async Task TakeCareOfTcpClient(TcpClient tcpClient)
		{
			try
			{
				var networkStream = tcpClient.GetStream();
				var streamReader = new StreamReader(networkStream);

				while (KeepRunning)
				{
					char[] buffer = new char[64];
					var byteLength = await streamReader.ReadAsync(buffer, 0, buffer.Length);
					Debug.WriteLine(" Returned: " + byteLength);

					if (byteLength == 0)
					{
						RemoveClient(tcpClient);
						Debug.WriteLine("Socket disconnected.");
						break;
					}

					var receivedText = new string(buffer);
					Debug.WriteLine($"Received: {receivedText} ");

					OnRaiseTextReceivedEvent(new TextReceivedEventArgs(tcpClient.Client.RemoteEndPoint.ToString(), receivedText));
				}
			}
			catch (Exception ex)
			{
				RemoveClient(tcpClient);
				Debug.WriteLine(ex.ToString());
			}
		}

		private void RemoveClient(TcpClient tcpClient)
		{
			if (tcpClients.Contains(tcpClient))
			{
				tcpClients.Remove(tcpClient);
				Debug.WriteLine($" {tcpClient.Client.RemoteEndPoint} Client removed.Total Client Count is {tcpClients.Count}");
			}
		}

		public async Task SendToAll(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				return;
			}

			try
			{
				byte[] buffer = Encoding.UTF8.GetBytes(message);
				foreach (var tcpClient in tcpClients)
				{
					await tcpClient.GetStream().WriteAsync(buffer, 0, buffer.Length);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		protected virtual void OnRaiseClientConnectedEvent(ClientConnectedEventArgs e)
		{
			RaiseClientConnectedEvent?.Invoke(this, e);
		}

		protected virtual void OnRaiseTextReceivedEvent(TextReceivedEventArgs e)
		{
			RaiseTextReceivedEvent?.Invoke(this, e);
		}
	}
}
