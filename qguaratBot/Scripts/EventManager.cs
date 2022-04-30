using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;

namespace qguaratBot
{
    public static class EventManager
    {
        public static Task HandleEvents()
        {
            Bot.ConnectionManager.discordClient.Ready += OnReady;

            Bot.ConnectionManager.discordClient.MessageCreated += OnMessageReceived;

            Bot.ConnectionManager.lavalinkNode.PlaybackStarted += OnTrackStarted;

            Bot.ConnectionManager.lavalinkNode.PlaybackFinished += OnTrackFinished;

            Bot.TrackAddedToQueue += OnTrackAddedToQueue;
            
            return Task.CompletedTask;
        }

        private static async void OnTrackAddedToQueue(object? sender, TrackEventArgs e)
        {
            if (!Bot.isPlaying)
            {
                Bot.PlayNextTrack();
            }
            else
            {
                Console.Log(Console.LogLevel.INFO, $"Track [{e.Track.Title}] is added to the queue!");
                await Bot.ConnectionManager.commandContext.RespondAsync($"Track [{e.Track.Title}] is added to the queue!");
            }
        }

        private static Task OnTrackStarted(LavalinkGuildConnection sender, TrackStartEventArgs e)
        {
            Bot.isPlaying = true;

            Console.Log(Console.LogLevel.INFO, $"Now playing [{e.Track.Title}]");
            Bot.ConnectionManager.commandContext.RespondAsync($"Now playing [{e.Track.Title}]!");

            return Task.CompletedTask;
        }

        private static Task OnTrackFinished(LavalinkGuildConnection sender, TrackFinishEventArgs e)
        {
            Bot.isPlaying = false;
            
            if (Bot.trackSkipped)
            {
                Console.Log(Console.LogLevel.INFO, $"Track [{e.Track.Title}] is skipped!");
            }
            else
            {
                Console.Log(Console.LogLevel.INFO, $"Track [{e.Track.Title}] has finished!");
                Bot.PlayNextTrack();
            }

            return Task.CompletedTask;
        }

        private static Task OnMessageReceived(DiscordClient client, MessageCreateEventArgs e)
        {
            var cnext = client.GetCommandsNext();
            var msg = e.Message;

            var cmdStart = msg.GetStringPrefixLength(ConfigManager.Config.Prefix);
            if (cmdStart == -1) return Task.CompletedTask;

            var prefix = msg.Content.Substring(0, cmdStart);
            var cmdString = msg.Content.Substring(cmdStart);

            var command = cnext.FindCommand(cmdString, out var args);
            if (command == null) return Task.CompletedTask;

            var ctx = cnext.CreateContext(msg, prefix, command, args);
            Task.Run(async () => await cnext.ExecuteCommandAsync(ctx));


            Console.Log(Console.LogLevel.INFO, $"*{ctx.User.Username}* executed [{ctx.Command.Name}]");
            return Task.CompletedTask;
        }

        private static async Task OnReady(DiscordClient sender, ReadyEventArgs e)
        {
            SetBotStatus();

            Console.Log(Console.LogLevel.INFO, "Bot is ready -----------");
        }

        private static void SetBotStatus()
        {
            Bot.ConnectionManager.discordClient.UpdateStatusAsync(new DiscordActivity
            {
                Name = "prefix *" + ConfigManager.Config.Prefix + "*",
                ActivityType = ActivityType.ListeningTo
            });
        }
    }
}