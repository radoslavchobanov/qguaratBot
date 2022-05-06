using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.CommandsNext;

namespace qguaratBot
{
    public class Bot
    {
        public static Queue<LavalinkTrack> Tracks {get; private set;}
        public static bool isPlaying;
        
        #region Events
        public static event EventHandler<TrackEventArgs> TrackAddedToQueue;
        public static event EventHandler AudioPlayerStopped;
        #endregion

        public Bot()
        {
            Tracks = new Queue<LavalinkTrack>();
            isPlaying = false;
        }

        public async Task MainSync()
        {
            ConnectionManager.Initialize();
            ConnectionManager.commandsNextExtension.SetHelpFormatter<CustomHelpFormatter>();
            ConnectionManager.RegisterCommands<MusicCommands>();
            ConnectionManager.RegisterCommands<RandomCommands>();

            await ConnectionManager.MainSync();
        }
        public static async Task PlayNextTrack()
        {
            var audioPlayer = ConnectionManager.lavalinkNode.GetGuildConnection(ConnectionManager.commandContext?.Member.VoiceState.Guild);
                
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
            TrackAddedToQueue(new object(), new TrackEventArgs(){Track = track});
        }

        public static async void SkipTrack()
        {
            if (isPlaying) await PlayNextTrack();
        }

        public static DiscordEmbedBuilder CreateEmbed(string text, string emojiName = null)
        {
            if (emojiName != null)
            {
                var emoji = DiscordEmoji.FromName(ConnectionManager.discordClient, emojiName);
                text = text.Insert(0, emoji);
            }
            text += " | " + ConnectionManager.commandContext?.Member?.Mention;

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
                Description = text,

                Color = DiscordColor.SpringGreen,
            };

            return embed;
        }
    }
}