using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Jaxxis
{
    class HelpMenu
    {
        private static Embed startEmbed;
        private static Embed exitEmbed;

        //Switch cases for the Reaction Help Menu
        public async Task ReactionMenu(SocketReaction reaction)
        {
            HelpMessage helpmsg = Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First();
            IUserMessage message = helpmsg.Message;
            HelpState helpState = helpmsg.State;

            string userChoice = "";
            int userChoiceIndex = 0;

            if (reaction.Emote.Name == "🗑")
            {
                await message.ModifyAsync(x =>
                {
                    x.Content = "";
                    x.Embed = exitEmbed;
                });

                Global.HelpMessageCache.Remove(Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First());
                return;
            }

            Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First().Time = DateTime.UtcNow;

            switch (helpState.Length())
            {
                case 1:
                    switch (reaction.Emote.Name)
                    {
                        case "1⃣":
                            userChoiceIndex = 0;
                            break;

                        case "2⃣":
                            userChoiceIndex = 1;
                            break;

                        case "3⃣":
                            userChoiceIndex = 2;
                            break;

                        case "4⃣":
                            userChoiceIndex = 3;
                            break;

                        case "5⃣":
                            userChoiceIndex = 4;
                            break;

                        case "6⃣":
                            userChoiceIndex = 5;
                            break;

                        case "7⃣":
                            userChoiceIndex = 6;
                            break;

                        case "8⃣":
                            userChoiceIndex = 7;
                            break;

                        case "9⃣":
                            userChoiceIndex = 8;
                            break;

                        case "↩":
                            await Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First().Message.ModifyAsync(x =>
                            {
                                x.Content = "";
                                x.Embed = startEmbed;
                            });
                            return;

                        default:
                            return;

                    }

                    foreach (var newEmbed in message.Embeds)
                    {
                        //Add logic for checking if the embed is the one for the menu.
                        if (newEmbed.Title == "Jaxxis Help Menu")
                        {
                            if (userChoiceIndex <= newEmbed.Fields.Count() - 1)
                            {
                                userChoice = newEmbed.Fields[userChoiceIndex].Name.Split(' ').Last().ToLower();

                                helpState = helpState.GetState(userChoice);
                                Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First().State = helpState;

                                await ReactionMenuLogicStageOne(reaction);
                            }
                            break;
                        }
                    }
                    break;

                case 2:
                    switch (reaction.Emote.Name)
                    {
                        case "1⃣":
                            userChoiceIndex = 0;
                            break;

                        case "2⃣":
                            userChoiceIndex = 1;
                            break;

                        case "3⃣":
                            userChoiceIndex = 2;
                            break;

                        case "4⃣":
                            userChoiceIndex = 3;
                            break;

                        case "5⃣":
                            userChoiceIndex = 4;
                            break;

                        case "6⃣":
                            userChoiceIndex = 5;
                            break;

                        case "7⃣":
                            userChoiceIndex = 6;
                            break;

                        case "8⃣":
                            userChoiceIndex = 7;
                            break;

                        case "9⃣":
                            userChoiceIndex = 8;
                            break;

                        case "↩":
                            await ReactionMenuLogicStageOne(reaction);
                            return;

                        default:
                            return;
                    }

                    foreach (var newEmbed in message.Embeds)
                    {
                        //Add Logic for checking if this is the embed used for the help menu.
                        if (newEmbed.Title == "Jaxxis Help Menu")
                        {
                            if (userChoiceIndex <= newEmbed.Fields.Count() - 1)
                            {
                                userChoice = newEmbed.Fields[userChoiceIndex].Name.Split(' ').Last().ToLower();

                                helpState = helpState.GetState(userChoice);
                                Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First().State = helpState;

                                await ReactionMenuLogicStageTwo(reaction);
                            }
                            break;
                        }
                    }
                    break;

                case 3:
                    switch (reaction.Emote.Name)
                    {
                        case "1⃣":
                            userChoiceIndex = 0;
                            break;

                        case "2⃣":
                            userChoiceIndex = 1;
                            break;

                        case "3⃣":
                            userChoiceIndex = 2;
                            break;

                        case "4⃣":
                            userChoiceIndex = 3;
                            break;

                        case "5⃣":
                            userChoiceIndex = 4;
                            break;

                        case "6⃣":
                            userChoiceIndex = 5;
                            break;

                        case "7⃣":
                            userChoiceIndex = 6;
                            break;

                        case "8⃣":
                            userChoiceIndex = 7;
                            break;

                        case "9⃣":
                            userChoiceIndex = 8;
                            break;

                        case "↩":
                            await ReactionMenuLogicStageTwo(reaction);
                            return;

                        default:
                            return;
                    }

                    foreach (var newEmbed in message.Embeds)
                    {
                        //Add Logic for checking if this is the embed used for the help menu.
                        if (newEmbed.Title == "Jaxxis Help Menu")
                        {
                            if (userChoiceIndex <= newEmbed.Fields.Count() - 1)
                            {
                                userChoice = newEmbed.Fields[userChoiceIndex].Name.Split(' ').Last().ToLower();

                                helpState = helpState.ToCMD();
                                Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First().State = helpState;

                                await ReactionMenuLogicStageThree(reaction, userChoice);
                            }
                            break;
                        }
                    }
                    break;
            }
        }

        //Garbage Collection for old instances of Help Menus
        public Task ReactionMenuGarbageCollection()
        {
            List<HelpMessage> helpMessageDelete = new List<HelpMessage>();

            foreach (var msg in Global.HelpMessageCache)
            {
                if (msg.Time.AddMinutes(5) < DateTime.UtcNow)
                {
                    helpMessageDelete.Add(msg);
                }
            }

            foreach (var msg in helpMessageDelete)
            {
                Global.HelpMessageCache.Remove(msg);
                msg.Message.ModifyAsync(x =>
                {
                    x.Content = "";
                    x.Embed = exitEmbed;
                });
            }

            return Task.CompletedTask;
        }

        //Logic for Stage One of the Help Menu (Category)
        public async Task ReactionMenuLogicStageOne(SocketReaction reaction)
        {
            HelpState helpState = Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First().State;
            List<ModuleInfo> moduleList = new List<ModuleInfo>();
            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
            EmbedFieldBuilder newField = new EmbedFieldBuilder();
            List<string> valueList = new List<string>();

            Embed embed;
            EmbedBuilder embedBuilder = new EmbedBuilder
            {
                Title = "Jaxxis Help Menu",
                Description = "--------------------------------------------------",
                Color = Color.Green,
                Footer = new EmbedFooterBuilder
                {
                    Text = helpState.Footer
                }
            };

            IUserMessage message = Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First().Message;
            string userChoice = helpState.ToString();

            int i = 1;

            foreach (ModuleInfo module in Global.commands.Modules)
            {
                if (module.Remarks != null)
                {
                    if (module.Remarks.ToLower() == userChoice)
                    {
                        newField.Name = i.ToString() + " - " + module.Name;
                        i++;

                        foreach (CommandInfo command in module.Commands)
                        {
                            valueList.Add(command.Name);
                        }
                        newField.Value = string.Join(", ", valueList);
                        fields.Add(newField);

                        newField = new EmbedFieldBuilder();
                        valueList = new List<string>();
                    }
                }
            }

            embedBuilder.Fields = fields;
            embed = embedBuilder.Build();

            await Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First().Message.ModifyAsync(x =>
            {
                x.Content = "";
                x.Embed = embed;
            });
        }

        //Logic for Stage Two of the Help Menu (Command Group from Category)
        public async Task ReactionMenuLogicStageTwo(SocketReaction reaction)
        {
            HelpState helpState = Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First().State;
            List<ModuleInfo> moduleList = new List<ModuleInfo>();
            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
            List<string> valueList = new List<string>();

            string userChoice = helpState.ToString();

            Embed embed;
            EmbedBuilder embedBuilder = new EmbedBuilder
            {
                Title = "Jaxxis Help Menu",
                Description = "--------------------------------------------------",
                Color = Color.Green,
            };

            EmbedFooterBuilder footer = new EmbedFooterBuilder
            {
                Text = $"{helpState.Footer} : <> = Required : {{}} = Optional"
            };

            EmbedFieldBuilder footerField = new EmbedFieldBuilder
            {
                Name = "--------------------------------------------------",
                Value = "<> = Required : {} = Optional " + Environment.NewLine
                + "Underlined Optional Parameters signify the default parameter.",
            };

            int i = 1;

            foreach (var command in Global.commands.Commands)
            {
                if (command.Remarks != null && command.Remarks.ToLower() == userChoice)
                {
                    fields.Add(await DecodeSummarySimple(command, i));
                    i++;
                }
            }

            //fields.Add(footerField);

            embedBuilder.Fields = fields;
            embedBuilder.Footer = footer;
            embed = embedBuilder.Build();

            await Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First().Message.ModifyAsync(x =>
            {
                x.Content = "";
                x.Embed = embed;
            });
        }

        //Logic for Stage Three of the Help Menu (Specific Command from Command Group)
        public async Task ReactionMenuLogicStageThree(SocketReaction reaction, string userChoice)
        {
            HelpState helpState = Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First().State;

            Embed embed;
            EmbedBuilder embedBuilder = new EmbedBuilder
            {
                Title = "Jaxxis Help Menu",
                Description = "--------------------------------------------------",
                Color = Color.Green,
            };

            EmbedFooterBuilder footer = new EmbedFooterBuilder
            {
                Text = "<> = Required : {} = Optional"
            };

            EmbedFieldBuilder footerField = new EmbedFieldBuilder
            {
                Name = "--------------------------------------------------",
                Value = "<> = Required : {} = Optional " + Environment.NewLine + "Underlined Optional Parameters signify the default parameter.",
            };

            foreach (var command in Global.commands.Commands)
            {
                if (command.Name.ToLower() == userChoice)
                {
                    embedBuilder.Fields.AddRange(await DecodeSummaryAdvanced(command.Summary));
                }
            }

            //embedBuilder.Fields.Add(footerField);
            embedBuilder.Footer = footer;
            embed = embedBuilder.Build();

            await Global.HelpMessageCache.Where(x => x.Message.Id == reaction.MessageId).First().Message.ModifyAsync(x =>
            {
                x.Content = "";
                x.Embed = embed;
            });
        }

        //Module Object for the Help Menu Below
        public class Module
        {
            public string Name { get; set; }
            public string Remark { get; set; }
        }

        //Create the start embed for Help Menu
        public void ReactionMenuStartEmbed()
        {

            List<Module> Modules = new List<Module>();

            foreach (ModuleInfo mod in Global.commands.Modules)
            {
                Module module = new Module();

                if (mod.Name != null && mod.Remarks != null)
                {
                    module.Name = mod.Name;
                    module.Remark = mod.Remarks.ToLower();

                    if (!Modules.Contains(module))
                    {
                        Modules.Add(module);
                    }
                }
            }

            EmbedBuilder embedBuilder = new EmbedBuilder
            {
                Title = "Jaxxis Help Menu",
                Description = "--------------------------------------------------",
                Color = Color.Green,
            };

            EmbedFieldBuilder field = new EmbedFieldBuilder();
            List<EmbedFieldBuilder> fieldList = new List<EmbedFieldBuilder>();

            List<string> completedRemarks = new List<string>();
            List<string> fieldValues = new List<string>();

            foreach (var mod in Modules)
            {
                if (!completedRemarks.Contains(mod.Remark))
                {
                    completedRemarks.Add(mod.Remark);
                    field.Name = mod.Remark;
                    field.Name = completedRemarks.Count().ToString() + " - " + field.Name.First().ToString().ToUpper() + field.Name.Substring(1);

                    foreach (var m in Modules)
                    {
                        if (m.Remark == mod.Remark)
                        {
                            fieldValues.Add(m.Name);
                        }
                    }

                    field.Value = String.Join(", ", fieldValues);
                    fieldList.Add(field);
                }

                field = new EmbedFieldBuilder();
                fieldValues = new List<string>();
            }

            embedBuilder.Fields = fieldList;
            startEmbed = embedBuilder.Build();
        }

        //Create the exit embed for Help Menu
        public void ReactionMenuExitEmbed()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder
            {
                Title = "Jaxxis Help Menu",
                Description = "--------------------------------------------------",
                Color = Color.Green,
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        Name = "This Jaxxis Help Menu has expired!",
                        Value = "Type !help to get a new instance of the Jaxxis Help Menu!"
                    }
                }
            };

            exitEmbed = embedBuilder.Build();
        }

        //Runs startup for the Reaction Help Menu
        public async Task ReactionMenuStart(CommandContext context)
        {
            try
            {
                await ReactionMenuGarbageCollection();
                IUserMessage message = await context.User.SendMessageAsync("", embed: startEmbed);

                int delay = 1000;
                Global.HelpMessageCache.Add(new HelpMessage(message, HelpState.START, DateTime.UtcNow));

                await message.AddReactionAsync(Global.emoji1);
                await Task.Delay(delay);
                await message.AddReactionAsync(Global.emoji2);
                await Task.Delay(delay);
                await message.AddReactionAsync(Global.emoji3);
                await Task.Delay(delay);
                /*
                await message.AddReactionAsync(Global.emoji4);
                await Task.Delay(delay);
                await message.AddReactionAsync(Global.emoji5);
                await Task.Delay(delay);
                await message.AddReactionAsync(Global.emoji6);
                await Task.Delay(delay);
                await message.AddReactionAsync(Global.emoji7);
                await Task.Delay(delay);
                await message.AddReactionAsync(Global.emoji8);
                await Task.Delay(delay);
                await message.AddReactionAsync(Global.emoji9);
                await Task.Delay(delay);
                */
                await message.AddReactionAsync(Global.emojiback);
                await Task.Delay(delay);
                await message.AddReactionAsync(Global.emojitrash);
            }
            catch (Exception ex)
            {
                await Global.LogError(ex);
            }
        }

        //Splits string; ~ = NewLine; - = Split
        public async Task<EmbedFieldBuilder> DecodeSummarySimple(CommandInfo command, int count)
        {
            try
            {
                string desc = command.Summary.Replace("~", Environment.NewLine);
                string[] aliasArray = desc.Split('-');
                EmbedFieldBuilder field = new EmbedFieldBuilder
                {
                    Name = count.ToString() + " - " + command.Name,
                    Value = aliasArray[0],
                };

                return field;
            }
            catch (Exception ex)
            {
                await Global.LogError(ex);
                return new EmbedFieldBuilder();
            }
        }

        //Splits string; ~ = NewLine; - = Split
        public async Task<List<EmbedFieldBuilder>> DecodeSummaryAdvanced(string desc)
        {
            try
            {
                desc = desc.Replace("~", Environment.NewLine);
                string[] aliasArray = desc.Split('-');
                List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder()
                    {
                        Name = aliasArray[0],
                        Value = aliasArray[1],
                    },

                    new EmbedFieldBuilder()
                    {
                        Name = aliasArray[2],
                        Value = "--------------------"
                    }
                };

                return fields;
            }

            catch (Exception ex)
            {
                await Global.LogError(ex);
                return new List<EmbedFieldBuilder>();
            }
        }
    }
}
