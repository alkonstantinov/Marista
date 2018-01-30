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
    public class CartController : Controller
    {
        ProductService db = new ProductService();
        public ActionResult Embed()
        {
            return PartialView();
        }

        public ActionResult ChangeAmmount(int productId, int step)
        {
            CartVM cart = (CartVM)Session["Cart"];
            foreach (var d in cart.Products)
                if (d.ProductId == productId)
                { 
                    d.Quantity += step;
                    break;
                }
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveProduct(int productId)
        {
            CartVM cart = (CartVM)Session["Cart"];
            SaleDetailVM p = null;
            foreach (var d in cart.Products)
                if (d.ProductId == productId)
                {
                    p = d;
                    break;
                }
            if (p != null)
                cart.Products.Remove(p);
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeCountry(string countryId)
        {
            CartVM cart = (CartVM)Session["Cart"];
            cart.CountryId = countryId;
            cart.CountryPrice = db.GetCountryDeliveryPrice(cart.CountryId);

            return Json(cart.CountryPrice, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> Index()
        {
            CartVM cart = new CartVM();
            cart.Products = new List<SaleDetailVM>();
            SaleDetailVM detail = new SaleDetailVM()
            {
                Price = 12,
                ProductId = 1,
                ProductName = "xxxxxxxxxxx",
                Quantity = 2
            };

            cart.Products.Add(detail);
            cart.Countries = db.GetCountries();
            cart.CountryId = "bg";
            cart.CountryPrice = db.GetCountryDeliveryPrice(cart.CountryId);

            Session.Add("Cart", cart);
            return View(cart);
        }
    }
}