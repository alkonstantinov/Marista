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
    public class MarketingMaterialController : BaseController
    {
        private readonly MarketingMaterialService _ms = new MarketingMaterialService();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.UserData == null || (this.UserData != null && this.UserData.LevelId != 1))
            {
                filterContext.HttpContext.Response.Redirect(Url.Action("Login", "User"), true);
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        public async Task<ActionResult> Index()
        {
            var materials = await _ms.Get();
            ViewBag.Level = this.UserData.LevelId;
            return View(materials);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MarketingMaterialVM m)
        {

            if (m.Title != "" && Request.Files.Count == 1)
            {
                m.FileName = Request.Files[0].FileName;
                MemoryStream target = new MemoryStream();
                Request.Files[0].InputStream.CopyTo(target);
                m.Content = target.ToArray();
                await _ms.Create(m);
            }
            return RedirectToAction("Index");
        }



        public async Task<ActionResult> Get(int id)
        {
            var b = await _ms.Get(id);
            if (b == null)
            {
                return RedirectToAction("Index");
            }
            var fc = new FileContentResult(b.Content, "application/octet-stream");
            fc.FileDownloadName = b.FileName;
            return fc;

        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _ms.Delete(id);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}