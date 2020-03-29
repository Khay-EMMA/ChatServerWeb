using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatServerWeb.Model.Entity
{
    public class ChatGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedByUser { get; set; }
        public string AppId { get; set; }
        public int NumberOfMembers { get; set; }


        [ForeignKey("AppId")]
        public ChatApplication ChatApplication { get; set; }
        ICollection<ChatMessage> ChatMessages { get; set; }
        ICollection<ChatGroup> ChatGroups { get; set; }
    }
}
