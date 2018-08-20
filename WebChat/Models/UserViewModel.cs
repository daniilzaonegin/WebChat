using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebChat_Model.Entities;

namespace WebChat.Models
{
    public class UserViewModel
    {
        public string CurrentUserName { get; set; }
        public IEnumerable<UserSession> OnlineUsers { get; set; }
    }
}