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
    public class BPService
    {
        private readonly MaristaEntities db = new MaristaEntities();
        private IMapper _map;

        public BPService()
        {
            _map = VMMapper.Instance.Mapper;
        }

        public async Task<BPRegVM> GetNew()
        {
            BPRegVM rec = new BPRegVM();
            rec.Countries = db.Countries.OrderBy(o => o.CountryName).Select(c => new SelectListItem() { Text = c.CountryName, Value = c.CountryId.ToString() }).ToList();
            rec.Leaders = db.BPs.OrderBy(o => o.BPName).Select(c => new SelectListItem() { Text = c.BPName, Value = c.SiteUserId.ToString() }).ToList();
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

            var customer = db.Customers.FirstOrDefault(c => c.Username == rec.EMail);
            if (customer == null)
            {
                db.Customers.Add(
                    new Customer()
                    {
                        Address = rec.Address,
                        City = "",
                        BPId = BP.BPId,
                        CountryId = rec.CountryId,
                        CustomerName = rec.BPName,
                        Password = MD5.ConvertToMD5(rec.Password),
                        Username = rec.EMail

                    }
                    );
            }
            else
                customer.BPId = rec.BPId;
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
