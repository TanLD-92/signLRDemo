using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DemoEntityCodeFirst.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Division { get; set; }
        public int Experencde { get; set; }
        public virtual ICollection<Skill> Skills { get; set; } // Navigation property
        public virtual ICollection<UserGroup> UserGroups { get; set; }

    }
}