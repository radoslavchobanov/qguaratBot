using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Lavalink;

namespace qguaratBot
{
    public class MusicCommands : BaseCommandModule
    {
        [Command("greet")]
        public async Task GreetCommand(CommandContext ctx)
        {
            await ctx.RespondAsync("GREETINGS");
        }

        [Command("join")]
        public async Task JoinCommand(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            Bot.socketConnection.commandContext = ctx;

            var node = Bot.socketConnection.lavalinkNode;
            var channel = ctx.Member.VoiceState.Channel;
            var conn = node.GetGuildConnection(channel.Guild);
            
            if (conn != null)
            {
                Console.Log(Console.LogLevel.WARNING, $"Bot is already in channel! {channel.Name}");
                await ctx.RespondAsync($"Bot is already in channel! {channel.Name}");
                return;
            }

            if (channel.Type != ChannelType.Voice)
            {
                Console.Log(Console.LogLevel.ERROR, $"Not a valid voice channel {channel.Type}");
                await ctx.RespondAsync("Not a valid voice channel.");
                return;
            }

            await node.ConnectAsync(channel);
            Console.Log(Console.LogLevel.INFO, $"Bot Joined {channel.Name}");
            await ctx.RespondAsync($"Joined {channel.Name}!");
        }
        
        [Command("leave")]
        public async Task LeaveCommand(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            Bot.socketConnection.commandContext = null;

            var node = Bot.socketConnection.lavalinkNode;
            var channel = ctx.Member.VoiceState.Channel;
            var conn = node.GetGuildConnection(channel.Guild);

            if (conn == null)
            {
                Console.Log(Console.LogLevel.ERROR, $"Bot is not in a voice channel!");
                await ctx.RespondAsync("Bot is not in a voice channel!");
                return;
            }

            // when leaving, it should trigger the OnTrackFinished event

            await conn.DisconnectAsync();
            Console.Log(Console.LogLevel.INFO, $"Bot left {channel.Name}");
            await ctx.RespondAsync($"Left {channel.Name}!");
        }

        [Command("play")]
        public async Task Play(CommandContext ctx, [RemainingText] string search)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await JoinCommand(ctx);
            }

            var loadResult = await node.Rest.GetTracksAsync(search);

            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed 
                || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.RespondAsync($"Track search failed for {search}.");
                return;
            }

            var track = loadResult.Tracks.First();
            
            await Bot.AddTrack(track);
        }
    }
}