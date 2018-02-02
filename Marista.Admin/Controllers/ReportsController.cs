using Marista.Admin.Filters;
using Marista.Common.ViewModels;
using Marista.DL;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Marista.Admin.Controllers
{
    [AdminAuthorize]
    public class ReportsController : BaseController
    {
        private readonly ReportService _db = new ReportService();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.UserData == null)
            {
                filterContext.HttpContext.Response.Redirect(Url.Action("Login", "User"), true);
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        public async Task<ActionResult> MyTeamReport()
        {
            if (this.UserData.LevelId != 2)
            {
                return RedirectToAction("Login", "User");
            }
            var report = await _db.GetMyTeamReport(this.UserData.UserId);
            return View(report);
        }

        public async Task<ActionResult> MyProfileReport()
        {
            if (this.UserData.LevelId != 2)
            {
                return RedirectToAction("Login", "User");
            }
            var report = await _db.GetMyProfileReport(this.UserData.UserId);
            return View(report);
        }

        public ActionResult MonthlyReports()
        {
            if (this.UserData.LevelId != 1)
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }

        public async Task<ActionResult> BoikoReport()
        {
            if (this.UserData.LevelId != 1)
            {
                return RedirectToAction("Login", "User");
            }

            var boiko = await _db.GetBoikoReport();
            StringBuilder sb = new StringBuilder();
            foreach (var b in boiko)
                sb.AppendLine(b.Barcode + ";" + b.Quantity + ";");
            var fc = new FileContentResult(Encoding.UTF8.GetBytes(sb.ToString()), "application/octet-stream");
            fc.FileDownloadName = "boiko.csv";
            return fc;
        }

        public async Task<ActionResult> MassPaymentPaypalReport()
        {
            if (this.UserData.LevelId != 1)
            {
                return RedirectToAction("Login", "User");
            }

            var report = _db.GetMassPaymentPaypalReport();
            StringBuilder sb = new StringBuilder();
            foreach (var r in report)
                sb.AppendLine(r.EMail + ";"+r.Ammount.ToString() + ";EUR;");
            var fc = new FileContentResult(Encoding.UTF8.GetBytes(sb.ToString()), "application/octet-stream");
            fc.FileDownloadName = "paypal.csv";
            return fc;
        }

        public async Task<ActionResult> MicroInvestReport()
        {
            if (this.UserData.LevelId != 1)
            {
                return RedirectToAction("Login", "User");
            }

            var mi = await _db.GetMicroinvestReport();
            XElement root = XElement.Parse("<TransferData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"urn:Transfer\"></TransferData>");

            XElement accountings = XElement.Parse("<Accountings/>");
            root.Add(accountings);
            foreach (var m in mi)
            {
                var date = m.OnDate.ToString("yyyy-MM-dd");

                XElement xeAccounting = new XElement("Accounting");
                xeAccounting.Add(new XAttribute("AccountingDate", date));
                xeAccounting.Add(new XAttribute("ViesMonth", date));
                xeAccounting.Add(new XAttribute("DueDate", date));
                xeAccounting.Add(new XAttribute("Term", "Продажби"));
                xeAccounting.Add(new XAttribute("Reference", "Продажба на стока"));
                xeAccounting.Add(
                    new XElement("Document",
                        new XAttribute("DocumentType", "1"),
                        new XAttribute("Number", m.SaleId.ToString().PadLeft(10, '0')),
                        new XAttribute("Date", date)
                    )
                    );
                xeAccounting.Add(
                    new XElement("Company",
                        new XAttribute("Name", m.CustomerName),
                        new XAttribute("ContactName", "Не е зададен"),
                        new XAttribute("Bulstat", "999999999"),
                        new XAttribute("VatNumber", "999999999"),
                        new XElement("Address",
                            new XAttribute("Location", m.Address),
                            new XAttribute("City", m.CountryName + "," + m.City)
                            )

                    )
                    );

                decimal novat = m.Total / 1.2M;
                decimal vat = m.Total - novat;
                xeAccounting.Add(
                    new XElement("AccountingDetails",
                        new XElement("AccountingDetail",
                            new XAttribute("AccountNumber", "411"),
                            new XAttribute("Amount", m.Total),
                            new XAttribute("Direction", "Debit")
                        ),
                        new XElement("AccountingDetail",
                            new XAttribute("AccountNumber", "702"),
                            new XAttribute("Amount", novat),
                            new XAttribute("Direction", "Credit")
                        ),
                        new XElement("AccountingDetail",
                            new XAttribute("AccountNumber", "453/2"),
                            new XAttribute("Amount", vat),
                            new XAttribute("Direction", "Credit")
                        )
                    )
                );

                accountings.Add(xeAccounting);
            }




            var fc = new FileContentResult(Encoding.UTF8.GetBytes(root.ToString()), "application/octet-stream");
            fc.FileDownloadName = "microinvest.xml";
            return fc;
        }

    }
}