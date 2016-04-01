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
        }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<User> Users { get; set; }
    }
}