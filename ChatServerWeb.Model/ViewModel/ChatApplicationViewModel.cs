using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ChatServerWeb.Model.ViewModel
{
    public class ChatApplicationViewModel
    {
        public string AppId { get; set; }
        [Required]
        public string AppName { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
    }
}
