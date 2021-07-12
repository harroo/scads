
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
			
			byte[] sendData = GetPageData(requestInfo.Split(' ')[1]);
			client.GetStream().Write(sendData, 0, sendData.Length);
			
			Console.WriteLine("Successfully Handled and Dropped: " + client.Client.RemoteEndPoint.ToString());
			
			client.GetStream().Close();
			client.Close();
		}
		
		public static byte[] GetPageData (string page) {
		
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
	}
}
