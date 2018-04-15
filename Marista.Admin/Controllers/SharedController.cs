using Marista.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marista.Admin.Controllers
{
    public class SharedController : BaseController
    {
        BPService bps = new BPService();

        // GET: Shared
        public ActionResult Menu()
        {
            if (this.UserData != null)
            {

                ViewBag.Username = this.UserData.Username;
                ViewBag.LevelId = this.UserData.LevelId;
                ViewBag.HasParent = bps.IsUpperBp(this.UserData.UserId);
            }
            return PartialView();
        }
    }
}