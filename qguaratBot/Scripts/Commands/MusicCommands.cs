using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Lavalink;

namespace qguaratBot
{
    public class MusicCommands : BaseCommandModule
    {
        [Command("join")]
        [Description("Joins the bot to the user's channel")]
        public async Task JoinCommand(CommandContext ctx)
        {
            ConnectionManager.commandContext = ctx;

            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync(Bot.CreateEmbed("You are not in a voice channel.", Emojis.noEntrySign));
                return;
            }

            var node = ConnectionManager.lavalinkNode;
            var channel = ctx.Member.VoiceState.Channel;
            var conn = node.GetGuildConnection(channel.Guild);
            
            if (conn != null)
            {
                Console.Log(Console.LogLevel.WARNING, $"Bot is already in channel! {channel.Name}");
                await ctx.RespondAsync(Bot.CreateEmbed($"Bot is already in channel! {channel.Name}"));
                return;
            }

            if (channel.Type != ChannelType.Voice)
            {
                Console.Log(Console.LogLevel.ERROR, $"Not a valid voice channel {channel.Type}");
                await ctx.RespondAsync(Bot.CreateEmbed("Not a valid voice channel.", Emojis.noEntrySign));
                return;
            }

            await node.ConnectAsync(channel);
            Console.Log(Console.LogLevel.INFO, $"Bot Joined {channel.Name}");
            await ctx.RespondAsync(Bot.CreateEmbed($"Joined {channel.Name}!", Emojis.whiteCheckMark));
        }
        
        [Command("leave")]
        [Description("Disconnects the bot from the current channel if the users is in it")]
        public async Task LeaveCommand(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync(Bot.CreateEmbed("You are not in a voice channel.", Emojis.noEntrySign));
                return;
            }

            ConnectionManager.commandContext = null;

            var node = ConnectionManager.lavalinkNode;
            var channel = ctx.Member.VoiceState.Channel;
            var conn = node.GetGuildConnection(channel.Guild);

            if (conn == null)
            {
                Console.Log(Console.LogLevel.ERROR, $"Bot is not in a voice channel!");
                await ctx.RespondAsync(Bot.CreateEmbed("Bot is not in a voice channel!", Emojis.noEntrySign));
                return;
            }

            // when leaving, it should trigger the OnTrackFinished event

            await conn.DisconnectAsync();
            Console.Log(Console.LogLevel.INFO, $"Bot left {channel.Name}");
            await ctx.RespondAsync(Bot.CreateEmbed($"Left {channel.Name}!", Emojis.exclamation));
        }

        [Command("play")]
        [Description("Plays music with a given link or song name")]
        public async Task Play(CommandContext ctx, [RemainingText] string search)
        {
            ConnectionManager.commandContext = ctx;

            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                Console.Log(Console.LogLevel.ERROR, $"You are not in a voice channel!");
                await ctx.RespondAsync(Bot.CreateEmbed("You are not in a voice channel.", Emojis.noEntrySign));
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
                Console.Log(Console.LogLevel.ERROR, $"Track search failed for [{search}]");
                await ctx.RespondAsync(Bot.CreateEmbed($"Track search failed for [{search}]", Emojis.exclamation));
                return;
            }

            var track = loadResult.Tracks.First();

            Bot.AddTrack(track);
        }

        [Command("skip")]
        [Description("Skips currently played song")]
        public async Task Skip(CommandContext ctx)
        {
            ConnectionManager.commandContext = ctx;
            
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                Console.Log(Console.LogLevel.ERROR, $"You are not in a voice channel!");
                await ctx.RespondAsync(Bot.CreateEmbed("You are not in a voice channel!", Emojis.noEntrySign));
                return;
            }
            
            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                Console.Log(Console.LogLevel.ERROR, $"Bot is not in a voice channel!");
                await ctx.RespondAsync(Bot.CreateEmbed("Bot is not in a voice channel!", Emojis.noEntrySign));
                return;
            }

            Bot.SkipTrack();
        }
        
        [Command("list")]
        [Description("List currently queued tracks")]
        public async Task List(CommandContext ctx)
        {
            string listText = "";
            int index = 1;

            if (Bot.Tracks.Count == 0)
            {
                await ctx.RespondAsync(Bot.CreateEmbed("Track queue is empty!", Emojis.exclamation));
                return;
            }

            foreach (LavalinkTrack track in Bot.Tracks)
            {
                listText += index.ToString()
                         + ". "
                         + track.Title
                         + "\n";
                index++;
            }

            await ctx.RespondAsync(Bot.CreateEmbed(listText));
        }
    }
}