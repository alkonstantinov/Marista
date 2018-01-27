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
    public class ReportsController : BaseController
    {
        private readonly ReportService _db = new ReportService();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.UserData == null || (this.UserData != null && this.UserData.LevelId != 2))
            {
                filterContext.HttpContext.Response.Redirect(Url.Action("Login", "User"), true);
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        public async Task<ActionResult> MyTeamReport()
        {
            var report = await _db.GetMyTeamReport(this.UserData.UserId);
            return View(report);
        }
        
    }
}