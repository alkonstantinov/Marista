using Marista.Common.Tools;
using Marista.DL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZXing;
using ZXing.Common;

namespace Marista.Admin.Controllers
{
    public class SalesController : BaseController
    {
        CustomerService dl = new CustomerService();
        // GET: Sales
        public ActionResult Index()
        {
            if (this.UserData == null || (this.UserData != null && this.UserData.LevelId != 1))
            {
                return RedirectToAction("Login", "User");
            }


            return View(dl.GetSalesForDispatch());
        }

        public ActionResult ShippingLabel(int saleId)
        {
            return View(dl.GetSale(saleId));
        }

        public ActionResult PackingList(int saleId)
        {
            return View(dl.GetSale(saleId));
        }

        public ActionResult SetDispatched(int saleId)
        {
            dl.SetDispatched(saleId);
            return View("Index", dl.GetSalesForDispatch());
        }

        public ActionResult GetSaleBarcode(int saleId)
        {
            
            var fc = new FileContentResult(Barcode.GenerateBarcode(saleId), "application/octet-stream");
            fc.FileDownloadName = "barcode.png";
            return fc;
        }
    }
}