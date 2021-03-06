
using System;

using System.IO;

using System.Threading;

using System.Collections.Generic;

namespace Scads {

    public static class Chat {

        private static Mutex mutex = new Mutex();

        public static void Append (string chat, string username, string message, bool send2Discord = true) {

        	username = System.Net.WebUtility.UrlDecode(username);
        	message = System.Net.WebUtility.UrlDecode(message);

        	username = username.Replace('<', '(');
        	username = username.Replace('>', ')');

        	message = message.Replace('<', '(');
        	message = message.Replace('>', ')');

        	Analytics.LogChat(chat, message);

            mutex.WaitOne(); try {

                if (File.Exists("pages" + chat)) {

                    string[] chatFileLines = File.ReadAllLines("pages" + chat);

                    List<string> segmentOne = new List<string>();
                    List<string> chatLines = new List<string>();
                    List<string> segmentTwo = new List<string>();

                    bool onSegTwo = false;

                    for (int i = 0; i < chatFileLines.Length; ++i) {

                    	if (chatFileLines[i].Contains("<!msg>")) {

	                    	chatLines.Add(chatFileLines[i]);
                    		onSegTwo = true;

                    	} else {

                    		if (onSegTwo)
                    			segmentTwo.Add(chatFileLines[i]);
                    		else
                    			segmentOne.Add(chatFileLines[i]);
                    	}
                    }

                    if (chatLines.Count > 42) chatLines.RemoveAt(0);
                    chatLines.Add(
                    	"<!msg>[" + DateTime.Now.ToString() + "]-[" +
                    	username + "]: " + message + "<br>"
                    );
                    if (send2Discord) DiscordInterface.AddMessage(
                        new MessageNode(
                            chat.Split('/')[2],
                            // "**[" + username + "]:**" + message + "\n"
                            message
                        )
                    );

                    File.WriteAllText("pages" + chat, segmentOne[0]);

                    for (int i = 1; i < segmentOne.Count; ++i)
                    	File.AppendAllText("pages" + chat, "\n" + segmentOne[i]);

                    for (int i = 0; i < chatLines.Count; ++i)
                    	File.AppendAllText("pages" + chat, "\n" + chatLines[i]);

                    for (int i = 0; i < segmentTwo.Count; ++i)
                    	File.AppendAllText("pages" + chat, "\n" + segmentTwo[i]);

                }

            } finally { mutex.ReleaseMutex(); }
        }

        public static byte[] ReadChat (string chat) {

        	byte[] chatData = new byte[0];

        	mutex.WaitOne(); try {

                if (File.Exists("pages" + chat)) {

                	chatData = File.ReadAllBytes("pages" + chat);

                } else {

                	chatData = File.ReadAllBytes("pages/404.html");
                }

        	} finally { mutex.ReleaseMutex(); }

        	return chatData;
        }
    }
}
