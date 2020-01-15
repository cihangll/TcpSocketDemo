using System;

namespace TcpSocketAsyncLib
{
	public class ClientConnectedEventArgs : EventArgs
	{
		public string NewClient { get; private set; }
		public ClientConnectedEventArgs(string newClient)
		{
			NewClient = newClient;
		}
	}

	public class TextReceivedEventArgs : EventArgs
	{
		public string ClientWhoSentText { get; private set; }
		public string TextReceived { get; set; }
		public TextReceivedEventArgs(string clientWhoSentText, string textReceived)
		{
			ClientWhoSentText = clientWhoSentText;
			TextReceived = textReceived;
		}
	}
}
