using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatServerWeb.Model.Entity
{
    public class ChatMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string MessageId { get; set; }
        public string AppId { get; set; }
        public string Username { get; set; }

        public string UserEmail { get; set; }

        public string Message { get; set; }
        public int GroupType { get; set; } // private, general
        public string GroupId { get; set; }
        public DateTime DateCreated { get; set; }
        public int Status { get; set; } //deleted
        [ForeignKey("AppId")]
        public ChatApplication ChatApplication { get; set; }
        [ForeignKey("GroupId")]
        public ChatGroup ChatGroup { get; set; }
    }
}
