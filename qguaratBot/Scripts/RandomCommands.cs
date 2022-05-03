using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace qguaratBot
{
    public class RandomCommands : BaseCommandModule
    {
        [Command("greet")]
        [Description("Greet you")]
        public async Task GreetCommand(CommandContext ctx)
        {
            await ctx.RespondAsync(Bot.CreateEmbed("GREETINGS"));
        }
        
        [Command("roll")]
        [Description("Return a number from 0 to 100")]
        public async Task Roll(CommandContext ctx)
        {
            await ctx.RespondAsync(Bot.CreateEmbed("rolled: " + GetRandom()));
        }

        // get random number from 0 to 100
        private int GetRandom()
        {
            return new Random().Next(0, 100);
        }
    }
}