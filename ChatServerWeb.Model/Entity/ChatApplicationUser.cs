using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatServerWeb.Model.Entity
{
    public class ChatApplicationUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string UserId { get; set; }
        public string AppId { get; set; }
        public string GroupId { get; set; }      
        public string Username { get; set; }  
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
        public int Status  { get; set; } //Banned, Suspended

        [ForeignKey("AppId")]
        public ChatApplication ChatApplication { get; set; }

        [ForeignKey("GroupId")]
        public ChatGroup ChatGroup { get; set; }

        ICollection<ChatMessage> ChatMessages { get; set; }

    }
}
