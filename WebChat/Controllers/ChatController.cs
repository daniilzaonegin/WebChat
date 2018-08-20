using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebSockets;
using WebChat.Abstract;
using WebChat.Models;
using Newtonsoft.Json.Linq;
using WebChat_Model.Abstract;
using WebChat_Model.Entities;
using System.Diagnostics;
using System.Globalization;

namespace WebChat.Controllers
{
    public class ChatController : Controller
    {
        private ISessionsRepository sessRepo;

        private IMessageRepository msgRepo;

        public ChatController(ISessionsRepository repo, IMessageRepository msgs) {
            sessRepo = repo;
            msgRepo = msgs;
        }
        
        public ActionResult ChatDialog(string chatUser) {
            
            UserSession anotherUserSession = sessRepo.GetUserSession(chatUser);
            
            //получение claim ассоциированных с текущим юзером
            ClaimsIdentity claimsIdentity;
            claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            string currentUser = claimsIdentity.Name;

            return View(new ChatDialogViewModel()
            {
                CurrentUser = currentUser,
                DialogUser = chatUser,
                Messages = msgRepo.getMessagesFromUser(currentUser, chatUser)
            });           
        }

        [HttpGet]
        public void ProcessRequest() {

            if (HttpContext.IsWebSocketRequest)
                HttpContext.AcceptWebSocketRequest(WebSocketRequest);
        }

        private async Task WebSocketRequest(AspNetWebSocketContext context) {
            //получаем сокет клиента из контекста запроса
            WebSocket socket = context.WebSocket;

            //получение claim ассоциированных с юзером
            ClaimsIdentity claimsIdentity;
            claimsIdentity = context.User.Identity as ClaimsIdentity;
            string currentUser = claimsIdentity.Name;
            //получаем объект сессии пользователя
            UserSession session = sessRepo.GetUserSession(currentUser);
            session.UserSocket = socket;
            
            //слушаем сокет
            while(true)
            {
                //(1000 символов сообщении)*2 + (50 символов имя пользователя)*2 + JSON обертка 20 символов + резерв
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[2200]);
                
                //ожидаем данные от клиента                
                WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                if (result.CloseStatus == null) // сообщения о закрытии соединения не передаем
                {
                    string data = Encoding.UTF8.GetString(buffer.Array);

                    JObject linkObj = JObject.Parse(data);
                    //msg.Append(DateTime.Now.ToString(@"[dd/MM/yyyy HH:mm:ss]")).Append(" " + claimsIdentity.Name + ":");
                    //максимальная длина сообщения 1000 символов
                    //msg.Append();

                    //50 символов максимальная длина имени пользователя
                    string to = linkObj["to"].ToString().Substring(0,Math.Min(linkObj["to"].ToString().Length, 50));

                    Message dbMessage = new Message {
                                            From = currentUser, To = to,
                                            //максимальная длина сообщения 1000 символов
                                            Text = linkObj["msg"].ToString().Substring(0, Math.Min(linkObj["msg"].ToString().Length, 1000))
                                            };

                    msgRepo.PostMessage(dbMessage);

                    Debug.Write($"Message Id:{dbMessage.MessageId}");
                    Debug.Write($"| Message TimeStamp:{dbMessage.Timestamp}");
                    Debug.Write($"| Message From:{dbMessage.From}");
                    //Передаем сообщение получателю
                    StringBuilder msg = new StringBuilder();
                    msg.Append(dbMessage.Timestamp.ToString(@"[dd/MM/yyyy HH:mm:ss]", CultureInfo.InvariantCulture)).Append(" ").Append(currentUser).Append(":").Append(dbMessage.Text.ToString());

                    UserSession[] clientsToSend = new UserSession[] { sessRepo.GetUserSession(to), session };

                    foreach (UserSession us in clientsToSend)
                    {
                        WebSocket client = us.UserSocket;
                        if (client != null)
                        {
                            try
                            {
                                if (client.State == WebSocketState.Open)
                                {                                    
                                    List<byte> toSend = new List<byte>(Encoding.UTF8.GetBytes(msg.ToString()).ToArray());
                                    ArraySegment<byte> sendBuffer = new ArraySegment<byte>(toSend.Take(2200).ToArray());
                                    await client.SendAsync(sendBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                                }
                            }
                            catch (ObjectDisposedException)
                            {
                                client = null;
                            }
                        }
                    }
                }
            }
        }
    }
}