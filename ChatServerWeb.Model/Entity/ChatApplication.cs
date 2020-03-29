using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatServerWeb.Model.Entity
{
    public class ChatApplication
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string AppId { get; set; }
        public string AppName { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }

        ICollection<ChatApplicationUser> ChatApplicationUsers { get; set; }
        ICollection<ChatMessage> ChatMessages { get; set; }

    }
}
