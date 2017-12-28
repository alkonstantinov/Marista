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
    public class ConstantController : BaseController
    {
        private readonly ConstantService _cs = new ConstantService();

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
            var bonusSizes = await _cs.Get();
            ViewBag.SearchQuery = q ?? string.Empty;
            return View(bonusSizes.ToPagedList(page ?? 1, 20));
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var c = await _cs.Get(id);
            return View(c);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ConstantVM c)
        {
            if (ModelState.IsValid)
            {
                c = await _cs.Update(c);
                return RedirectToAction("Index", new { editedId = c.ConstantId });
            }
            return View(c);
        }

    }
}