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
        private readonly ProductService _ps = new ProductService();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.UserData == null || (this.UserData != null && this.UserData.LevelId != 1))
            {
                filterContext.HttpContext.Response.Redirect(Url.Action("Login", "User"), true);
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        public async Task<ActionResult> Index(string q, int? page, int? createdId)
        {
            var coupons = await _cs.Get(q);
            ViewBag.SearchQuery = q ?? string.Empty;
            ViewBag.CreatedId = createdId;
            return View(coupons.ToPagedList(page ?? 1, 20));
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var c = new CouponVM();
            c.Expires = DateTime.Now.AddDays(30); // default expiration date
            c.ForAll = true; //select for all products by default
            await PopulateSelectLists();
            return View(c);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CouponVM c)
        {
            if (ModelState.IsValid)
            {
                // check if only one of the possible options has been selected
                if (!c.IsOnlyOneOptionSelected)
                {
                    ModelState.AddModelError(string.Empty, "Only one of the options should be selected");
                    await PopulateSelectLists();
                    return View(c);
                }

                var uniqueId = GenerateUniqueId();

                while (await _cs.UniqueIdExisting(uniqueId))
                {
                    uniqueId = GenerateUniqueId();
                }

                c.UniqueId = uniqueId;
                c.SiteUserId = UserData.UserId;
                c.Img = await GenerateImage(uniqueId, c.Expires, c.Discount);
                c = await _cs.Create(c);

                return RedirectToAction("Index", new { createdId = c.CouponId });
            }

            await PopulateSelectLists();
            return View(c);
        }

        public async Task<ActionResult> Download(int id)
        {
            var coupon = await _cs.Get(id);
            var img = coupon.Img;

            return File(img, "image/jpg", "coupon-number-" + coupon.UniqueId + ".jpg");
        }

        public async Task<ActionResult> SwitchUsed(int id)
        {
            await _cs.SwitchUsed(id);
            return RedirectToAction("Index");
        }

        //Helpers
        private async Task<byte[]> GenerateImage(string uniqueId, DateTime expirationDate, int discount)
        {
            var templateImagePath = "~/Content/Images/coupon-template.jpg";
            var generatedImagePath = $"~/Content/Images/generated-coupon-{uniqueId}.jpg";

            CompositionBuilder builder = new CompositionBuilder() // generating the image
                           .WithLayer(LayerBuilder.Image
                                           .SourceFile(Server.MapPath(templateImagePath)))
                           .WithLayer(LayerBuilder.Text.Text("Coupon number: " + uniqueId)
                                           .Anchor(SoundInTheory.DynamicImage.AnchorStyles.TopLeft)
                                           .ForeColor(SoundInTheory.DynamicImage.Color.FromArgb(160, 10, 10, 10))
                                           .FontSize(30)
                                           .FontBold())
                           .WithLayer(LayerBuilder.Text.Text("Expires: " + expirationDate.ToString())
                                           .Anchor(SoundInTheory.DynamicImage.AnchorStyles.BottomLeft)
                                           .ForeColor(SoundInTheory.DynamicImage.Color.FromArgb(160, 10, 10, 10))
                                           .FontSize(20)
                                           .FontBold())
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
            ViewBag.HCategories = new SelectList(await _ps.GetHCategories(), "HCategoryId", "CategoryName");
            ViewBag.VCategories = new SelectList(await _ps.GetVCategories(), "VCategoryId", "CategoryName");
            ViewBag.Products = new SelectList(await _ps.Get(), "ProductId", "Name");
        }

        private string GenerateUniqueId()
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            int randomNumber = random.Next(1, 999999999);
            return randomNumber.ToString().PadLeft(10, '0');
        }
    }
}