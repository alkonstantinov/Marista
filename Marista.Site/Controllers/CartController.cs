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

        private decimal GetDeliveryPrice(string countryId)
        {
            CartVM cart = (CartVM)Session["Cart"];
            decimal weight = 0;
            decimal price = 0;
            foreach (var d in cart.Products)
            {
                weight += d.TotalWeight * d.Quantity;
                price += d.Total;
            }
            return price <= 80 ? db.GetCountryDeliveryPrice(countryId, weight) : 0;

        }

        public ActionResult PersonalInfo()
        {
            if (Session["CustomerId"] != null)
                return RedirectToAction("checkout");
            PersonalInfoVM model = new PersonalInfoVM();
            model.Countries = db.GetCountries();

            return View(model);
        }

        public ActionResult Confirm()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterNewCustomer(PersonalInfoVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Countries = db.GetCountries();
                return View("PersonalInfo", model);
            }

            if (db.CustomerExists(model.CustomerEmail))
            {
                ViewBag.EmailUsed = true;
                model.Countries = db.GetCountries();
                return View("PersonalInfo", model);

            }

            var pass = model.Password;
            var customerData = db.CreateCustomerWithPassword(model);
            string content = System.IO.File.ReadAllText(Server.MapPath("/Mails/newuser.txt"));
            content = content.Replace("{username}", model.CustomerEmail);
            content = content.Replace("{password}", pass);

            Parallel.Invoke(() =>
            {
                Common.Tools.Mailer.SendMailSpecific(content, model.CustomerEmail, "Your user is created");
            });

            Session["CustomerId"] = customerData;
            Session["CustomerName"] = model.CustomerName;


            return RedirectToAction("checkout");
        }

        [HttpPost]
        public ActionResult LoginExisting(PersonalInfoVM model)
        {

            var cmr = db.Login(model);
            if (cmr == null)
            {
                ViewBag.LoginError = true;
                model.Countries = db.GetCountries();
                foreach (var modelValue in ModelState.Values)
                {
                    modelValue.Errors.Clear();
                }
                return View("PersonalInfo", model);
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
            }
            return RedirectToAction("checkout");
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
            cart.CountryPrice = GetDeliveryPrice(cart.CountryId);

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
            cart.CountryPrice = GetDeliveryPrice(cart.CountryId);

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
                    result.Price += p.Price * p.Quantity * (100 - p.Discount) / 100 * 1.2M;
                }
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Checkout()
        {
            int customerId = -1;
            if (Session["CustomerId"] != null)
            {
                customerId = (int)Session["CustomerId"];
                ViewBag.login = false;
            }
            else
            {
                ViewBag.login = true;

            }
            var model = db.GetLastCheckoutData(customerId);
            model.Countries = db.GetCountries();
            CartVM cart = (CartVM)Session["Cart"];
            model.BillingCountryId = cart.CountryId;
            model.DeliveryCountryId = cart.CountryId;
            model.Details = cart.Products;
            model.DeliveryPrice = cart.CountryPrice;

            if (model.CustomerName == null)
            {
                var cmr = await db.GetCustomer(customerId);
                model.BillingCountryId = cmr.CountryId;
                model.CustomerEmail = cmr.Username;
                model.CustomerName = cmr.CustomerName;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Checkout(CheckoutVM model)
        {
            model.Countries = db.GetCountries();
            CartVM cart = (CartVM)Session["Cart"];
            model.Details = cart.Products;
            model.DeliveryPrice = GetDeliveryPrice(model.DeliveryCountryId);
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
            model.DeliveryPrice = GetDeliveryPrice(model.DeliveryCountryId);
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveOrder(CheckoutVM model)
        {
            CartVM cart = (CartVM)Session["Cart"];
            model.Details = cart.Products;
            model.DeliveryPrice = GetDeliveryPrice(model.DeliveryCountryId);
            if (!string.IsNullOrEmpty(cart.CouponUniqueId))
                model.CouponId = db.GetCouponInfo(cart.CouponUniqueId).CouponId;
            Session.Remove("Cart");
            if (Session["CustomerId"] == null)
            {
                var customerData = db.CreateCustomer(model);
                string content = System.IO.File.ReadAllText(Server.MapPath("/Mails/newuser.txt"));
                content = content.Replace("{username}", model.CustomerEmail);
                content = content.Replace("{password}", customerData.Password);
                Parallel.Invoke(() =>
                {
                    Common.Tools.Mailer.SendMailSpecific(content,
                        model.CustomerEmail,
                "Your user is created");
                });
                Session["CustomerId"] = customerData.CustomerId;
                Session["CustomerName"] = model.CustomerName;
            }



            model.CustomerId = (int)Session["CustomerId"];
            db.SaveSale(model);

            if (model.CouponId.HasValue)
            {
                db.AddPyramidValuesByCoupon(model);
            }
            else
                if (Session["IsBP"] != null && (bool)Session["IsBP"])
            {
                db.AddPyramidValuesToBP(model, (int)Session["CustomerId"]);
            }
            else
            {
                db.AddPyramidValuesRandom(model);
            }
            return RedirectToAction("Confirm", "Cart");
        }

    }
}