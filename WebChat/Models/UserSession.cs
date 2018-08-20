using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Web;

namespace WebChat.Models {
    public class UserSession {
        public string Name { get; set; }
        public string Status { get; set; }

        public WebSocket UserSocket { get; set; }
        public override bool Equals(object obj) {
            
            //если параметр null вернуть false
            if (obj ==null) {
                return false;
            }
            
            //если объект не преобразуется к типу UserSession вернуть false
            UserSession sessionObj = obj as UserSession;
            if (sessionObj==null) {
                return false;
            }
            return sessionObj.Name==Name;
        }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }
    }
}