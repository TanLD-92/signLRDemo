using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DemoEntityCodeFirst.Models
{
    public class ManagerDbContext: DbContext
    {
        public ManagerDbContext(): base("SignLRDemoVersionOne")
        {
            //Database.SetInitializer<ManagerDbContext>(null);
        }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<MessageBox> MessageBoxes { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
    }
}