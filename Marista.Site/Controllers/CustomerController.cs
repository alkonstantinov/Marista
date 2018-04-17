using Marista.Common.ViewModels;
using Marista.DL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
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
        public ActionResult Login(bool backToCheckout = false)
        {
            return View(new CustomerVM() { BackToCheckout = backToCheckout });
        }

        [HttpPost]
        public ActionResult Login(CustomerVM model)
        {
            var cmr = db.Login(model);
            if (cmr == null)
            {
                ViewBag.LoginError = true;
                return View(model);
            }
            else
            {
                Session["CustomerId"] = cmr.CustomerId;
                Session["CustomerName"] = cmr.CustomerName;
                Session["IsBP"] = cmr.BPId.HasValue;
                if (cmr.BPId.HasValue && Session["Cart"] != null)
                {
                    CartVM cart = (CartVM)Session["Cart"];
                    foreach (var good in cart.Products)
                        good.Price = good.Price * 0.77M;

                }
                if (model.BackToCheckout)
                    return RedirectToAction("Checkout", "Cart");
                else
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
                Parallel.Invoke(() =>
                {
                    Common.Tools.Mailer.SendMailSpecific(content, model.Username,"Your password is changed");
                });
                
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


        [HttpGet]
        public ActionResult Invoice(int saleId)
        {
            return View(db.GetSale(saleId));
        }

        string tempPath = Path.GetTempPath();
        string GetPdf(int saleId)
        {
            string wkhtmlPath = ConfigurationManager.AppSettings["PATHTOWKHTMLTOPDF"];
            string url = ConfigurationManager.AppSettings["URLTOINV"];
            string printUrl = url + "?saleId=" + saleId;
            string pdf_path = Path.Combine(tempPath, saleId.ToString() + ".pdf");

            ProcessStartInfo psi = new ProcessStartInfo();

            psi.FileName = wkhtmlPath;
            psi.Arguments = "  \"" + printUrl + "\" \"" + pdf_path + "\"";
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            using (Process process = new Process())
            {
                process.StartInfo = psi;
                process.EnableRaisingEvents = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                process.WaitForExit();
                process.Close();
                string result = "";

                if (System.IO.File.Exists(pdf_path))
                {
                    result = pdf_path;
                    //File.Delete(pdf_path);
                }

                return result;

            }
        }

        [HttpGet]
        public ActionResult InvoicePdf(int saleId)
        {
            string fnm = this.GetPdf(saleId);
            var doc = System.IO.File.ReadAllBytes(fnm);
            System.IO.File.Delete(fnm);
            var fc = new FileContentResult(doc, "application/octet-stream");
            fc.FileDownloadName = fnm;
            return fc;
        }

        public ActionResult Logout()
        {

            Session.Remove("CustomerId");
            Session.Remove("IsBP");
            return RedirectToAction("Index", "Home");
        }
        public async Task<ActionResult> Menu()
        {
            return PartialView();
        }
    }
}