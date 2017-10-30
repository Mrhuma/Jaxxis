using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Jaxxis.Database;
using Jaxxis;

namespace Jaxxis.Modules
{
    public class InfoModule : ModuleBase
    {

        // ~say hello -> hello
        [Command("ping"), Summary("Ping Pong!")]
        public async Task Ping()
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync("Pong!");
        }
    }
}
