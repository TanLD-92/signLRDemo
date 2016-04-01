using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DemoEntityCodeFirst.Models
{
    public class Skill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string NameSkill { get; set; }
        public int MarkSkill { get; set; }
        [ForeignKey("User")]
        public int PersonId { get; set; }

        public virtual User User { get; set; }
    }
}