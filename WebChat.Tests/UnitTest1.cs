using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebChat_Model.Abstract;
using WebChat_Model.Entities;
using WebChat.Controllers;
using WebChat.Models;
using WebChat.Abstract;
using System.Collections.Generic;
using Microsoft.Owin;
using Owin;
using System.Net.Http;

namespace WebChat.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAuthorisation() {
            //Организация - создание имитации хранилища сессий
            List<UserSession> sess = new List<UserSession>();
            Mock<ISessionsRepository> sessRepoMock = new Mock<ISessionsRepository>();
            sessRepoMock.Setup(m => m.GetSessions())
                .Returns(new UserSession[] { new UserSession { Name = "Test", Status = "Online" } });
            sessRepoMock.Setup(m => m.AddSession(It.IsAny<UserSession>()))
                .Callback((UserSession session)=>sess.Add(session));

            //Организация - создание имитации хранилища пользователей
            Mock<IMessageRepository> repoMock = new Mock<IMessageRepository>();
            repoMock.Setup(m => m.getUser("Danila", "Test"))
                .Returns(new User { Name = "Danila", Email = "zaoneg@gmail.com", Password = "Test", Surname = "Zaonegin" });
            
            //Организация - создание контроллера
            AuthController target = new AuthController(repoMock.Object, sessRepoMock.Object);             
            LogInModel model = new LogInModel { UserName = "Danila", Password = "Test" };
            
            //Действие
            target.LogIn(model);

            //Утверждение
            Assert.AreEqual(sess[0].Name,"Danila");
            Assert.AreEqual(sess[0].Status, "Online");
        }
    }
}
