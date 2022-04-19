using System;
using Discord.WebSocket;
using Discord.Commands;

namespace DiscordBot
{
    public class BotClient
    {
        private DiscordSocketClient _client;
        private CommandService _commandService;


        public BotClient(DiscordSocketClient client = null, CommandService commandService = null)
        {
            _client = client;
            _commandService = commandService;
        }
    }
}
