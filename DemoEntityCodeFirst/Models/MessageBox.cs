using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DemoEntityCodeFirst.Models
{
    public class MessageBox
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int GroupId { get; set; }
        public string MessageContent { get; set; }

        public virtual Group Group { get; set; }
    }
}