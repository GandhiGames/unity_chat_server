using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Chat
{
	public enum MessageType
	{
		Name, 
		Data
	}

	public class Message 
	{
		public MessageType type { get; private set; }
		public object data { get; private set; }

		private static readonly string ERROR = "Error";

		private bool isProcessable = true;

		public Message(MessageType type, object data)
		{
			this.type = type;
			this.data = data;
		}

		public static Message FromSerialized(string data)
		{
			string[] split = data.SplitMessage();

			MessageType msgType;

			try
			{
				msgType = (MessageType)Enum.Parse(typeof(MessageType), split[0]);
			}
			catch(ArgumentException e) {
				Debug.Log (e);

				return new Message (false);
			}

			return new Message (msgType, split[1]);
		}

		private Message(bool canBeProcessed)
		{
			isProcessable = canBeProcessed;
		}

		public string Serialize ()
		{
			if (!CanBeProcessed ()) {
				return ERROR;
			}
			
			return string.Format ("{0}|{1}", type.ToString(), data.ToString());
		}

		public bool CanBeProcessed()
		{
			return isProcessable;
		}
	}
}