using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class Music : ModuleBase<SocketCommandContext>
    {
        private MusicService musicService;

        public Music(MusicService musicService)
        {
            this.musicService = musicService;
        }

        [Command("Join")]
        public async Task Join()
        {
            var user = Context.User as SocketGuildUser;

            if (user.VoiceChannel is null)
            {
                await ReplyAsync("You need to connect to a voice channel!!!");
                return;
            }

            await musicService.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
            await ReplyAsync($"now connected to {user.VoiceChannel.Name}");
        }
    }
}