using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.DependencyInjection;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;

namespace qguaratBot
{
    public static class ConnectionManager
    {
        // this is set when command JOIN is executed / when bot has joined a voice channel
        // and is used for Responding messages on the discord chat
        public static CommandContext commandContext;

        public static DiscordClient discordClient;
        public static DiscordConfiguration discordConfiguration;
        public static CommandsNextExtension commandsNextExtension;

        public static LavalinkNodeConnection lavalinkNode;

        public static LavalinkConfiguration lavalinkConfiguration;
        
        public static void Initialize()
        {
            var services = new ServiceCollection()
                .BuildServiceProvider();

            // if config.json file is missing it creates one
            if (string.IsNullOrWhiteSpace(ConfigManager.Config.Token)) return;

            discordConfiguration = SetDiscordConf();
            discordClient = new DiscordClient(discordConfiguration);

            commandsNextExtension = discordClient.UseCommandsNext(SetCommandsNextConf(services));

            lavalinkConfiguration = SetLavalinkConf(SetConnectionEndpointConf());
        }
        
        public static async Task MainSync()
        {
            await discordClient.ConnectAsync();

            var lavalink = discordClient.UseLavalink();
            lavalinkNode = await lavalink.ConnectAsync(lavalinkConfiguration);
            
            await EventManager.HandleEvents();

            await Task.Delay(-1);
        }

        public static void RegisterCommands<T>() where T:BaseCommandModule
        {
            commandsNextExtension.RegisterCommands<T>();
            Console.Log(Console.LogLevel.INFO, $"Commands: {typeof(T).ToString().Split(".")[1]} registered succesfully!");
        }
        
        private static DiscordConfiguration SetDiscordConf()
        {
            return new DiscordConfiguration()
            {
                Token = ConfigManager.Config.Token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information,
            };
        }
        
        private static CommandsNextConfiguration SetCommandsNextConf(IServiceProvider services)
        {
            return new CommandsNextConfiguration()
            {
                UseDefaultCommandHandler = false,
                Services = services,
                CaseSensitive = ConfigManager.Config.CaseSensitive,
                StringPrefixes = new[] {ConfigManager.Config.Prefix},
            };
        }
        private static ConnectionEndpoint SetConnectionEndpointConf()
        {
            return new ConnectionEndpoint
            {
                Hostname = "127.0.0.1",
                Port = 2333,
            };
        }
        private static LavalinkConfiguration SetLavalinkConf(ConnectionEndpoint cep)
        {
            return new LavalinkConfiguration
            {
                Password = "youshallnotpass",
                RestEndpoint = cep,
                SocketEndpoint = cep,
            };
        }
    }
}