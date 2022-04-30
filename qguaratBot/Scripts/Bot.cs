using DSharpPlus.Lavalink;

namespace qguaratBot
{
    public class Bot
    {
        public static bool trackSkipped = false;

        public static Queue<LavalinkTrack> Tracks {get; private set;}
        public static ConnectionManager ConnectionManager {get; private set;}

        #region Events
        public static event EventHandler<TrackEventArgs> TrackAddedToQueue;
        #endregion
        
        public static bool isPlaying;

        public Bot()
        {
            ConnectionManager = new ConnectionManager();
            Tracks = new Queue<LavalinkTrack>();
            isPlaying = false;
        }

        public async Task MainSync()
        {
            ConnectionManager.RegisterCommands<MusicCommands>();

            await ConnectionManager.MainSync();
        }
        public static async Task PlayNextTrack()
        {
            if(Bot.Tracks.TryDequeue(out LavalinkTrack ?result))
            {
                var conn = Bot.ConnectionManager.lavalinkNode.GetGuildConnection(Bot.ConnectionManager.commandContext?.Member.VoiceState.Guild);
                await conn.PlayAsync(result);
            }
            else
            {
                Console.Log(Console.LogLevel.WARNING, "Song queue is empty!");
            }
        }

        public static Task AddTrack(LavalinkTrack track)
        {
            Tracks.Enqueue(track);
            TrackAddedToQueue.Invoke(new object(), new TrackEventArgs(){Track = track});

            return Task.CompletedTask;
        }

        public static async void SkipTrack()
        {
            trackSkipped = true;
            if (isPlaying) await PlayNextTrack();
        }
    }
}