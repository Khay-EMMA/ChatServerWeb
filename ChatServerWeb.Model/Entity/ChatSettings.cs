using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatServerWeb.Model.Entity
{
    public class ChatSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string SettingsId { get; set; }
        public int Language { get; set; }
        public DateTime DateCreated { get; set; }

    }
}
