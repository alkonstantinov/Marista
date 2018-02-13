using Marista.Common.Tools;
using Marista.Common.ViewModels;
using Marista.DL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Marista.Site.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductService _ps = new ProductService();

        private void FixPrices(ShopVM shop)
        {
            if (Session["IsBP"] == null || !(bool)Session["IsBP"])
                return;
            foreach (var good in shop.Products)
                good.Price = good.Price * 0.77M;
        }
        public ActionResult Index()
        {
            ShopVM shop = new ShopVM();
            _ps.GetShopContent(shop);
            FixPrices(shop);
            return View(shop);
        }

        [HttpPost]
        public ActionResult Search(ShopVM shop)
        {
            _ps.GetShopContent(shop);
            FixPrices(shop);
            return View("Index", shop);
        }

        public async Task<ActionResult> AddToCart(int productId, int? times)
        {
            if (!times.HasValue)
                times = 1;
            bool added = false;
            CartVM cart = null;
            if (Session["Cart"] != null)
            {
                cart = (CartVM)Session["Cart"];

                foreach (var p in cart.Products)
                    if (p.ProductId == productId)
                    {
                        added = true;
                        p.Quantity += times.Value;
                    }
            }
            else
            {
                cart = new CartVM();
                cart.Products = new List<SaleDetailVM>();
                Session.Add("Cart", cart);
            }

            if (!added)
            {
                var product = await _ps.Get(productId);
                SaleDetailVM detail = new SaleDetailVM()
                {
                    Price = (Session["IsBP"] == null || !(bool)Session["IsBP"]) ? product.Price : product.Price * 0.77M,
                    ProductId = productId,
                    Discount = 0,
                    ProductName = product.Name,
                    Quantity = times.Value,
                    Weight = product.Weight
                };
                cart.Products.Add(detail);
            }
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult Opportunity()
        {
            return View();
        }

        public ActionResult Benefits()
        {
            return View();
        }

        public ActionResult BusinessModel()
        {
            return View();
        }

        public ActionResult MaristaCosmetics()
        {
            return View();
        }

        public ActionResult PaymentAndShipping()
        {
            return View();
        }

        public ActionResult TermsOfUse()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendFeedback(FeedbackVM data)
        {
            string content = "From:" + data.Name + " " + data.Email + "<br/>" + data.Message;
            new Common.Tools.Mailer().SendMailSpecific(
                content,
                ConfigurationManager.AppSettings["ToEmail"],
                data.Subject);

            return Json("ok", JsonRequestBehavior.AllowGet);
        }


    }
}