using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using Victoria;

namespace DiscordBot
{
    public class BotClient
    {
        private DiscordSocketClient client;
        private CommandService commandService;
        private IServiceProvider services;


        public BotClient(DiscordSocketClient client = null, CommandService commandService = null)
        {
            this.client = client ?? new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 50,

                LogLevel = LogSeverity.Debug,
            });
            this.commandService = commandService ?? new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,

                LogLevel = LogSeverity.Verbose,
            });
        }

        public async Task Initialize()
        {
            await this.client.LoginAsync(TokenType.Bot, "OTY2MjQzMTkyNjc4ODA1NTY1.Yl-6GQ.BI92ih4aYUdaA95AC5-5otkE2bg");
            await this.client.StartAsync();
            this.client.Log += LogAsync;

            var commandHandler = new CommandHandler(client, commandService, services);
            await commandHandler.InitializeAsync();
            
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage logMessage)
        {
            return Task.CompletedTask;
        }

        private IServiceProvider SetupServices()
        {
            return new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commandService)
                .AddSingleton<LavaRestClient>()
                .AddSingleton<LavaSocketClient>()
                .AddSingleton<MusicService>()
                .BuildServiceProvider();
        }
    }
}
