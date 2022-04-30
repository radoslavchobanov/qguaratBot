using DSharpPlus.Lavalink;

namespace qguaratBot
{
    public class Bot
    {
        public static SocketConnection socketConnection;
        public static Queue<LavalinkTrack> Tracks {get; private set;}
        public static event EventHandler<TrackEventArgs> TrackAddedToQueue;
        public static bool IsPlaying {get; set;}

        public Bot()
        {
            socketConnection = new SocketConnection();
            Tracks = new Queue<LavalinkTrack>();
            IsPlaying = false;
        }

        public async Task MainSync()
        {
            socketConnection.RegisterCommands<MusicCommands>();

            await socketConnection.MainSync();
        }

        public static Task AddTrack(LavalinkTrack track)
        {
            Tracks.Enqueue(track);
            TrackAddedToQueue.Invoke(new object(), new TrackEventArgs(){Track = track});
            return Task.CompletedTask;
        }

        public static void InvokeTrackAddedToQueue(LavalinkGuildConnection sender, TrackEventArgs a)
        {
            TrackAddedToQueue.Invoke(sender, a);
        }
    }
}