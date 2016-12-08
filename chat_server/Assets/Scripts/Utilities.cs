using System;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Utilities
{
	public static bool IsConnected (this TcpClient tcp)
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

	public static string[] SplitMessage(this string data)
	{
		return data.Split ('|');
	}
}
