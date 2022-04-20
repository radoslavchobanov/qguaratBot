using System.Linq;
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
        private LavaPlayer player;

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

        public async Task DisconnectAsync(SocketVoiceChannel channel)
        {
            await lavaSocketClient.DisconnectAsync(channel);
        }

        public async Task<string> PlayAsync(string query, ulong guildId)
        {
            player = lavaSocketClient.GetPlayer(guildId);

            var result = await lavaRestClient.SearchYouTubeAsync(query);
            System.Console.WriteLine("here");

            if (result.LoadType == LoadType.NoMatches || result.LoadType == LoadType.LoadFailed)
                return "No matches found !!!";
            
            var track = result.Tracks.FirstOrDefault();

            if (player.IsPlaying)
            {
                player.Queue.Enqueue(track);
                return $"{track.Title} has been added to the playlist";
            }
            else 
            {
                await player.PlayAsync(track);
                return $"Now playing: {track.Title}";
            }
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