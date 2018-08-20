using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebChat_Model.Abstract;
using WebChat_Model.Entities;

namespace WebChat_Model.Concrete
{
    public class EFMessageRepository : IMessageRepository
    {
        private EFDbContext context;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public EFMessageRepository() {
            context = new EFDbContext();
            context.Database.Log = logger.Info;
            //DataBaseLogger logger = new DataBaseLogger(@"C:\temp\efLog.txt");
            //logger.Setup();
            //context.Database.Log = logger.LogToFile;
        }
        public Message[] getMessagesFromUser(string currentUsername, string userName) {
            var result = context.Messages.Where(m => ((m.To == currentUsername && m.From == userName) || (m.From == currentUsername && m.To == userName)));
            if (result.Any())
            {
                return result.OrderBy(m => m.MessageId).Take(100).ToArray();
            }
            else
            {
                return new Message[0];
            }
        }

        public User[] getSendersByUser(string userName) {
            return context.Users.Where(u => (context.Messages.Where(m => m.To == userName || m.From == userName).Count() > 0)).ToArray();
        }

        public User getUser(string name, string password) {
            return context.Users.Where(u =>(u.Name==name && u.Password==password)).FirstOrDefault();
        }

        public void PostMessage(Message msg) {
            context.Messages.Add(msg);
            context.SaveChanges();
        }

        public void SaveChanges() {
            context.SaveChanges();
        }
    }
}
