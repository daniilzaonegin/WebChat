using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebChat_Model.Entities;

namespace WebChat.Models
{
    public class ChatDialogViewModel
    {
        public string CurrentUser { get; set; }
        public string DialogUser { get; set; }
        public Message[] Messages { get; set; }
    }
}