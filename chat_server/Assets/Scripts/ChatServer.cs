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

					if (client.tcp.IsConnected ()) {

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

				for (int i = 0; i > disconnectedClients.Count; i++) {

					string name = disconnectedClients [i].name;
					serverClients.Remove (disconnectedClients [i]);
					disconnectedClients.RemoveAt (i);

					Broadcast (name + " has disconnected", serverClients);

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

		private void OnInComingData (ServerClient client, string data)
		{
			Message incoming = Message.FromSerialized (data);

			if (!incoming.CanBeProcessed()) {
				return;
			}

			if (incoming.type == MessageType.Name) {
				serverClients [serverClients.Count - 1].name = incoming.data.ToString();

				Broadcast (serverClients [serverClients.Count - 1].name + " has connected", serverClients);
			} else if(incoming.type == MessageType.Data) {

				Broadcast (incoming.data.ToString(), serverClients);
			}
		}

		private void Broadcast (string data, List<ServerClient> clients)
		{
			foreach (var client in clients) {
				try {
					StreamWriter writer = new StreamWriter(client.tcp.GetStream());
					writer.WriteLine(data);
					writer.Flush();
				} catch (Exception e) {
					Debug.Log (e);
				}
			}
		}
	}

	public class ServerClient
	{
		public TcpClient tcp;
		public string name;

		public ServerClient (TcpClient tcp) : this (tcp, "Guest")
		{
		}

		public ServerClient (TcpClient tcp, string name)
		{
			this.tcp = tcp;
			this.name = name;
		}
	}
}