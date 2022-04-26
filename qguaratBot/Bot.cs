using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.DependencyInjection;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;

namespace qguaratBot
{
    public class Bot
    {
        public static DiscordClient discordClient;
        private static DiscordConfiguration discordConfiguration;
        private static CommandsNextExtension commandsNextExtension;

        public static LavalinkNodeConnection lavalinkNode;
        private static LavalinkConfiguration lavalinkConfiguration;

        public Bot()
        {
            var services = new ServiceCollection()
                .AddSingleton<Commands>()
                .BuildServiceProvider();

            // if config.json file is missing it creates one
            if (string.IsNullOrWhiteSpace(ConfigManager.Config.Token)) return;

            discordConfiguration = SetDiscordConf();
            discordClient = new DiscordClient(discordConfiguration);

            commandsNextExtension = discordClient.UseCommandsNext(SetCommandsNextConf(services));

            lavalinkConfiguration = SetLavalinkConf(SetConnectionEndpointConf());
        }
        
        public async Task MainSync()
        {
            await EventManager.HandleEvents();

            commandsNextExtension.RegisterCommands<Commands>();

            await discordClient.ConnectAsync();

            var lavalink = discordClient.UseLavalink();
            lavalinkNode = await lavalink.ConnectAsync(lavalinkConfiguration);

            await Task.Delay(-1);
        }

        private DiscordConfiguration SetDiscordConf()
        {
            return new DiscordConfiguration()
            {
                Token = ConfigManager.Config.Token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information,
            };
        }
        private CommandsNextConfiguration SetCommandsNextConf(IServiceProvider services)
        {
            return new CommandsNextConfiguration()
            {
                UseDefaultCommandHandler = false,
                Services = services,
                CaseSensitive = ConfigManager.Config.CaseSensitive,
                StringPrefixes = new[] {ConfigManager.Config.Prefix},
            };
        }
        private ConnectionEndpoint SetConnectionEndpointConf()
        {
            return new ConnectionEndpoint
            {
                Hostname = ConfigManager.Config.Hostname,
                Port = ConfigManager.Config.Port,
            };
        }
        private LavalinkConfiguration SetLavalinkConf(ConnectionEndpoint cep)
        {
            return new LavalinkConfiguration
            {
                Password = ConfigManager.Config.Password,
                RestEndpoint = cep,
                SocketEndpoint = cep,
            };
        }
    }
}