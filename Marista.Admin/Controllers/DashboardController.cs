using Marista.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Marista.Admin.Controllers
{
    public class DashboardController : BaseController
    {
        BPService bps = new BPService();
        CustomerService cs = new CustomerService();
        public DashboardController()
        {

        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(this.UserData == null)
            {
                filterContext.HttpContext.Response.Redirect(Url.Action("Login", "User"), true);
            }
            base.OnActionExecuting(filterContext);
        }

        public async Task<ActionResult> Index()
        {
            ViewBag.Username = this.UserData.Username;
            ViewBag.LevelId = this.UserData.LevelId;
            ViewBag.HasParent = bps.IsUpperBp(this.UserData.UserId);

            return View();
        }

        public ActionResult CalcAllProfits()
        {
            cs.CalcAllProfits();
            return RedirectToAction("Index");
        }
    }
}