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
    public class SOBonusSizeController : BaseController
    {
        private readonly SOBonusSizeService _bs = new SOBonusSizeService();

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
            var bonusSizes = await _bs.Get();
            ViewBag.SearchQuery = q ?? string.Empty;
            return View(bonusSizes.ToPagedList(page ?? 1, 20));
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var p = new SOBonusSizeVM();
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SOBonusSizeVM b)
        {
            if (ModelState.IsValid)
            {
                b = await _bs.Create(b);
                return RedirectToAction("Index", new { createdId = b.SOBonusSizeId });
            }
            return View(b);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var b = await _bs.Get(id);
            return View(b);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SOBonusSizeVM b)
        {
            if (ModelState.IsValid)
            {
                b = await _bs.Update(b);
                return RedirectToAction("Details", new { id = b.SOBonusSizeId });
            }
            return View(b);
        }

        public async Task<ActionResult> Details(int id)
        {
            var b = await _bs.Get(id);
            if (b == null)
            {
                return RedirectToAction("Index");
            }
            return View(b);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int soBonusSizeId)
        {
            try
            {
                await _bs.Delete(soBonusSizeId);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { });
            }
            return RedirectToAction("Index");
        }
    }
}