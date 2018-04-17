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

        [HttpGet]
        public ActionResult LostPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LostPassword(LoginVM model)
        {

            Random r = new Random(DateTime.Now.Millisecond);
            string password = r.Next(10000).ToString().PadRight(5, '0');
            model.Password = Common.Tools.MD5.ConvertToMD5(password);
            bool result = await this.UserService.ChangePass(model);
            if (result)
            {
                string content = System.IO.File.ReadAllText(Server.MapPath("/Mails/resetpass.txt"));
                content = content.Replace("{username}", model.Username);
                content = content.Replace("{password}", password);

                Parallel.Invoke(() =>
                {
                    Common.Tools.Mailer.SendMailSpecific(content, 
                        model.Username,
                "Your password is changed");
                });
                
            }

            ViewBag.ok = true;


            return View();
        }

        public ActionResult TestMail()
        {
            
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