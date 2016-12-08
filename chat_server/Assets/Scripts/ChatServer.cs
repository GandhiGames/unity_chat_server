using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Chat
{
	public class ChatServer : MonoBehaviour
	{
		public int port = 2001;

		private List<ServerClient> serverClients;
		private List<ServerClient> disconnectedClients;
		private TcpListener listener;
		private bool isStarted;

		void Start ()
		{
			serverClients = new List<ServerClient> ();
			disconnectedClients = new List<ServerClient> ();

			try {
				listener = new TcpListener (IPAddress.Any, port); 

				listener.Start ();

				StartListening ();

				isStarted = true;
			} catch (Exception e) {
				print (e);
				isStarted = false;
			}

			if (isStarted) {
				Debug.Log ("Server started on port " + port);
			}

		}

		void Update ()
		{
			if (isStarted) {

				foreach (var client in serverClients) {

					if (IsConnected (client.tcp)) {

						NetworkStream stream = client.tcp.GetStream ();

						if (stream.DataAvailable) {
							StreamReader reader = new StreamReader (stream, true);

							string data = reader.ReadLine ();

							if (data != null) {
								OnInComingData (client, data);
							}
						}

					} else { // client disconnected
						client.tcp.Close ();
						disconnectedClients.Add (client);
					}

				}
			}
		}

		private void StartListening ()
		{
			listener.BeginAcceptTcpClient (AcceptTCPClient, listener);
		}

		private void AcceptTCPClient (IAsyncResult ar)
		{
			var lis = (TcpListener)ar.AsyncState;

			serverClients.Add (new ServerClient (lis.EndAcceptTcpClient (ar)));

			StartListening ();
		}

		private bool IsConnected (TcpClient tcp)
		{
			try {
				if (tcp != null && tcp.Client != null && tcp.Client.Connected) {
					if (tcp.Client.Poll (0, SelectMode.SelectRead)) {
						return !(tcp.Client.Receive (new byte[1], SocketFlags.Peek) == 0);
					}

					return true;
				}

			} catch (Exception) {
				return false;
			}

			return false;
		}

		private void OnInComingData (ServerClient client, string data)
		{
			Console.WriteLine ("client={0}, data={1}", client.name, data);
			
		}
	}

	public class ServerClient
	{
		public TcpClient tcp;
		public string name;

		public ServerClient (TcpClient tcp) : this (tcp, "Client")
		{
		}

		public ServerClient (TcpClient tcp, string name)
		{
			this.tcp = tcp;
			this.name = name;
		}
	}
}