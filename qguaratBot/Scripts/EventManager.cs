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
            Bot.socketConnection.discordClient.Ready += OnReady;

            Bot.socketConnection.discordClient.MessageCreated += OnMessageReceived;

            Bot.socketConnection.lavalinkNode.PlaybackFinished += OnTrackFinished;

            Bot.TrackAddedToQueue += OnTrackAddedToQueue;
            
            return Task.CompletedTask;
        }

        private static async void OnTrackAddedToQueue(object? sender, TrackEventArgs e)
        {
            if (!Bot.IsPlaying)
            {
                PlayNextTrack();
            }
            else
            {
                System.Console.WriteLine($"[{DateTime.Now}]\tTrack [{e.Track.Title}] is added to the queue!");
                await Bot.socketConnection.commandContext.RespondAsync($"Track [{e.Track.Title}] is added to the queue!");
            }
        }

        private static Task OnTrackFinished(LavalinkGuildConnection sender, TrackFinishEventArgs e)
        {
            Bot.IsPlaying = false;
            System.Console.WriteLine($"[{DateTime.Now}]\tTrack [{e.Track.Title}] has finished!");

            PlayNextTrack();

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


            System.Console.WriteLine($"[{DateTime.Now}]\t*{ctx.User.Username}* executed [{ctx.Command.Name}]");
            return Task.CompletedTask;
        }

        private static async Task OnReady(DiscordClient sender, ReadyEventArgs e)
        {
            SetBotStatus();

            System.Console.WriteLine($"[{DateTime.Now}]\tBot is ready");
        }

        private static void SetBotStatus()
        {
            Bot.socketConnection.discordClient.UpdateStatusAsync(new DiscordActivity
            {
                Name = "prefix *" + ConfigManager.Config.Prefix + "*",
                ActivityType = ActivityType.ListeningTo
            });
        }

        private static async void PlayNextTrack()
        {
            if(Bot.Tracks.TryDequeue(out LavalinkTrack ?result))
            {
                System.Console.WriteLine($"[{DateTime.Now}]\tNow playing [{result.Title}]");
                var conn = Bot.socketConnection.lavalinkNode.GetGuildConnection(Bot.socketConnection.commandContext?.Member.VoiceState.Guild);
                Bot.IsPlaying = true;
                await conn.PlayAsync(result);
                await Bot.socketConnection.commandContext.RespondAsync($"Now playing [{result.Title}]!");
            }
        }
    }
}