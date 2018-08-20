using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebChat.Models {
    public class LogInModel {

        [Required(ErrorMessage ="Please enter your user name!")]       
        public string UserName { get; set; }

        [Required(ErrorMessage ="Please enter password, empty password is not permitted!")]
        public string Password { get; set; }

        [HiddenInput]
        public string ReturnUrl { get; set; }
    }
}