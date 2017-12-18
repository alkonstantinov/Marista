using Marista.Admin.ViewModels;
using Marista.Common.Models;
using Marista.Common.Tools;
using Marista.DL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Marista.Admin.Controllers
{
    public class UserController : BaseController
    {
        private readonly MaristaEntities db = new MaristaEntities();

        public UserController()
        {
        }

        [HttpGet]
        public ActionResult Login()
        {
            if(this.UserData != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginVM login)
        {
            string hashedPassword = MD5.ConvertToMD5(login.Password);
            var user = await db.SiteUsers.SingleOrDefaultAsync(u => u.Username == login.Username && u.Password == hashedPassword);
            if(user == null)
            {
                ModelState.AddModelError(string.Empty, "Incorrect username and/or password");
                return View();
            }

            this.UserData = new UserData()
            {
                UserId = user.SiteUserId,
                Username = user.Username,
                LevelId = user.LevelId
            };
            return RedirectToAction("Index", "Dashboard");
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