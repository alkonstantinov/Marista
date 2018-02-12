using Marista.Admin.Filters;
using Marista.Common.ViewModels;
using Marista.DL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace Marista.Admin.Controllers
{
    [AdminAuthorize]
    public class ChatController : BaseController
    {
        private readonly ChatService _cs = new ChatService();

        public async Task<ActionResult> Index(bool up=false)
        {
            if (this.UserData == null || (this.UserData != null && this.UserData.LevelId != 2))
            {
                return RedirectToAction(Url.Action("Login", "User"));
            }

            ViewBag.UserId = this.UserData.UserId;
            ViewBag.Username = this.UserData.Username;

            var c = await _cs.Get(this.UserData.UserId, up);
            return View(c);
        }

        [HttpPost]
        public ActionResult StoreAttachmentInCache(string guid, string content, string filename)
        {
            if (this.UserData == null || (this.UserData != null && this.UserData.LevelId != 2))
            {
                return RedirectToAction(Url.Action("Login", "User"));
            }

            AttachmentVM att = new AttachmentVM()
            {
                Content = Convert.FromBase64String(content.Split(',')[1]),
                Filename = filename,
                Guid = guid
            };
            HttpContext.Cache.Add(att.Guid, att, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAttachment(int chatItemId)
        {
            if (this.UserData == null || (this.UserData != null && this.UserData.LevelId != 2))
            {
                return RedirectToAction(Url.Action("Login", "User"));
            }

            var docs = await _cs.Get(chatItemId);
            var fc = new FileContentResult(docs.Attachment, "application/octet-stream");
            fc.FileDownloadName = docs.FileName;
            return fc;
        }


        public async Task<ActionResult> GetTxt(int chatId)
        {
            if (this.UserData == null || (this.UserData != null && this.UserData.LevelId != 2))
            {
                return RedirectToAction(Url.Action("Login", "User"));
            }

            var docs = await _cs.GetWholeChat(chatId);
            StringBuilder sb = new StringBuilder();
            foreach (var item in docs.Items)
            {
                sb.AppendLine(item.SiteUserUsername + " " + item.OnDate.ToShortDateString());
                sb.AppendLine(item.Said);
                if (!string.IsNullOrEmpty(item.FileName))
                    sb.AppendLine(item.FileName);
                sb.AppendLine();
            }
            var fc = new FileContentResult(Encoding.UTF8.GetBytes(sb.ToString()), "application/octet-stream");
            fc.FileDownloadName = "chat.txt";
            return fc;
        }

        public async Task<ActionResult> PlainChat(int chatId)
        {
            ViewBag.UserId = this.UserData.UserId;
            ViewBag.Username = this.UserData.Username;

            var c = await _cs.GetWholeChat(chatId);
            return View(c);
        }


        string GetPdfHtml(int chatId)
        {
            string wkhtmlPath = ConfigurationManager.AppSettings["PATHTOWKHTMLTOPDF"];
            string url = ConfigurationManager.AppSettings["URLTOCHAT"];
            string printUrl = url + "?chatid=" + chatId;
            string pdf_path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");

            ProcessStartInfo psi = new ProcessStartInfo();

            psi.FileName = wkhtmlPath;
            psi.Arguments = "  \"" + printUrl + "\" \"" + pdf_path + "\"";
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            using (Process process = new Process())
            {
                process.StartInfo = psi;
                process.EnableRaisingEvents = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                process.WaitForExit();
                process.Close();
                string result = "";

                if (System.IO.File.Exists(pdf_path))
                {
                    result = pdf_path;
                    //File.Delete(pdf_path);
                }

                return result;

            }
        }

        public async Task<ActionResult> GetPdf(int chatId)
        {
            string fnm = GetPdfHtml(chatId);
            var fc = new FileContentResult(System.IO.File.ReadAllBytes(fnm), "application/octet-stream");
            System.IO.File.Delete(fnm);
            fc.FileDownloadName = "chat.pdf";
            return fc;
        }


    }
}