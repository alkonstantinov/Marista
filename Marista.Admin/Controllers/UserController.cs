using Marista.Common.Models;
using Marista.Common.Tools;
using Marista.Common.ViewModels;
using Marista.DL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ZXing;
using ZXing.Common;

namespace Marista.Admin.Controllers
{
    public class UserController : BaseController
    {

        public UserController()
        {
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (this.UserData != null)
            {
                if (this.UserData.LevelId == 1)
                    return RedirectToAction("Index", "Sales");
                else
                    return RedirectToAction("MyTeamReport", "Reports");

            }
            return View();
        }

        public ActionResult TestMail()
        {
            new Mailer().SendMailSpecific("kor", "alkonstantinov@outlook.com", "123");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await this.UserService.Login(login);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Incorrect username and/or password");
                return View();
            }

            this.UserData = user;
            await this.UserService.StoreSessionId(user.UserId, Session.SessionID);

            if (this.UserData.LevelId == 1)
                return RedirectToAction("Index", "Sales");
            else
                return RedirectToAction("MyTeamReport", "Reports");

        }

        [HttpGet]
        public ActionResult Logout()
        {
            this.UserData = null;
            Session.Clear();
            return RedirectToAction("Login");
        }

    }
}