using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("pedal")]
        public async Task Pong()
        {
            await ReplyAsync("awe ei");
        }
    }
}