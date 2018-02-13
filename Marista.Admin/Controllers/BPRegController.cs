using Marista.Admin.Filters;
using Marista.Common.Tools;
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

    public class BPRegController : BaseController
    {
        private readonly BPService db = new BPService();


        public async Task<ActionResult> Index()
        {
            var bpReg = await db.GetNew();
            return View(bpReg);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BPRegVM rec)
        {
            if (Request.Files[0].ContentLength == 0)
            {
                ViewBag.AttachDocuments = true;
                var bpReg = await db.GetNew();
                rec.Countries = bpReg.Countries;
                rec.Leaders = bpReg.Leaders;
                return View("Index", rec);
            }


            MemoryStream target = new MemoryStream();

            Request.Files[0].InputStream.CopyTo(target);
            rec.Files = target.ToArray();
            rec.FileName = Request.Files[0].FileName;
            if (ModelState.IsValid)
            {
                int res = await db.Create(rec);
                if (res == 1)
                {
                    ViewBag.EmailUsed = true;
                    var bpReg = await db.GetNew();
                    rec.Countries = bpReg.Countries;
                    rec.Leaders = bpReg.Leaders;
                    return View("Index", rec);
                }
            }
            return RedirectToAction("Success");
        }

        public ActionResult Success()
        {
            return View();
        }


        public async Task<ActionResult> NotApproved()
        {
            var rec = await db.GetNotApproved();
            return View(rec);

        }

        public async Task<ActionResult> Approve(int bpId)
        {
            var bp = await db.Approve(bpId);
            var rec = await db.GetNotApproved();
            new Common.Tools.Mailer().SendMail(
                Server.MapPath("/Mails/approve.txt"),
                bp.EMail,
                "You are approved"
                );

            return View("NotApproved", rec);

        }

        public async Task<ActionResult> Documents(int bpId)
        {
            var docs = await db.GetDocument(bpId);
            var fc = new FileContentResult(docs.Files, "application/octet-stream");
            fc.FileDownloadName = docs.FileName;
            return fc;
        }

        [HttpGet]
        public async Task<ActionResult> FilterBPs(SearchBPVM model)
        {
            if (model == null)
                model = new SearchBPVM();
            model.Countries = db.GetCountries();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> DoFilterBPs(SearchBPVM model)
        {
            await db.Search(model);
            return View("FilterBPs", model);
        }

        [HttpPost]
        public async Task<ActionResult> MailFilterBPs(SearchBPVM model)
        {
            ViewBag.ok = true;
            foreach (var item in Request.Form.AllKeys.Where(k => k.StartsWith("cbBP_")))
            {
                var bp = await db.GetBP(int.Parse(Request.Form[item]));
                new Mailer().SendMailSpecific(
                    model.MailText,
                    bp.EMail,
                    "Mail from Marista"
                    );
            }
            model.Countries = db.GetCountries();
            return View ("FilterBPs", model);
        }


    }
}