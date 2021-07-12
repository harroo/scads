
using System;

using System.IO;

namespace Scads {

	public static class Program {
		
		public static void Main (string[] args) {
			
			Console.WriteLine("Starting up Scads!");
			
			WebServer.Start();
			
			while (true) {
			
				if (File.Exists("stop")) {
					
					File.Delete("stop");
					break;
				}
				
				if (Console.ReadKey().KeyChar == 'q') break;
			}
			
			Console.WriteLine("Closing Down!");
		}
	}
}
