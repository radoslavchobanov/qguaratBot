using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient client;
        private readonly CommandService commandService;
        private readonly IServiceProvider services;

        public CommandHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider services)
        {
            this.client = client;
            this.commandService = commandService;
            this.services = services;
        }

        public async Task InitializeAsync()
        {
            await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), services);
            commandService.Log += LogAsync;
            client.MessageReceived += HandleMessageAsync;
        }

        private Task LogAsync(LogMessage logMessage)
        {
            return Task.CompletedTask;
        }

        private async Task HandleMessageAsync(SocketMessage message)
        {
            var argPos = 0;
            var userMessage = message as SocketUserMessage ?? null;

            if (message.Author.IsBot)
                return;

            if (!userMessage.HasStringPrefix("qguarat ", ref argPos))
                return;

            var context = new SocketCommandContext(client, userMessage);
            var result = await commandService.ExecuteAsync(context, argPos, services);
        }
    }
}