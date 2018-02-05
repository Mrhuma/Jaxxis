using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Jaxxis.Database;
using System.Collections;

namespace Jaxxis
{
    //Global static variables
    public class Global
    {
        public static bool isFirstLaunch;
        public static string botToken = "";
        public static string imageURL = "";
        public static string PUBGApiKey = "";
        public static string gameValue = "";
        public static string filePath = @"..\..\Data\";
        public static List<HelpMessage> HelpMessageCache = new List<HelpMessage>();
        public static List<string> SiegeAttackOps;
        public static List<string> SiegeDefenseOps;
        public static List<string> SiegeCasualMapPool;
        public static List<string> SiegeRankedMapPool;
        public static Emoji emoji1 = new Emoji("1⃣");
        public static Emoji emoji2 = new Emoji("2⃣");
        public static Emoji emoji3 = new Emoji("3⃣");
        public static Emoji emoji4 = new Emoji("4⃣");
        public static Emoji emoji5 = new Emoji("5⃣");
        public static Emoji emoji6 = new Emoji("6⃣");
        public static Emoji emoji7 = new Emoji("7⃣");
        public static Emoji emoji8 = new Emoji("8⃣");
        public static Emoji emoji9 = new Emoji("9⃣");
        public static Emoji emojiback = new Emoji("↩");
        public static Emoji emojitrash = new Emoji("🗑");
        public static JSONHelper JsonHelper = new JSONHelper();
        public static HiddenData hiddenData;

        public static void Initialize()
        {
            hiddenData = JsonHelper.JsonDeserialize();

            isFirstLaunch = hiddenData.IsFirstLaunch;
            botToken = hiddenData.BotToken;
            gameValue = "!help";
            imageURL = hiddenData.ImageURL;
            PUBGApiKey = hiddenData.PUBGApiKey;
            SiegeAttackOps = hiddenData.SiegeAttackOps;
            SiegeDefenseOps = hiddenData.SiegeDefenseOps;
            SiegeCasualMapPool = hiddenData.SiegeCasualMapPool;
            SiegeRankedMapPool = hiddenData.SiegeRankedMapPool;
        }

        public static async Task<Embed> LogError(Exception ex, CommandContext context = null)
        {
            string Username = null;
            if (context != null)
            {
                Username = context.User.Username;
            }

            string Message = ex.Message;
            string StackTrace = ex.StackTrace;

            Dataset.ErrorLog newError = new Dataset.ErrorLog
            {
                Message = Message,
                StackTrace = StackTrace,
                User = Username
            };
            
            await Dataset.InsertErrorLog(newError);

            EmbedBuilder embedBuilder = new EmbedBuilder
            {
                Color = Color.Red,
                Title = Message,
                Footer = new EmbedFooterBuilder
                {
                    Text = DateTime.UtcNow.ToString() + " UTC"
                },
            };

            Embed embed = embedBuilder.Build();
            return embed;
        }

        public static async Task LogMessage(string message)
        {
            Dataset.MessageLog newMessageLog = new Dataset.MessageLog
            {
                Message = message,
                Time = DateTime.Now,
            };

            await Dataset.InsertMessageLog(newMessageLog);
        }
    }

    //Type-Safe-Enum - https://stackoverflow.com/questions/424366/c-sharp-string-enums?page=1&tab=votes#tab-top
    public sealed class HelpState
    {
        private readonly String Message;
        private readonly int Value;
        public readonly String Footer;

        public static readonly HelpState START = new HelpState(1, "start", "");
        public static readonly HelpState GAME = new HelpState(11, "game", "Game");
        public static readonly HelpState SIEGE = new HelpState(111, "siege", "Game-Siege");
        public static readonly HelpState SIEGECMD = new HelpState(1110, "siegecmd", "");
        public static readonly HelpState PUBG = new HelpState(112, "pubg", "PUBG");
        public static readonly HelpState PUBGCMD = new HelpState(1120, "pubgcmd", "");
        public static readonly HelpState INFO = new HelpState(12, "info", "Info");
        public static readonly HelpState POLL = new HelpState(121, "poll", "Info-Poll");
        public static readonly HelpState POLLCMD = new HelpState(1210, "pollcmd", "");

        private static List<HelpState> helpStateList = new List<HelpState>()
        {
            START,
            GAME,
            SIEGE,
            SIEGECMD,
            PUBG,
            PUBGCMD,
            INFO,
            POLL,
            POLLCMD,
        };

        private HelpState(int Value, String Message, String Footer)
        {
            this.Message = Message;
            this.Value = Value;
            this.Footer = Footer;
        }

        public HelpState GoBack()
        {
            string stateString = Value.ToString();
            if(stateString.Length == 1) { return this; }
            stateString = stateString.Remove(stateString.Length - 1);

            foreach (var state in helpStateList)
            {
                if (state.Value.ToString() == stateString)
                {
                    return state;
                }
            }

            return null;
        }

        public HelpState GetState(string stateName)
        {
            foreach (HelpState state in helpStateList)
            {
                if (state.Message == stateName)
                {
                    return state;
                }
            }

            return null;
        }

        public HelpState ToCMD()
        {
            foreach(var state in helpStateList)
            {
                if(state.Message == Message + "cmd")
                {
                    return state;
                }
            }

            return this;
        }

        public int Length()
        {
            string stateLength = Value.ToString();

            return stateLength.Length;
        }

        public override string ToString()
        {
            return Message;
        }
    }

    public class HelpMessage
    {
        public IUserMessage Message;
        public HelpState State;
        public DateTime Time;

        public HelpMessage(IUserMessage message, HelpState state, DateTime time)
        {
            Message = message;
            State = state;
            Time = time;
        }
    }
}