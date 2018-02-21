using Marista.Common.Tools;
using Marista.Common.ViewModels;
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

        public ActionResult List()
        {
            ShopVM shop = new ShopVM();
            _ps.GetShopContent(shop);
            return PartialView(shop);
        }

        public async Task<ActionResult> View(int id)
        {
            var product = await _ps.Get(id);
            if (Session["IsBP"] != null && (bool)Session["IsBP"])
            {

                foreach (var good in product.RelatedProducts)
                    good.Price = good.Price * 0.77M;
                product.Price = product.Price * 0.77M;
            }
            return View(product);
        }

        public async Task<ActionResult> Picture(int id)
        {
            var picture = await _ps.Get(id);
            return File(picture.Picture, "image/jpeg");
        }

        public async Task<ActionResult> PictureSized(int id, int width, int height)
        {
            var picture = await _ps.Get(id);
            
            return File(BitmapProcess.Resize(picture.Picture, width, height), "image/jpeg");
        }


    }
}