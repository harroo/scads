
using System;

using System.IO;

using System.Threading;

namespace Scads {

	public static class Program {
	
		private static int timer;
		
		public static void Main (string[] args) {
			
			Console.WriteLine("Starting up Scads!");
			
			WebServer.Start();
			DiscordInterface.Start();
			
			while (true) {
			
				if (File.Exists("stop")) {
					
					File.Delete("stop");
					break;
				}
				
				Thread.Sleep(1000);
				
				timer++; if (timer > 1800000) { timer = 0;
				
					DiscordInterface.Restart();
				}
			}
			
			Console.WriteLine("Closing Down!");
			
			DiscordInterface.AwaitStop();
			
			Console.WriteLine("Shutdown completed.");
		}
	}
}
