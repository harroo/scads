
using System;

using System.IO;

using System.Text;

using System.Threading;

using System.Net;
using System.Net.Sockets;

namespace Scads {

	public static class WebServer {

		public static void Start () {

			TcpListener listener = new TcpListener(IPAddress.Any, 80);

			try {

				listener.Start();

				Console.WriteLine("WebServer Online!");

			} catch (Exception ex) {

				Console.WriteLine(ex.Message);
				Start();
			}

			while (true) {

				TcpClient client = listener.AcceptTcpClient();
				new Thread(()=>HandleClient(client)).Start();
			}
		}

		public static void HandleClient (TcpClient client) {

			Console.WriteLine("Connection Received: " + client.Client.RemoteEndPoint.ToString());

			byte[] recvData = new byte[4096];
			client.GetStream().Read(recvData, 0, recvData.Length);
			string requestInfo = Encoding.ASCII.GetString(recvData);

			Console.WriteLine("Read: \n" + requestInfo);

			if (requestInfo.Contains(' ')) {

				byte[] sendData = GetPageData(requestInfo.Split(' ')[1]);
				client.GetStream().Write(sendData, 0, sendData.Length);
			}

			Console.WriteLine("Successfully Handled and Dropped: " + client.Client.RemoteEndPoint.ToString());

			client.GetStream().Close();
			client.Close();
		}

		public static byte[] GetPageData (string page) {

			if (page.Contains('?') && page.Contains('&')) {

				string[] values = page.Split('?');

				page = values[0];

				bool chat = false;
				if (page == "chat.html")
					chat = ProcessChatArgs(values[1]);

				if (chat) return Chat.ReadChat();
			}

			Console.WriteLine("Request for: " + page);

			page = page == "/" ? "/home.html" : page;

			string path = "pages" + page;

			if (File.Exists(path)) {

				Console.WriteLine("Found!");

				return File.ReadAllBytes(path);

			} else {

				Console.WriteLine("Not Found!");

				return File.ReadAllBytes("pages/404.html");
			}
		}

		public static bool ProcessChatArgs (string chatArgs) {

			Console.WriteLine("Chat Args Read: " + chatArgs);

			if (!chatArgs.Contains('&')) return false;
			if (!chatArgs.Contains('=')) return false;

			string[] args = chatArgs.Split('&');
			string[] arg1 = args.Split('=');
			string[] arg2 = args.Split('=');

			string username, message;

			if (arg1[0] == "username") username = arg1[1]; else return false;
			if (arg2[0] == "message") username = arg2[1]; else return false;

			Chat.Append(username, message);
			
			return true;
		}
	}
}
