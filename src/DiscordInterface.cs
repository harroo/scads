
using System;
using System.IO;

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
		
		public static void Initialize () {

			shouldStop = false;

			Bot.MainAsync().ConfigureAwait(false);

			running = true;
		}
		
		private static bool shouldStop = false; running = false;
		
		private static DiscordClient discord;
		
		private static async Task MainAsync () {
		
			Console.WriteLine("Starting DiscordInterface...");
		
			DiscordConfiguration configuration = new DiscordConfiguration {
			
				Token = File.ReadAllText("Token.text"),
				TokenType = TokenType.Bot,
				
				AutoReconnect = true;
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
            }

            await discord.DisconnectAsync();

            running = false;

            return;
		}

        public static Task OnReady (ReadyEventArgs e) {

            Log("DiscordInterface online!");

            return Task.CompletedTask;
        }

        public static Task OnGuildAvailable (ReadyEventArgs e) {

            Log("");

            return Task.CompletedTask;
        }

        public static Task OnClientError (ReadyEventArgs e) {

            Log("im in discord now.");

            return Task.CompletedTask;
        }

        public static Task OnMessageCreated (ReadyEventArgs e) {

            Log("im in discord now.");

            return Task.CompletedTask;
        }
	}
}
