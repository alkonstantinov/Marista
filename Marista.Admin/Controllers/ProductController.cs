using Marista.Admin.Filters;
using Marista.Common.ViewModels;
using Marista.DL;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Marista.Admin.Controllers
{
    [AdminAuthorize]
    public class ProductController : BaseController
    {
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

        public async Task<ActionResult> Index(string q, int? page)
        {
            var products = await _ps.Get(q);
            ViewBag.SearchQuery = q ?? string.Empty;
            return View(products.ToPagedList(page ?? 1, 5));
        }

        public async Task<ActionResult> Picture(int id)
        {
            var p = await _ps.Get(id);
            return File(p.Picture, "image/jpeg");
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var p = new ProductVM();
            await PopulateSelectLists();
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductVM p)
        {
            if(ModelState.IsValid)
            {
                p.Picture = await GetUploadedFile("filePicture");
                if (p.Picture == null)
                {
                    ModelState.AddModelError("Picture", "Picture is required");
                }
                else
                {
                    p = await _ps.Create(p);
                    return RedirectToAction("Index", new { createdId = p.ProductId });
                }
            }
            await PopulateSelectLists();
            return View(p);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var p = await _ps.Get(id);
            await PopulateSelectLists();
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductVM p)
        {
            if(ModelState.IsValid)
            {
                // update the picture only if a new one is provided
                var picture = await GetUploadedFile("filePicture");
                if(picture != null)
                {
                    p.Picture = picture;
                }

                p = await _ps.Update(p);
                return RedirectToAction("Details", new { id = p.ProductId });
            }
            await PopulateSelectLists();
            return View(p);
        }

        public async Task<ActionResult> Details(int id)
        {
            var p = await _ps.Get(id);
            if(p == null)
            {
                return RedirectToAction("Index");
            }
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int productId)
        {
            await _ps.Delete(productId);
            return RedirectToAction("Index");
        }

        private async Task PopulateSelectLists()
        {
            ViewBag.HCategories = new SelectList(await _ps.GetHCategories(), "HCategoryId", "CategoryName");
            ViewBag.VCategories = new SelectList(await _ps.GetVCategories(), "VCategoryId", "CategoryName");
        }

        private async Task<byte[]> GetUploadedFile(string name)
        {
            var files = Request.Files;
            if(files.Count > 0)
            {
                var file = files[name];
                if(file != null && file.ContentLength > 0)
                {
                    using(var m = new MemoryStream())
                    {
                        await file.InputStream.CopyToAsync(m);
                        m.Position = 0;
                        return m.ToArray();
                    }
                }
            }
            return null;
        }
    }
}