using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Jaxxis.Database;
using LinqToDB;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Squirrel;
using IWshRuntimeLibrary;

namespace Jaxxis
{
    /* TODO
     *  - Siege Stats
     *  - Overwatch Stats
     *  - CSGO Stats
     *  - Diablo 3 Stats
     *  - Destiny 2 Stats
     *  - Owner Command for Bot Game
     */

    public class Program
    {
        //private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;
        public object helpMenu;

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
                        Console.WriteLine("First Time Launch was a success.");
                    }
                    else
                    {
                        Console.WriteLine("First Time Launch wasn't a success.");
                    }

                    Global.isFirstLaunch = false;
                    //hiddenData.isFirstLaunch = false;
                }

                catch(Exception ex)
                {
                    Task.Run(() => Global.LogError(ex).GetAwaiter());
                }

                Global.JsonHelper.JsonSerialize(Global.hiddenData);
            }

            new Program().Start().GetAwaiter().GetResult();
        }

        public async Task Start()
        {

            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                WebSocketProvider = Discord.Net.Providers.WS4Net.WS4NetProvider.Instance
            });

            helpMenu = new HelpMenu();
            Global.commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();
            //Log bot errors/warnings
            client.Log += Logger;
            Global.commands.Log += Logger;

            //Control bot events
            client.Ready += Ready;
            client.JoinedGuild += JoinedGuild;
            client.UserJoined += UserJoined;
            client.UserLeft += UserLeft;
            client.LeftGuild += LeftGuild;
            client.ReactionAdded += ReactionAdded;
            client.ReactionRemoved += ReactionRemoved;

            await client.SetGameAsync(Global.gameValue);

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
            await Global.commands.AddModulesAsync(Assembly.GetEntryAssembly());
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
            var result = await Global.commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        } 

        //Runs on the first time launch of the bot
        public static bool FirstTimeLaunch()
        {
            try
            {
                Task.Run(() => Dataset.CreateAllTables().GetAwaiter());
            }

            catch (LinqToDBException ex)
            {
                Task.Run(() => Global.LogError(ex).GetAwaiter());
                return false;
            }

            catch (Exception ex)
            {
                Task.Run(() => Global.LogError(ex).GetAwaiter());
                return false;
            }

            return true;
        }

        //Bot is connected and ready
        public async Task Ready()
        {
            await Task.Run(() => new HelpMenu().ReactionMenuStartEmbed());
            await Task.Run(() => new HelpMenu().ReactionMenuExitEmbed());

            //Updates DB tables with guild/user info
            try
            {
                await Dataset.IsActiveFalse();

                foreach (SocketGuild g in client.Guilds)
                {
                    Dataset.Guild newGuild = new Dataset.Guild
                    {
                        Guildid = g.Id.ToString(),
                        Guildname = g.Name,
                        Usercount = g.Users.Count,
                        IsActive = true,
                    };

                    await Dataset.GuildInsertOrUpdate(newGuild);

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

                        await Dataset.UserInsertOrUpdate(newUser);
                    }
                }
            }
            catch (LinqToDBException ex)
            {
                await Global.LogError(ex);
            }
            catch (Exception ex)
            {
                await Global.LogError(ex);
            }

            new Update().StartTimer();
        }

        //Bot has been added to a guild
        public async Task JoinedGuild(SocketGuild g)
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

                await Dataset.GuildInsertOrUpdate(newGuild);

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

                    await Dataset.UserInsertOrUpdate(newUser);
                }

                await Global.LogMessage($"I have been added to the {g.Name} Guild.");
            }

            catch (Exception ex)
            {
                await Global.LogError(ex);
            }
        }

        //Bot has been kicked from a guild.
        public async Task LeftGuild(SocketGuild g)
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

                await Dataset.GuildInsertOrUpdate(newGuild);
                await Global.LogMessage($"I have been kicked from the {g.Name} Guild.");
            }

            catch (Exception ex)
            {
                await Global.LogError(ex);
            }
        }

        //User has joined a guild
        public async Task UserJoined(SocketGuildUser u)
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

                await Dataset.GuildInsertOrUpdate(newGuild);

                if (u.IsBot)
                {
                    return;
                }

                Dataset.User newUser = new Dataset.User
                {
                    UserID = u.Id.ToString(),
                    Username = u.Username,
                };

                await Dataset.UserInsertOrUpdate(newUser);
            }

            catch (Exception ex)
            {
                await Global.LogError(ex);
            }
        }

        //User has left a guild
        public Task UserLeft(SocketGuildUser u)
        {
            return Task.CompletedTask;
        }
        
        //Reaction removed
        public async Task ReactionRemoved(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            try
            {
                if (reaction.UserId != client.CurrentUser.Id)
                {
                    foreach (var msg in Global.HelpMessageCache)
                    {
                        if (msg.Message.Id == reaction.MessageId)
                        {
                            if (reaction.Emote.Name == "↩")
                            {
                                HelpState msgState = Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First().State;
                                msgState = msgState.GoBack();
                                Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First().State = msgState;
                            }
                            await Task.Run(() => new HelpMenu().ReactionMenu(reaction).GetAwaiter());
                            break;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                await Global.LogError(ex);
            }
        }
        
        //Reaction added
        public async Task ReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            try
            {
                if (reaction.UserId != client.CurrentUser.Id)
                {
                    foreach (var msg in Global.HelpMessageCache)
                    {
                        if (msg.Message.Id == reaction.MessageId)
                        {
                            if(reaction.Emote.Name == "↩")
                            {
                                HelpState msgState = Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First().State;
                                msgState = msgState.GoBack();
                                Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First().State = msgState;
                            }
                            await Task.Run(() => new HelpMenu().ReactionMenu(reaction).GetAwaiter());
                            break;
                        }
                    }
                }
            }

            catch(Exception ex)
            {
                await Global.LogError(ex);
            }
        }
    }
}