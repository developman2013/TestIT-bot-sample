using Microsoft.EntityFrameworkCore;
using ProactiveBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProactiveBot.Data
{
    public class BotContext : DbContext
    {
        public DbSet<BotKeysModel> BotKeys { get; set; }

        public BotContext(DbContextOptions<BotContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
