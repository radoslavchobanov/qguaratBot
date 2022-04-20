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
                await ReplyAsync("You need to connect to a voice channel !!!");
                return;
            }

            await musicService.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
            await ReplyAsync($"qguaratBot now connected to {user.VoiceChannel.Name}");

            // TODO: set the current voice channel when a bot connects, rather than when we are using the command join
            // TODO: same for leave..
            BotClient.singleton.SetCurrentVoiceChannel(user.VoiceChannel);
        }

        [Command("Leave")]
        public async Task Leave()
        {
            var user = Context.User as SocketGuildUser;

            if (BotClient.singleton.CurrentVoiceChannel == null)
            {
                await ReplyAsync("qguaratBot is not connected to any voice channel !!!");
                return;
            }
            else if (user.VoiceChannel != BotClient.singleton.CurrentVoiceChannel)
            {
                await ReplyAsync("You need to be in the same channel as qguaratBot in order to disconnect it !!!");
                return;
            }

            await musicService.DisconnectAsync(BotClient.singleton.CurrentVoiceChannel);
            await ReplyAsync($"qguaratBot disconnected from {Context.Channel.Name}");

            BotClient.singleton.SetCurrentVoiceChannel(null);
        }

        [Command("Play")]
        public async Task Play([Remainder]string query)
        {
            var result = await musicService.PlayAsync(query, Context.Guild.Id);
            await ReplyAsync(result);
        }

        [Command("Channel")]
        public async Task ShowChannel()
        {
            if (BotClient.singleton.CurrentVoiceChannel == null)
            {
                await ReplyAsync("qguaratBot is not connected to any voice channel !!!");
                return;
            }

            await ReplyAsync("qguaratBot is connected in " + BotClient.singleton.CurrentVoiceChannel);
        }
    }
}