
using System;

using System.IO;

using System.Threading;

namespace Scads {

    public static class Chat {

        private static Mutex mutex = new Mutex();

        public static void Append (string chat, string username, string message) {

            mutex.WaitOne(); try {

                if (File.Exists("pages/chats/" + chat)) {

                    string chatFile = File.ReadAllText("pages/chats/" + chat);

                    '
                }

            } finally { mutex.ReleaseMutex(); }
        }
    }
}
