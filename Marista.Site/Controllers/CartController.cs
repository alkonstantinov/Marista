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

        public async Task<ActionResult> SetCoupon(string uniqueId)
        {
            CartVM cart = (CartVM)Session["Cart"];
            cart.CouponUniqueId = "";
            foreach (var p in cart.Products)
                p.Discount = 0;
            var c = db.GetCouponInfo(uniqueId);
            if (c == null)
                return Json("error", JsonRequestBehavior.AllowGet);
            cart.CouponUniqueId = uniqueId;

            List<ProductDiscountVM> result = new List<ProductDiscountVM>();
            foreach (var p in cart.Products)
            {
                ProductVM prd = await db.Get(p.ProductId);
                if (c.ForAll || c.HCategoryId == prd.HCategoryId || c.VCategoryId == prd.VCategoryId || c.ProductId == prd.ProductId)
                {
                    p.Discount = c.Discount;
                    result.Add(new ProductDiscountVM()
                    {
                        ProductId = prd.ProductId,
                        Discount = c.Discount
                    });

                }
            }
            return Json(result.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Index()
        {
            CartVM cart = new CartVM();
            cart.Products = new List<SaleDetailVM>();
            SaleDetailVM detail = new SaleDetailVM()
            {
                Price = 12,
                ProductId = 1,
                Discount = 0,
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