
using System;

using System.IO;

using System.Threading;

namespace Scads {

	public static class Analytics {

		private static void CheckDir () {

			if (!Directory.Exists("logs"))
				Directory.CreateDirectory("logs");

			if (!Directory.Exists("logs/users"))
				Directory.CreateDirectory("logs/users");

			if (!Directory.Exists("logs/chats"))
				Directory.CreateDirectory("logs/chats");
		}

		private static Mutex mutex = new Mutex();

		public static void LogClient (string user, string content) {

			mutex.WaitOne(); try {

				CheckDir();
				string logPath = "logs/users/" + user;

				content = ":::" + DateTime.Now.ToString() + ":::\n" + content;

				if (File.Exists(logPath))
					File.AppendAllText(logPath, "\n" + content);
				else
					File.WriteAllText(logPath, content);

			} finally { mutex.ReleaseMutex(); }
		}

		public static void LogChat (string chat, string message) {

			mutex.WaitOne(); try {

				CheckDir();
				string logPath = "logs/" + chat;

				if (File.Exists(logPath))
					File.AppendAllText(logPath, "\n" + message);
				else
					File.WriteAllText(logPath, message);

			} finally { mutex.ReleaseMutex(); }
		}
	}
}
