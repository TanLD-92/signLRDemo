using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DemoEntityCodeFirst.Models
{
    public class UserGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Group")]
        public int GroupId { get; set; }

        public virtual User User { get; set; }
        public virtual Group Group { get; set; }
    }
}