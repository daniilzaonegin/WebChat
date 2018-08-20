using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using WebChat.Abstract;
using WebChat.Models;
using WebChat_Model.Abstract;
using WebChat_Model.Entities;

namespace WebChat.Controllers
{
    public class HomeController : Controller
    {
       private ISessionsRepository sessRepo;
       public HomeController( ISessionsRepository sessRepository) {
            sessRepo = sessRepository;        
       }
        public ActionResult UserDashBoard() {
            ClaimsIdentity claimsIdentity;
            claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;

            string currentUserName = claimsIdentity.Name;

            return View(new UserViewModel { CurrentUserName = currentUserName, OnlineUsers = sessRepo.GetSessions() });
        }
    }
}