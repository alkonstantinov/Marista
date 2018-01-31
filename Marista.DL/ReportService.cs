using AutoMapper;
using Marista.Common.Tools;
using Marista.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Marista.DL
{
    public class ReportService
    {
        private readonly MaristaEntities db = new MaristaEntities();
        private IMapper _map;

        public ReportService()
        {
            _map = VMMapper.Instance.Mapper;
        }

        public async Task<MyTeamReportVM> GetMyTeamReport(int siteUserId)
        {


            MyTeamReportVM rec = new MyTeamReportVM();
            var myp = await db.vMyTeamReports.FirstAsync(p => p.SiteUserId == siteUserId);
            rec.MyResult = new PyramidResultVM()
            {
                NB = myp.FromOthers,
                PBV = myp.PBV,
                Sales = myp.total.HasValue ? myp.total.Value : 0
            };

            List<TeamReportVM> lMem = new List<TeamReportVM>();
            foreach (vMyTeamReport pir in db.vMyTeamReports.Where(p => p.PyramidParentId == myp.PyramidId))
            {

                TeamReportVM member = new TeamReportVM()
                {
                    BPName = pir.BPName,
                    eMail = pir.EMail
                };
                member.Result = new PyramidResultVM()
                {
                    NB = pir.FromOthers,
                    PBV = pir.PBV,
                    Sales = pir.total.HasValue ? pir.total.Value : 0
                };
                lMem.Add(member);
            }
            rec.Team = lMem;
            List<SaleVM> lSales = new List<SaleVM>();
            foreach (vSalesTotal sal in db.vSalesTotals.Where(p => p.SiteUserId == siteUserId))
            {
                SaleVM oneSale = new SaleVM()
                {
                    CustomerName = sal.CustomerName,
                    OnDate = sal.OnDate,
                    SaleId = sal.SaleId,
                    Total = sal.Total.HasValue ? sal.Total.Value : 0,
                    SaleDetails = new List<SaleDetailVM>()
                };
                foreach (SaleDetail sd in db.SaleDetails.Where(s => s.SaleId == sal.SaleId))
                {
                    SaleDetailVM sdvm = new SaleDetailVM()
                    {
                        ProductName = sd.Product.Name,
                        Price = sd.Price,
                        Quantity = sd.Quantity,
                        SaleDetailId = sd.SaleDetailId
                    };
                    oneSale.SaleDetails.Add(sdvm);
                }
                lSales.Add(oneSale);
            }
            rec.Sales = lSales;
            return rec;
        }

        public async Task<MyProfileReportVM> GetMyProfileReport(int siteUserId)
        {
            MyProfileReportVM result = new MyProfileReportVM();
            var myPyramid = await db.Pyramids.FirstAsync(b => b.SiteUserId == siteUserId);
            var MyLeader = await db.Pyramids.FirstOrDefaultAsync(b => b.PyramidId == myPyramid.PyramidParentId);
            if (MyLeader != null)
            {
                var myBP = MyLeader.SiteUser.BPs.FirstOrDefault();
                result.LeaderEmail = myBP.EMail;
                result.LeaderName = myBP.BPName;
            }
            List<SaleVM> lSales = new List<SaleVM>();
            foreach (var s in db.v3MonthSalesPerUser.Where(v => v.SiteUserId == siteUserId).OrderBy(v => v.OnDate))
            {
                SaleVM sale = new SaleVM()
                {
                    CustomerName = s.CustomerName,
                    OnDate = s.OnDate,
                    Total = s.Total.Value
                };
                lSales.Add(sale);
            }
            result.Sales = lSales;
            List<BonusVM> lFirstLevel = new List<BonusVM>();
            foreach (var fl in db.v3MonthsBonuses.Where(v => v.ParentSiteId == siteUserId))
            {
                var b = new BonusVM()
                {
                    BPName = fl.BPName,
                    Bonus = fl.Bonus.Value,
                    Month = fl.Month,
                    Year = fl.Year

                };
                lFirstLevel.Add(b);
            }
            result.FirstLevel = lFirstLevel;

            List<BonusVM> lAllBonuses = new List<BonusVM>();
            List<BonusVM> lYearBonuses = new List<BonusVM>();
            foreach (var fl in db.vBonuses.Where(v => v.SiteUserId == siteUserId).OrderBy(v=>v.Year).OrderBy(v=>v.Month))
            {
                var b = new BonusVM()
                {
                    BPName = fl.BPName,
                    Bonus = fl.Bonus.Value,
                    Month = fl.Month,
                    Year = fl.Year

                };
                lAllBonuses.Add(b);
                if (fl.Year == DateTime.Now.Year)
                    lYearBonuses.Add(b);
            }
            result.Bonuses = lAllBonuses;
            result.BonusesPerYear = lYearBonuses;

            return result;
        }

        public async Task<IList<BoikoVM>> GetBoikoReport()
        {
            var boiko = await db.vBoikoes.ProjectToListAsync<BoikoVM>(_map.ConfigurationProvider);
            return boiko;
        }

        public async Task<IList<MicroinvestVM>> GetMicroinvestReport()
        {
            var mi = await db.vMicroinvests.ProjectToListAsync<MicroinvestVM>(_map.ConfigurationProvider);
            return mi;
        }
    }
}
