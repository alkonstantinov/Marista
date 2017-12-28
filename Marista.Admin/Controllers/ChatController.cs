using Marista.Admin.Filters;
using Marista.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Marista.Admin.Controllers
{
    [AdminAuthorize]
    public class ChatController : BaseController
    {
        private readonly ChatService _cs = new ChatService();

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
            var c = await _cs.Get(this.UserData.UserId);
            return View(c);
        }
    }
}