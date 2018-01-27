using Marista.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Marista.Site.Controllers
{
    public class CartController : Controller
    {
        public ActionResult Embed()
        {
            return PartialView();
        }

        public async Task<ActionResult> Index()
        {
            return View();
        }
    }
}