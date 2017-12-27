using Marista.Common.ViewModels;
using Marista.DL;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Marista.Admin.Controllers
{
    public class RelatedProductController : BaseController
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
            var relatedProducts = await _ps.GetAllRelationships(q);
            ViewBag.SearchQuery = q ?? string.Empty;
            return View(relatedProducts.ToPagedList(page ?? 1, 20));
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var p = new RelatedProductVM();
            await PopulateSelectLists();
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RelatedProductVM p)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _ps.CreateRelated(p);
                    return RedirectToAction("Index");
                }
                catch(ArgumentException aex)
                {
                    ModelState.AddModelError(string.Empty, aex.Message);
                }
                catch(Exception)
                {
                    ModelState.AddModelError(string.Empty, "This relationship already exists");
                }
            }
            await PopulateSelectLists();
            return View(p);
        }

        public async Task<ActionResult> Delete(int fromProductId, int toProductId)
        {
            await _ps.DeleteRelated(fromProductId, toProductId);
            return RedirectToAction("Index");
        }

        private async Task PopulateSelectLists()
        {
            ViewBag.Products = new SelectList(await _ps.Get(), "ProductId", "Name");
        }
    }
}