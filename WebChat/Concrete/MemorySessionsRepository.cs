using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using WebChat.Abstract;
using WebChat.Models;

namespace WebChat.Concrete
{
    public class MemorySessionsRepository : ISessionsRepository
    {
        private static List<UserSession> _sessions;
        private static readonly object padlock = new object();

        public MemorySessionsRepository() {
            if (_sessions == null)
                _sessions = new List<UserSession>();
        }
        public IEnumerable<UserSession> GetSessions() {
            lock (padlock)
            {
                return _sessions.ToArray();
            }
        }

        public void AddSession (UserSession session) {
            lock(padlock)
            {
                _sessions.Add(session);
            }
        }

        public void RemoveSession (UserSession session) {
            lock(padlock)
            {
                _sessions.Remove(session);
            }
        }

        public UserSession GetUserSession(string userName) {
            UserSession result;
            lock (padlock)
            {
                result =_sessions.Where(u => u.Name == userName).FirstOrDefault();
            }
            return result;
        }
    }
}