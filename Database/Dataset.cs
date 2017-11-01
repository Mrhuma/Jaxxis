using System;
using System.Collections.Generic;
using System.Linq;
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

        public class DbJaxxis : LinqToDB.Data.DataConnection
        {
            public DbJaxxis() : base("Jaxxis") { }

            public ITable<Guild> Guildlist { get { return GetTable<Guild>(); } }
            public ITable<User> Userlist { get { return GetTable<User>(); } }
        }

        public static void CreateGuildList()
        {
            using (var db = new DbJaxxis())
            {
                try { db.CreateTable<Guild>(); } catch { Console.WriteLine($"Caught a failed creation of the guildlist table"); }
            }
        }

        public static void CreateUserList()
        {
            using (var db = new DbJaxxis())
            {
                try
                {
                    db.CreateTable<User>();
                }
                catch
                {
                    Console.WriteLine($"Caught a failed creation of the user table");
                }
            }
        }

        public static void InsertGuild(Guild guild)
        {
            using (var db = new DbJaxxis())
            {
                db.Insert(guild);
            }
        }

        public static void InsertUser(User user)
        {
            using (var db = new DbJaxxis())
            {
                db.Insert(user);
            }
        }

        public static void GuildInsertOrUpdate(Guild guild)
        {
            using (var db = new DbJaxxis())
            {
                db.GetTable<Guild>()
                    .InsertOrUpdate(
                        () => new Guild
                        {
                            Guildid = guild.Guildid,
                            Guildname = guild.Guildname,
                            Usercount = guild.Usercount,
                            IsActive = guild.IsActive,
                        },
                        t => new Guild
                        {
                            Guildname = guild.Guildname,
                            Usercount = guild.Usercount,
                            IsActive = guild.IsActive,
                        });
            }
        }

        public static void UserInsertOrUpdate(User user)
        {
            using (var db = new DbJaxxis())
            {
                db.GetTable<User>()
                    .InsertOrUpdate(
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

        public static void IsActiveFalse()
        {
            using (var db = new DbJaxxis())
            {
                db
                    .GetTable<Guild>()
                    .Where(t => t.Guildid != "")
                    .Update(t => new Guild
                    {
                        IsActive = false,
                    });
            }
        }

    }
}