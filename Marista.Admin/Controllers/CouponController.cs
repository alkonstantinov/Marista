using Marista.Admin.Filters;
using Marista.Common.ViewModels;
using Marista.DL;
using PagedList;
using SoundInTheory.DynamicImage.Fluent;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Marista.Admin.Controllers
{
    [AdminAuthorize]
    public class CouponController : BaseController
    {
        private readonly CouponService _cs = new CouponService();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.UserData == null || (this.UserData != null && this.UserData.LevelId != 1))
            {
                filterContext.HttpContext.Response.Redirect(Url.Action("Login", "User"), true);
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        public async Task<ActionResult> Index(string q, int? page)
        {
            var coupons = await _cs.Get(q);
            ViewBag.SearchQuery = q ?? string.Empty;
            return View(coupons.ToPagedList(page ?? 1, 20));
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var c = new CouponVM();
            await PopulateSelectLists();
            return View(c);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CouponVM c)
        {
            if (ModelState.IsValid)
            {
                var uniqueId = await GenerateUniqueId();

                while (await _cs.UniqueIdExisting(uniqueId))
                {
                    uniqueId = await GenerateUniqueId();
                }

                c.UniqueId = uniqueId;
                c.SiteUserId = UserData.UserId;
                c.Img = await  GenerateImage(uniqueId, c.Expires, c.Discount);
                c = await _cs.Create(c);

                return RedirectToAction("Index", new { createdId = c.CouponId });
            }

            return View(c);
        }

        public async Task<ActionResult> Download(int id)
        {
            var coupon = await _cs.Get(id);
            var img = coupon.Img;

            return File(img, "image/jpg", "coupon-number-" + coupon.UniqueId + ".jpg");
        }

        //Helpers
        private async Task<byte[]> GenerateImage(string uniqueId, DateTime expirationDate, int discount)
        {
            var templateImagePath = "~/Content/Images/coupon-template.jpg";
            var generatedImagePath = "~/Content/Images/generated-coupon.jpg";

            CompositionBuilder builder = new CompositionBuilder() // generating the image
                           .WithLayer(LayerBuilder.Image
                                           .SourceFile(Server.MapPath(templateImagePath)))
                           .WithLayer(LayerBuilder.Text.Text("Coupon number: " + uniqueId)
                                           .Anchor(SoundInTheory.DynamicImage.AnchorStyles.TopLeft)
                                           .ForeColor(SoundInTheory.DynamicImage.Color.FromArgb(160, 10, 10, 10))
                                           .FontSize(30)
                                           .FontBold())
                                           .ImageFormat(SoundInTheory.DynamicImage.DynamicImageFormat.Jpeg)
                           .WithLayer(LayerBuilder.Text.Text("Expires: " + expirationDate.ToString())
                                           .Anchor(SoundInTheory.DynamicImage.AnchorStyles.BottomLeft)
                                           .ForeColor(SoundInTheory.DynamicImage.Color.FromArgb(160, 10, 10, 10))
                                           .FontSize(20)
                                           .FontBold())
                                           .ImageFormat(SoundInTheory.DynamicImage.DynamicImageFormat.Jpeg)
                           .WithLayer(LayerBuilder.Text.Text("Discount: " + discount)
                                           .Anchor(SoundInTheory.DynamicImage.AnchorStyles.TopRight)
                                           .ForeColor(SoundInTheory.DynamicImage.Color.FromArgb(160, 10, 10, 10))
                                           .FontSize(20)
                                           .FontBold())
                                           .ImageFormat(SoundInTheory.DynamicImage.DynamicImageFormat.Jpeg);

            builder.SaveTo(Server.MapPath(generatedImagePath)); // Saving the generated image with the unique id on it.

            var generatedImageFileInfo = new FileInfo(Server.MapPath(generatedImagePath));
            var imageBytes = new byte[generatedImageFileInfo.Length];

            using (var fileStream = generatedImageFileInfo.OpenRead())
            {
                await fileStream.ReadAsync(imageBytes, 0, imageBytes.Length);
            }

            generatedImageFileInfo.Delete();

            return imageBytes;
        }


        private async Task PopulateSelectLists()
        {
            ViewBag.HCategories = new SelectList(await _cs.GetHCategories(), "HCategoryId", "CategoryName");
            ViewBag.VCategories = new SelectList(await _cs.GetVCategories(), "VCategoryId", "CategoryName");
            ViewBag.Products = new SelectList(await _cs.GetProducts(), "ProductId", "Name");
        }

        private async Task<string> GenerateUniqueId()
        {
            var random = new Random();
            var numberAsString = string.Empty;

            for (int i = 0; i < 10; i++)
            {
                 numberAsString = String.Concat(numberAsString, random.Next(10).ToString());
            }
                
            return numberAsString;
        }
    }
}