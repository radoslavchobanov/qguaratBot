using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Victoria;
using Victoria.Entities;

namespace DiscordBot
{
    public class MusicService
    {
        private LavaRestClient lavaRestClient;
        private LavaSocketClient lavaSocketClient;
        private readonly DiscordSocketClient client;

        public MusicService(LavaRestClient lavaRestClient, LavaSocketClient lavaSocketClient, DiscordSocketClient client)
        {
            this.lavaRestClient = lavaRestClient;
            this.lavaSocketClient = lavaSocketClient;
            this.client = client;
        }

        public Task InitalizeAsync()
        {
            client.Ready += ClientReadyAsync;
            lavaSocketClient.Log += LogAsync;
            lavaSocketClient.OnTrackFinished += TrackFinished;
            return Task.CompletedTask;
        }

        public async Task ConnectAsync(SocketVoiceChannel channel, ITextChannel textChannel)
        {
            await lavaSocketClient.ConnectAsync(channel, textChannel);
        }

        private async Task ClientReadyAsync()
        {
            await lavaSocketClient.StartAsync(client, new Configuration
            {
                LogSeverity = LogSeverity.Info
            });
        }

        private Task LogAsync(LogMessage arg)
        {
            return Task.CompletedTask;
        }

        private async Task TrackFinished(LavaPlayer player, LavaTrack track, TrackEndReason endReason)
        {
            if (!endReason.ShouldPlayNext())
                return;

            if (player.Queue.TryDequeue(out var item) || !(item is LavaTrack nextTrack))
            {
                await player.TextChannel.SendMessageAsync("There are no more tracks queued!!!");
                return;
            }

            await player.PlayAsync(nextTrack);
        }
    }
}