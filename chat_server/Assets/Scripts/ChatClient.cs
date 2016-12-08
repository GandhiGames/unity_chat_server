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
		public Text chatOutput;
		public InputField chatInput;

		private static readonly string LOCAL_HOST = "127.0.0.1";
		private static readonly int DEFAULT_PORT = 2001;

		private bool isSocketReady;
		private TcpClient socket;
		private NetworkStream stream;
		private StreamWriter writer;
		private StreamReader reader;

		// Use this for initialization
		void Start ()
		{
		
		}
	
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
		

			try {
				socket = new TcpClient (host, port);

				stream = socket.GetStream();
				writer = new StreamWriter(stream);
				reader = new StreamReader(stream);

				isSocketReady = true;
			} catch (Exception e) {
				Debug.Log (e);			
				isSocketReady = false;
			}


		}

		public void Send()
		{
			if (chatInput.text != "") {
				OnOutgoingData (chatInput.text);
			}
		}

		private void OnIncomingData(string data)
		{
			chatOutput.text += data + '\n';
		}

		private void OnOutgoingData(string data)
		{
			if (isSocketReady) {
				writer.WriteLine (data);
				writer.Flush ();
			}
		}
	}
}