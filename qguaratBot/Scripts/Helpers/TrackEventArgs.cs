using DSharpPlus.Lavalink;
using DSharpPlus.CommandsNext;

namespace qguaratBot
{
    public class TrackEventArgs : EventArgs
    {
        public LavalinkTrack Track { get; set;}
    }
}