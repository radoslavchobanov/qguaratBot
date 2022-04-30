using DSharpPlus.Lavalink;

namespace qguaratBot
{
    public class Bot
    {
        public static Queue<LavalinkTrack> Tracks {get; private set;}
        public static ConnectionManager ConnectionManager {get; private set;}

        #region Events
        public static event EventHandler<TrackEventArgs> TrackAddedToQueue;
        public static event EventHandler AudioPlayerStopped;
        #endregion
        
        public static bool isPlaying;

        public static int AFK_TIMER = ConfigManager.Config.AFK_TIMER;

        public Bot()
        {
            Tracks = new Queue<LavalinkTrack>();
            ConnectionManager = new ConnectionManager();
            isPlaying = false;
        }

        public async Task MainSync()
        {
            ConnectionManager.RegisterCommands<MusicCommands>();

            await ConnectionManager.MainSync();
        }
        public static async Task PlayNextTrack()
        {
            var audioPlayer = Bot.ConnectionManager.lavalinkNode.GetGuildConnection(Bot.ConnectionManager.commandContext?.Member.VoiceState.Guild);
                
            if(Bot.Tracks.TryDequeue(out LavalinkTrack ?result))
            {
                await audioPlayer.PlayAsync(result);
            }
            else
            {
                await audioPlayer.StopAsync();
                AudioPlayerStopped.Invoke(new object(), new EventArgs());
            }
        }

        public static void AddTrack(LavalinkTrack track)
        {
            Tracks.Enqueue(track);
            TrackAddedToQueue.Invoke(new object(), new TrackEventArgs(){Track = track});
        }

        public static async void SkipTrack()
        {
            if (isPlaying) await PlayNextTrack();
        }
    }
}