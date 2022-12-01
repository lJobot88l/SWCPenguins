using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System;
using static System.Text.Encoding;
using System.Net.Http;
using System.Net.Sockets;

namespace SWCPenguins
{
	static class Communication
	{
		static TcpClient Client;
		static NetworkStream Stream;
		static int port;
		static string host;
		static int roomid;
		public static void ConnectClient(string hostarg = "localhost", Gameserver server = Gameserver.GUI, int roomid = -1, string reservationcode = "")
		{
			port = (int)server;
			host = hostarg;
			Client = new TcpClient();
			Client.Connect(host, (int)server);
			Stream = Client.GetStream();

			StringWriterWithEncoding oString = new StringWriterWithEncoding(Encoding.UTF8);
			XmlWriter xmlWriter = XmlWriter.Create(oString);
			xmlWriter.WriteStartDocument(false);
			xmlWriter.WriteStartElement("join");

			if(roomid != -1) 
			{ xmlWriter.WriteAttributeString("roomId", roomid.ToString()); }

			if(!string.IsNullOrEmpty(reservationcode))
			{ xmlWriter.WriteAttributeString("reservationcode", reservationcode); }

			xmlWriter.WriteEndDocument();
			xmlWriter.Close();

			WriteString(oString.ToString().Insert("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\"?>".Length, "<protocol>"));

			byte[] buff = new byte[256];
			int rLen = Stream.Read(buff, 0, buff.Length);
			string oXml = "";
			for(int i = 0; i < rLen; i++)
			{
				oXml += (char)buff[i];
			}
			Console.WriteLine(oXml);
			XmlDocument xmlDoc = new XmlDocument();
			oXml = oXml.Insert(oXml.Length, "</response>").Replace("<protocol>", "<response>");
			xmlDoc.LoadXml(oXml);
			
			GameSession.currentsession = new GameSession(xmlDoc.ChildNodes[1].Attributes[0].Value, xmlDoc.ChildNodes[1].ChildNodes[0].Attributes[1].Value); // I hate whatever the fuck this is
		}
		static void WriteString(string s)
		{
			byte[] data = UTF8.GetBytes(s);
			Stream.Write(data, 0, data.Length);
			Console.WriteLine(s);
		}
	}
	enum Gameserver
	{
		GUI = 13050,
		TestServer = 13051
	}

	public class StringWriterWithEncoding : StringWriter
	{
		public override Encoding Encoding
		{
			get
			{
				return MyEncoding;
			}
		}

		private Encoding myEncoding;
		public Encoding MyEncoding
		{
			get
			{
				return myEncoding;
			}
			set
			{
				myEncoding = value;
			}
		}

		public StringWriterWithEncoding(Encoding enc) : base()
		{
			myEncoding = enc;
		}
	}
}