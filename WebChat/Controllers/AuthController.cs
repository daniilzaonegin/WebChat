using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
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
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private IMessageRepository repo;
        private ISessionsRepository sessRepo;
        public AuthController(IMessageRepository repositoryParam, ISessionsRepository sessionsRepository) {
            repo = repositoryParam;
            sessRepo = sessionsRepository;
        }

        // GET: Auth
        // returns login form
        [HttpGet]
        public ActionResult LogIn(string returnUrl)
        {
            LogInModel model = new LogInModel {
                ReturnUrl = returnUrl
            };

            return View(model);
        }
        
        [HttpPost]
        public ActionResult LogIn (LogInModel model) {
            if(!ModelState.IsValid) {
                return View();
            }

            User currentUser = repo.getUser(model.UserName, model.Password);

            if(!(currentUser == null)) {
                if(sessRepo.GetUserSession(currentUser.Name)==null)
                    //пользователь еще не залогинен
                    sessRepo.AddSession(new UserSession { Name = currentUser.Name, Status = "Online" });
                ClaimsIdentity identity = new ClaimsIdentity(
                    new[] {
                        new Claim(ClaimTypes.Name,currentUser.Name),
                        new Claim(ClaimTypes.Email,currentUser.Email)
                    },
                    "ApplicationCookie");
                
                IOwinContext ctx = Request.GetOwinContext();
                IAuthenticationManager authManager = ctx.Authentication;

                authManager.SignIn(identity);

                return Redirect(GetRedirectUrl(model.ReturnUrl));
            }

            ModelState.AddModelError("", "Invalid user name or password");

            return View();
        }

        public ActionResult LogOut() {

            IOwinContext ctx = Request.GetOwinContext();
            IAuthenticationManager authManager = ctx.Authentication;

            //получение текущего пользователя
            ClaimsIdentity claimsIdentity;
            claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            string currentUserName = claimsIdentity.Name;

            authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            sessRepo.RemoveSession(new UserSession { Name = currentUserName });

            return RedirectToAction("UserDashBoard", "Home");

        }

        private string GetRedirectUrl(string returnUrl) {
            if(string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl)) {
                return Url.Action("UserDashBoard","Home");
            }

            return returnUrl;
        }
    }
}