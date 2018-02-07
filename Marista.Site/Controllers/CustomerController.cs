using Marista.Common.ViewModels;
using Marista.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Marista.Site.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CustomerService db = new CustomerService();

        public async Task<ActionResult> Index()
        {
            if (Session["CustomerId"] == null)
                return RedirectToAction("login");
            var model = await db.Get((int)Session["CustomerId"]);

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePass(CustomerVM model)
        {
            if (Session["CustomerId"] == null)
                return RedirectToAction("login");

            if (ModelState.IsValid)
            {
                db.ChangePass(model);
                ViewBag.ok = true;

            }
            return View("Index", model);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View(new CustomerVM());
        }

        [HttpPost]
        public ActionResult Login(CustomerVM model)
        {
            int id = db.Login(model);
            if (id == -1)
            {
                ViewBag.LoginError = true;
                return View(model);
            }
            else
            {
                Session["CustomerId"] = id;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View(new CustomerVM());
        }

        [HttpPost]
        public ActionResult ResetPassword(CustomerVM model)
        {
            model.NewPassword = DateTime.Now.Millisecond.ToString().PadLeft(6, '0');
            bool sent = db.ResetPassword(model);
            if (sent)
            {
                string content = System.IO.File.ReadAllText(Server.MapPath("/Mails/resetpass.txt"));
                content = content.Replace("{username}", model.Username);
                content = content.Replace("{password}", model.NewPassword);

                new Common.Tools.Mailer().SendMailSpecific(
                content,
                model.Username,
                "Your password is changed");
            }
            ViewBag.sent = true;
            return View(new CustomerVM());
        }

        [HttpGet]
        public ActionResult GetSales()
        {
            if (Session["CustomerId"] == null)
                return RedirectToAction("login");
            return View(db.GetSales((int)Session["CustomerId"]));
        }

        [HttpGet]
        public ActionResult GetSale(int saleId)
        {
            if (Session["CustomerId"] == null)
                return RedirectToAction("login");
            return View(db.GetSale(saleId));
        }


    }
}