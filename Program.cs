using LinqToDB;
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
    /* TODO
     *  - Siege Stats
     *  - Overwatch Stats
     *  - PUBG Stats
     *  - CSGO Stats
     *  - Diablo 3 Stats
     *  - Destiny 2 Stats
     *  - Commands for adding to siege maps & ops lists
     */

    public class Program
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;

        //Start up bot
        static void Main(string[] args)
        {
            //Init Global vars
            Global.Initialize();

            //Run FirstTimeLaunch if this is the first time the bot has been launched
            if (Global.isFirstLaunch)
            {
                try
                {
                    if (FirstTimeLaunch())
                    {
                        string msg = "The first time launch completed successfully!";
                        Global.LogMessage(msg, Severity.SUCCESS);
                    }
                    else
                    {
                        string msg = "The first time launch was not successfully completed.";
                        Global.LogMessage(msg, Severity.ERROR);
                    }

                    Global.isFirstLaunch = false;
                    //hiddenData.isFirstLaunch = false;
                }

                catch(Exception ex)
                {
                    Global.LogMessage(ex.Message, Severity.ERROR);
                }

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
            client.LeftGuild += LeftGuild;

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

            catch (LinqToDBException ex)
            {
                Global.LogMessage(ex.Message, Severity.ERROR);
                return false;
            }

            catch (Exception ex)
            {
                Global.LogMessage(ex.Message, Severity.ERROR);
                return false;
            }

            return true;

        }

        //Bot is connected and ready
        public Task Ready()
        {
            //Updates DB tables with guild/user info
            try
            {
                Dataset.IsActiveFalse();

                foreach (SocketGuild g in client.Guilds)
                {
                    Dataset.Guild newGuild = new Dataset.Guild
                    {
                        Guildid = g.Id.ToString(),
                        Guildname = g.Name,
                        Usercount = g.Users.Count,
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
                }
            }
            catch (LinqToDBException ex)
            {
                Global.LogMessage(ex.Message, Severity.ERROR);
            }
            catch (Exception ex)
            {
                Global.LogMessage(ex.Message, Severity.ERROR);
            }

            return Task.CompletedTask;
        }

        //Bot has been added to a guild
        public Task JoinedGuild(SocketGuild g)
        {
            try
            {
                Dataset.Guild newGuild = new Dataset.Guild
                {
                    Guildid = g.Id.ToString(),
                    Guildname = g.Name,
                    Usercount = g.Users.Count,
                    IsActive = true,
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

                Global.LogMessage($"I have been added to the {g.Name} Guild.", Severity.INFO);
            }

            catch (Exception ex)
            {
                Global.LogMessage(ex.Message, Severity.CRITICAL);
            }

            return Task.CompletedTask;
        }

        //Bot has been kicked from a guild.
        public Task LeftGuild(SocketGuild g)
        {
            try
            {
                Dataset.Guild newGuild = new Dataset.Guild
                {
                    Guildid = g.Id.ToString(),
                    Guildname = g.Name,
                    Usercount = g.Users.Count,
                    IsActive = false,
                };

                Dataset.GuildInsertOrUpdate(newGuild);
                Global.LogMessage($"I have been kicked from the {g.Name} Guild.", Severity.INFO);
            }

            catch (Exception ex)
            {
                Global.LogMessage(ex.Message, Severity.CRITICAL);
            }

            return Task.CompletedTask;
        }

        //User has joined a guild
        public Task UserJoined(SocketGuildUser u)
        {
            try
            {
                Dataset.Guild newGuild = new Dataset.Guild
                {
                    Guildid = u.Guild.Id.ToString(),
                    Guildname = u.Guild.Name,
                    Usercount = u.Guild.Users.Count,
                    IsActive = true
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
            }

            catch (Exception ex)
            {
                Global.LogMessage(ex.Message, Severity.CRITICAL);
            }

            return Task.CompletedTask;
        }

        //User has left a guild
        public Task UserLeft(SocketGuildUser u)
        {
            return Task.CompletedTask;
        }
    }
}