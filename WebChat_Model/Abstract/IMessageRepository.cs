using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebChat_Model.Entities;

namespace WebChat_Model.Abstract
{
    public interface IMessageRepository
    {
        Message[] getMessagesFromUser(string currentUserName, string userName);
        User[] getSendersByUser(string userName);
        void PostMessage(Message msg);
        User getUser(string userName, string password);
        void SaveChanges();
    }
}
