using System.Reflection;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Jaxxis.Database;

namespace Jaxxis
{
    /*TODO
     * Strawpolls
     * Siege Statistics
     */

    public class Program
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;

        //Initialize global variables
        //Start up bot
        static void Main(string[] args)
        {
            Global.Initialize();

            //Run firstTimeLaunch if this is the first time the bot has been launched
            if (Global.isFirstLaunch)
            {
                var cc = Console.ForegroundColor;

                if (FirstTimeLaunch())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("FirstTimeLaunch was successfull!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FirstTimeLaunch was NOT successfull!");
                }

                Console.ForegroundColor = cc;
                Global.isFirstLaunch = false;
                //hiddenData.isFirstLaunch = false;

                Global.JsonHelper.JsonSerialize(Global.hiddenData);
            }

            new Program().Start().GetAwaiter().GetResult();
        }

        public async Task Start()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });

            commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();

            //Log bot errors/warnings
            client.Log += Logger;
            commands.Log += Logger;

            //Control bot events
            client.Ready += Ready;
            client.JoinedGuild += JoinedGuild;
            client.UserJoined += UserJoined;
            client.UserLeft += UserLeft;

            await InstallCommands();

            await client.LoginAsync(TokenType.Bot, Global.botToken);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        // Example of a logging handler. This can be re-used by addons
        // that ask for a Func<LogMessage, Task>.
        private static Task Logger(LogMessage message)
        {
            var cc = Console.ForegroundColor;
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message}");
            Console.ForegroundColor = cc;

            // If you get an error saying 'CompletedTask' doesn't exist,
            // your project is targeting .NET 4.5.2 or lower. You'll need
            // to adjust your project's target framework to 4.6 or higher
            // (instructions for this are easily Googled).
            // If you *need* to run on .NET 4.5 for compat/other reasons,
            // the alternative is to 'return Task.Delay(0);' instead.
            return Task.CompletedTask;
        }

        public async Task InstallCommands()
        {
            // Hook the MessageReceived Event into our Command Handler
            client.MessageReceived += HandleCommand;
            // Discover all of the commands in this assembly and load them.
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
            // Create a Command Context
            var context = new CommandContext(client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        } 

        //Runs on the first time launch of the bot
        public static bool FirstTimeLaunch()
        {
            try
            {
                Dataset.CreateGuildList();
                Dataset.CreateUserList();
            }
            catch
            {
                return false;
            }

            return true;

        }

        //Bot is connected and ready
        public Task Ready()
        {
            foreach (SocketGuild g in client.Guilds)
            {
                Dataset.Guild newGuild = new Dataset.Guild
                {
                    Guildid = g.Id.ToString(),
                    Guildname = g.Name,
                    Usercount = g.Users.Count
                };

                Dataset.GuildInsertOrUpdate(newGuild);

                foreach (SocketGuildUser u in g.Users)
                {
                    if(u.IsBot)
                    {
                        continue;
                    }

                    Dataset.User newUser = new Dataset.User
                    {
                        UserID = u.Id.ToString(),
                        Username = u.Username,
                    };

                    Dataset.UserInsertOrUpdate(newUser);
                }
            }
            return Task.CompletedTask;
        }

        //Bot has been added to a guild
        public Task JoinedGuild(SocketGuild g)
        {
            Dataset.Guild newGuild = new Dataset.Guild
            {
                Guildid = g.Id.ToString(),
                Guildname = g.Name,
                Usercount = g.Users.Count
            };

            Dataset.GuildInsertOrUpdate(newGuild);

            foreach (SocketGuildUser u in g.Users)
            {
                if (u.IsBot)
                {
                    continue;
                }

                Dataset.User newUser = new Dataset.User
                {
                    UserID = u.Id.ToString(),
                    Username = u.Username,
                };

                Dataset.UserInsertOrUpdate(newUser);
            }

            return Task.CompletedTask;
        }

        //User has joined a guild
        public Task UserJoined(SocketGuildUser u)
        {
            Dataset.Guild newGuild = new Dataset.Guild
            {
                Guildid = u.Guild.Id.ToString(),
                Guildname = u.Guild.Name,
                Usercount = u.Guild.Users.Count,
            };

            Dataset.GuildInsertOrUpdate(newGuild);

            if (u.IsBot)
            {
                return Task.CompletedTask;
            }

            Dataset.User newUser = new Dataset.User
            {
                UserID = u.Id.ToString(),
                Username = u.Username,
            };

            Dataset.UserInsertOrUpdate(newUser);

            return Task.CompletedTask;
        }

        //User has left a guild
        public Task UserLeft(SocketGuildUser u)
        {
            return Task.CompletedTask;
        }
    }
}