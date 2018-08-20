using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebChat.Models;

namespace WebChat.Abstract
{
    public interface ISessionsRepository
    {
        IEnumerable<UserSession> GetSessions();
        void AddSession(UserSession session);
        void RemoveSession(UserSession session);
        UserSession GetUserSession(string userName);
    }
}