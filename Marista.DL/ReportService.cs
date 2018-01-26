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
            var myBp = await db.BPs.FirstOrDefaultAsync(b => b.SiteUserId == siteUserId);
            var myPir = await db.Pyramids.FirstOrDefaultAsync(p => p.SiteUserId == siteUserId);

            MyTeamReportVM rec = new MyTeamReportVM();
            var sales = await db.vMonthSalesPerUsers.FirstOrDefaultAsync(v => v.SiteUserId == siteUserId);
            rec.MyResult = new PyramidResultVM()
            {
                Sales = sales == null ? 0 : sales.total.Value,
                NB = myPir.ToReceive,
                PBV = myPir.PBV
            };


            return rec;
        }

        public async Task<int> Create(BPRegVM rec)
        {
            //има ли такъв мейл
            var exists = await db.BPs.AnyAsync(bp => bp.EMail == rec.EMail);
            if (exists)
                return 1;
            int? leaderId = null;

            if (!rec.LeaderId.HasValue)
            {
                var leader = db.BPs.Where(b => b.CountryId == rec.CountryId && b.SiteUser.Pyramids.Any(p => !p.PyramidParentId.HasValue)).OrderBy(o => Guid.NewGuid()).Take(1);
                if (leader.Count() == 1)
                    leaderId = leader.ToArray()[0].SiteUserId;
            }
            else
                leaderId = rec.LeaderId.Value;

            int? leaderPyramidId = null;
            if (leaderId.HasValue)
                leaderPyramidId = db.Pyramids.First(p => p.SiteUserId == leaderId.Value).PyramidId;


            var siteUser = new SiteUser()
            {
                LevelId = 2,
                Password = MD5.ConvertToMD5(rec.Password),
                Username = rec.EMail
            };
            db.SiteUsers.Add(siteUser);
            db.SaveChanges();
            var BP = new BP()
            {
                Active = false,
                Address = rec.Address,
                BPName = rec.BPName,
                CountryId = rec.CountryId,
                EMail = rec.EMail,
                Files = rec.Files,
                FileName = rec.FileName,
                PayPal = rec.PayPal,
                SiteUserId = siteUser.SiteUserId

            };
            db.BPs.Add(BP);

            var pyramid = new Pyramid()
            {
                PyramidParentId = leaderPyramidId,
                SiteUserId = siteUser.SiteUserId

            };
            db.Pyramids.Add(pyramid);
            db.SaveChanges();
            return 0;



        }

        public async Task<IList<BPRegVM>> GetNotApproved()
        {
            var b = await db.BPs.Where(bp => !bp.Active).OrderBy(bp => bp.BPName).ProjectToListAsync<BPRegVM>(_map.ConfigurationProvider);
            return b;
        }

        public async Task<BPRegVM> GetDocument(int bpId)
        {
            var b = await db.BPs.FirstAsync(bp => bp.BPId == bpId);
            return _map.Map<BPRegVM>(b);
        }


        public async Task<BPRegVM> Approve(int bpId)
        {
            var b = await db.BPs.FirstAsync(bp => bp.BPId == bpId);
            b.Active = true;
            await db.SaveChangesAsync();
            return _map.Map<BPRegVM>(b);
        }

    }
}
