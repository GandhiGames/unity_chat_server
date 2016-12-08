using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

namespace Chat
{
	public class ChatClient : MonoBehaviour
	{
		public InputField hostInput;
		public InputField portInput;
		public InputField nameInput;
		public Text chatOutput;
		public InputField chatInput;

		private static readonly string LOCAL_HOST = "127.0.0.1";
		private static readonly int DEFAULT_PORT = 2001;
		private static readonly string DEFAULT_NAME = "Guest";

		private bool isSocketReady;
		private TcpClient socket;
		private NetworkStream stream;
		private StreamWriter writer;
		private StreamReader reader;


		// Update is called once per frame
		void Update ()
		{
			if (isSocketReady && stream.DataAvailable) {

				string data = reader.ReadLine ();

				if (data != null) {

					OnIncomingData (data);
				}
			}
		}

		public void Connect ()
		{
			if (isSocketReady) {
				return;
			}

			string host = hostInput.text != "" ? hostInput.text : LOCAL_HOST;

			int portConverted;
			int port = int.TryParse (portInput.text, out portConverted) ? portConverted : DEFAULT_PORT;
			string userName = nameInput.text != "" ? nameInput.text : DEFAULT_NAME; 


			try {
				socket = new TcpClient (host, port);

				stream = socket.GetStream();
				writer = new StreamWriter(stream);
				reader = new StreamReader(stream);

				isSocketReady = true;

				OnOutgoingData(new Message(MessageType.Name, userName));
			} catch (Exception e) {
				Debug.Log (e);			
				isSocketReady = false;
			}


		}

		public void Send()
		{
			if (chatInput.text != "") {
				OnOutgoingData (new Message(MessageType.Data, chatInput.text));
			}
		}

		private void OnIncomingData(string data)
		{
			chatOutput.text += data + '\n';
		}

		private void OnOutgoingData(Message data)
		{
			if (isSocketReady) {
				writer.WriteLine (data.Serialize());
				writer.Flush ();
			}
		}

		private void CloseSocket()
		{
			if (isSocketReady) {
				writer.Close ();
				reader.Close ();
				stream.Close ();
				socket.Close ();
				isSocketReady = false;
			}
		}

		void OnDisable()
		{
			CloseSocket ();
		}

		void OnApplicationQuit()
		{
			CloseSocket ();
		}

	}
}