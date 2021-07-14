
using System;
using System.IO;
using System.Collections.Generic;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using System.Threading.Tasks;
using System.Threading;

namespace Scads {

	public static class DiscordInterface {

		public static void Start () {

			new Thread(()=>Initialize()).Start();
		}
		public static void Stop () {

			shouldStop = true;
		}
		public static void AwaitStop () {

			shouldStop = true;
			while (running)
				Thread.Sleep(16);
		}

		public static void Restart () {

			Console.WriteLine("Restarting DiscordInterface...");

			AwaitStop(); Start();
		}

		public static void Initialize () {

			shouldStop = false;

			DiscordInterface.MainAsync().ConfigureAwait(false);

			running = true;
		}

		private static bool shouldStop = false, running = false;

		private static DiscordClient discord;

		private static async Task MainAsync () {

			Console.WriteLine("Starting DiscordInterface...");

			DiscordConfiguration configuration = new DiscordConfiguration {

				Token = File.ReadAllText("Token.text"),
				TokenType = TokenType.Bot,

				AutoReconnect = true
			};

			discord = new DiscordClient(configuration);

            discord.Ready += OnReady;
            discord.GuildAvailable += OnGuildAvailable;
            discord.ClientErrored += OnClientError;
            discord.MessageCreated += OnMessageCreated;

            await discord.ConnectAsync();

            await Task.Delay(512);

            await discord.UpdateStatusAsync(

                new DiscordGame("Interfacing"),

                UserStatus.Online,

                null
            );

            while (!shouldStop) {

                await Task.Delay(512);

				while (MessageCount() != 0) {

					MessageNode mNode = PopMessage();

					if (channels.ContainsKey(mNode.targetChannel)) {

						await channels[mNode.targetChannel].SendMessageAsync(mNode.messageContent);

					} else Console.WriteLine("Missed message to: " + mNode.targetChannel);
				}
            }

            await discord.DisconnectAsync();

            running = false;

            return;
		}

		public static Dictionary<string, DiscordChannel> channels
			= new Dictionary<string, DiscordChannel>();

        public static Task OnReady (ReadyEventArgs e) {

            Console.WriteLine("DiscordInterface online!");

            return Task.CompletedTask;
        }

        public static Task OnGuildAvailable (GuildCreateEventArgs e) {

			foreach (var channel in e.Guild.Channels) {

				if (channel.Type == ChannelType.Text) {

					Console.WriteLine("Channel Online: " + e.Guild.Name + "::" + channel.Name);

					if (channels.ContainsKey(channel.Name))
						channels[channel.Name] = channel;
					else
						channels.Add(channel.Name, channel);
				}
			}

            return Task.CompletedTask;
        }

        public static Task OnClientError (ClientErrorEventArgs e) {

            return Task.CompletedTask;
        }

        public static Task OnMessageCreated (MessageCreateEventArgs e) {

			if (e.Message.Author.IsCurrent) return Task.CompletedTask;

			Chat.Append(
				"/chats/" + e.Message.Channel.Name,
				e.Message.Author.Username,
				e.Message.Content, false
			);

            return Task.CompletedTask;
        }

		private static Mutex mutex = new Mutex();
		private static List<MessageNode> messageQueue = new List<MessageNode>();

		public static void AddMessage (MessageNode message) { mutex.WaitOne(); try {

			messageQueue.Add(message);

		} finally { mutex.ReleaseMutex(); } }

		public static MessageNode PopMessage () { MessageNode rtn; mutex.WaitOne(); try {

			rtn = messageQueue[0]; messageQueue.RemoveAt(0);

		} finally { mutex.ReleaseMutex(); } return rtn; }

		public static int MessageCount () { int rtn = 0; mutex.WaitOne(); try {

			rtn = messageQueue.Count;

		} finally { mutex.ReleaseMutex(); } return rtn; }
	}

	public struct MessageNode {

		public string targetChannel;
		public string messageContent;

		public MessageNode (string a, string b) {

			targetChannel = a;
			messageContent = b;
		}
	}
}
