using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("koi e nai golemiq pedal")]
        public async Task Pong()
        {
            await ReplyAsync("ivo");
        }
    }
}