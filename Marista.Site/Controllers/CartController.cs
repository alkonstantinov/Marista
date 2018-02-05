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
            CartVM cart = null;
            if (Session["Cart"] == null)
            {
                cart = new CartVM();
                cart.Products = new List<SaleDetailVM>();

                Session.Add("Cart", cart);
            }
            else
                cart = (CartVM)Session["Cart"];
            cart.Countries = db.GetCountries();
            cart.CountryId = "bg";
            cart.CountryPrice = db.GetCountryDeliveryPrice(cart.CountryId);

            return View(cart);
        }

        public ActionResult GetSmallCart()
        {
            SmallCartInfoVM result = new SmallCartInfoVM();
            if (Session["Cart"] != null)
            {

                CartVM cart = (CartVM)Session["Cart"];
                foreach (var p in cart.Products)
                {
                    result.ItemsCount += p.Quantity;
                    result.Price += p.Price * p.Quantity * (100 - p.Discount) / 100;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Checkout()
        {
            int customerId = -1;
            if (Session["CustomerId"] != null)
                customerId = (int)Session["CustomerId"];
            var model = db.GetLastCheckoutData(customerId);
            model.Countries = db.GetCountries();
            CartVM cart = (CartVM)Session["Cart"];
            model.BillingCountryId = cart.CountryId;
            model.DeliveryCountryId = cart.CountryId;
            model.Details = cart.Products;
            model.DeliveryPrice = cart.CountryPrice;
            return View(model);
        }

        [HttpPost]
        public ActionResult Checkout(CheckoutVM model)
        {
            model.Countries = db.GetCountries();
            CartVM cart = (CartVM)Session["Cart"];
            model.Details = cart.Products;
            model.DeliveryPrice = db.GetCountryDeliveryPrice(model.DeliveryCountryId);
            if (!ModelState.IsValid)
                return View(model);
            if (Session["CustomerId"] == null && db.CustomerExists(model.CustomerEmail))
            {
                ViewBag.EmailUsed = true;
                return View(model);

            }


            return RedirectToAction("Pay", model);
        }

        public ActionResult Pay(CheckoutVM model)
        {
            model.Countries = db.GetCountries();
            CartVM cart = (CartVM)Session["Cart"];
            model.Details = cart.Products;
            model.DeliveryPrice = db.GetCountryDeliveryPrice(model.DeliveryCountryId);
            return View(model);
        }
        [HttpPost]
        public ActionResult SaveOrder(CheckoutVM model)
        {
            CartVM cart = (CartVM)Session["Cart"];
            model.Details = cart.Products;
            model.DeliveryPrice = db.GetCountryDeliveryPrice(model.DeliveryCountryId);
            if (!string.IsNullOrEmpty(cart.CouponUniqueId))
                model.CouponId = db.GetCouponInfo(cart.CouponUniqueId).CouponId;
            Session.Remove("Cart");
            if (Session["CustomerId"] == null)
            {
                var customerData = db.CreateCustomer(model);
                string content = System.IO.File.ReadAllText(Server.MapPath("/Mails/newuser.txt"));
                content = content.Replace("{username}", model.CustomerEmail);
                content = content.Replace("{password}", customerData.Password);

                new Common.Tools.Mailer().SendMailSpecific(
                content,
                model.CustomerEmail,
                "Your user is created"
                );

                Session["CustomerId"] = customerData.CustomerId;
            }



            model.CustomerId = (int)Session["CustomerId"];
            db.SaveSale(model);
            return RedirectToAction("Index", "Home");
        }

    }
}