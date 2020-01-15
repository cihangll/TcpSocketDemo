using System;
using System.Windows.Forms;
using TcpSocketAsyncLib;

namespace TcpServerAsyncUI
{
	public partial class MainForm : Form
	{
		private readonly SocketServer _socketServer;
		public MainForm()
		{
			InitializeComponent();
			_socketServer = new SocketServer();
			_socketServer.RaiseClientConnectedEvent += HandleClientConnected;
			_socketServer.RaiseTextReceivedEvent += HandleTextReceived;
		}

		private void ConnectToServerEvent(object sender, EventArgs e)
		{
			_socketServer.StartListeningForIncomingConnection().ConfigureAwait(false);
		}

		private void SendAllEvent(object sender, EventArgs e)
		{
			_socketServer.SendToAll(textMessage.Text.Trim()).ConfigureAwait(false);
		}

		private void StopServerEvent(object sender, EventArgs e)
		{
			_socketServer.StopServer();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			_socketServer.StopServer();
		}

		void HandleClientConnected(object sender, ClientConnectedEventArgs e)
		{
			textConsole.AppendText($"{DateTime.Now} New Client Connected! {e.NewClient}{Environment.NewLine}");
		}

		void HandleTextReceived(object sender, TextReceivedEventArgs e)
		{
			textConsole.AppendText(Environment.NewLine);
			textConsole.AppendText($"{DateTime.Now} Received from {e.ClientWhoSentText}: {e.TextReceived}");
		}
	}
}
