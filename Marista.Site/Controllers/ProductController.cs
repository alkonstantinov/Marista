using Marista.DL;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Marista.Site.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _ps = new ProductService();

        public ActionResult List(int? page)
        {
            var products = Task.Run(async () => await _ps.Get()).Result;
            return PartialView(products.ToPagedList(page ?? 1, 8));
        }

        public async Task<ActionResult> View(int id)
        {
            var product = await _ps.Get(id);
            return View(product);
        }

        public async Task<ActionResult> Picture(int id)
        {
            var picture = await _ps.Get(id);
            return File(picture.Picture, "image/jpeg");
        }
    }
}