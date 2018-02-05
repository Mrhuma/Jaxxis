using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Mapping;

namespace Jaxxis.Database
{
    class Dataset
    {
        [Table(Name = "guildlist")]
        public class Guild
        {
            [Column(Length = 100), NotNull, PrimaryKey]
            public string Guildid { get; set; }

            [Column(Length = 100), NotNull]
            public string Guildname { get; set; }

            [Column, NotNull]
            public int Usercount { get; set; }

            [Column, NotNull]
            public bool IsActive { get; set; }
        }

        [Table(Name = "userlist")]
        public class User
        {
            [Column(Length = 100), NotNull, PrimaryKey]
            public string UserID { get; set; }

            [Column(Length = 100), NotNull]
            public string Username { get; set; }
        }

        [Table(Name = "errorlog")]
        public class ErrorLog
        {
            [Column, NotNull, Identity, PrimaryKey]
            public int Id { get; set; }
            
            [Column(Length = 100), Nullable]
            public string User { get; set; }

            [Column(Length = 100), Nullable]
            public string Message { get; set; }

            [Column(DataType = DataType.Text), Nullable]
            public string StackTrace { get; set; }
        }

        [Table(Name = "messagelog")]
        public class MessageLog
        {
            [Column, NotNull, Identity, PrimaryKey]
            public int Id { get; set; }

            [Column, NotNull]
            public string Message { get; set; }

            [Column, NotNull]
            public DateTime Time { get; set; }
        }

        [Table(Name = "stats")]
        public class Stats
        {
            [Column, NotNull, PrimaryKey]
            public string Name { get; set; }

            [Column, NotNull]
            public int Count { get; set; }
        }

        public class DbJaxxis : LinqToDB.Data.DataConnection
        {
            public DbJaxxis() : base("Jaxxis") { }

            public ITable<Guild> Guildlist { get { return GetTable<Guild>(); } }
            public ITable<User> Userlist { get { return GetTable<User>(); } }
            public ITable<ErrorLog> ErrorLog { get { return GetTable<ErrorLog>(); } }
            public ITable<MessageLog> MessageLog { get { return GetTable<MessageLog>(); } }
            public ITable<Stats> Stats { get { return GetTable<Stats>(); } }
        }

        private static async Task CreateGuildList()
        {
            using (var db = new DbJaxxis())
            {
                try
                {
                    await db.CreateTableAsync<Guild>();
                }
                catch
                {
                    Console.WriteLine($"Caught a failed creation of the Guildlist table");
                }
            }
        }

        private static async Task CreateUserList()
        {
            using (var db = new DbJaxxis())
            {
                try
                {
                    await db.CreateTableAsync<User>();
                }
                catch
                {
                    Console.WriteLine($"Caught a failed creation of the Userlist table");
                }
            }
        }

        private static async Task CreateErrorLog()
        {
            using (var db = new DbJaxxis())
            {
                try
                {
                    await db.CreateTableAsync<ErrorLog>();
                }
                catch
                {
                    Console.WriteLine($"Caught a failed creation of the Error Log table");
                }
            }
        }

        private static async Task CreateMessageLog()
        {
            using (var db = new DbJaxxis())
            {
                try
                {
                    await db.CreateTableAsync<MessageLog>();
                }
                catch
                {
                    Console.WriteLine($"Caught a failed creation of the Message Log table");
                }
            }
        }

        private static async Task CreateStats()
        {
            using (var db = new DbJaxxis())
            {
                try
                {
                    await db.CreateTableAsync<Stats>();
                }
                catch
                {
                    Console.WriteLine($"Caught a failed creation of the Stats table");
                }
            }
        }

        public static async Task CreateAllTables()
        {
            await CreateGuildList();
            await CreateUserList();
            await CreateErrorLog();
            await CreateMessageLog();
            await CreateStats();
        }

        public static async Task InsertGuild(Guild guild)
        {
            using (var db = new DbJaxxis())
            {
                await db.InsertAsync(guild);
            }
        }

        public static async Task InsertUser(User user)
        {
            using (var db = new DbJaxxis())
            {
                await db.InsertAsync(user);
            }
        }

        public static async Task InsertErrorLog(ErrorLog errorLog)
        {
            using (var db = new DbJaxxis())
            {
                await db.InsertAsync(errorLog);
            }
        }

        public static async Task InsertMessageLog(MessageLog messageLog)
        {
            using (var db = new DbJaxxis())
            {
                await db.InsertAsync(messageLog);
            }
        }

        private static async Task InsertStats(Stats stats)
        {
            using (var db = new DbJaxxis())
            {
                await db.InsertAsync(stats);
            }
        }

        public static async Task GuildInsertOrUpdate(Guild guild)
        {
            using (var db = new DbJaxxis())
            {
                await db.GetTable<Guild>()
                    .InsertOrUpdateAsync(
                        () => new Guild
                        {
                            Guildid = guild.Guildid,
                            Guildname = guild.Guildname,
                            Usercount = guild.Usercount,
                            IsActive = true,
                        },
                        t => new Guild
                        {
                            Guildname = guild.Guildname,
                            Usercount = guild.Usercount,
                            IsActive = true,
                        });
            }
        }

        public static async Task StatsInsertOrUpdate(string name)
        {
            Stats newStats = new Stats
            {
                Name = name
            };

            try
            {
                newStats.Count = await StatsGetCount(newStats) + 1;
            }
            catch
            {
                newStats.Count = 1;
                await InsertStats(newStats);
                return;
            }

            if (newStats.Count != 0)
            {
                await StatsUpdate(newStats);
            }
        }
        
        private static async Task StatsUpdate(Stats stats)
        {
            using (var db = new DbJaxxis())
            {
                await db.UpdateAsync(stats);
            }
        }

        private static async Task<int> StatsGetCount(Stats stats)
        {
            using (var db = new DbJaxxis())
            {
                var query = from s in db.Stats
                            where s.Name == stats.Name
                            select s.Count;

                return await query.FirstAsync();
            }
        }

        public static async Task UserInsertOrUpdate(User user)
        {
            using (var db = new DbJaxxis())
            {
                await db.GetTable<User>()
                    .InsertOrUpdateAsync(
                        () => new User
                        {
                            UserID = user.UserID,
                            Username = user.Username,
                        },
                        t => new User
                        {
                            Username = user.Username,
                        });
            }
        }

        public static async Task IsActiveFalse()
        {
            using (var db = new DbJaxxis())
            {
                await db
                    .GetTable<Guild>()
                    .Where(t => t.Guildid != "")
                    .UpdateAsync(t => new Guild
                    {
                        IsActive = false,
                    });
            }
        }
    }
}