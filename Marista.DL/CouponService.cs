using AutoMapper;
using Marista.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Marista.DL
{
    public class CouponService
    {
        private readonly MaristaEntities db = new MaristaEntities();
        private IMapper _map;

        public CouponService()
        {
            this._map = VMMapper.Instance.Mapper;
        }

        public async Task<IList<CouponVM>> Get(string search = null)
        {
            var query = db.Coupons
                .Include(x => x.HCategory)
                .Include(x => x.VCategory)
                .Include(x => x.Product);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.UniqueId.Contains(search));
            }

            return await query.ProjectToListAsync<CouponVM>(_map.ConfigurationProvider);
        }

        public async Task<CouponVM> Get(int id)
        {
            var c = await db.Coupons
                .Include(x => x.HCategory)
                .Include(x => x.VCategory)
                .Include(x => x.Product)
                .SingleOrDefaultAsync(x => x.CouponId == id);

            return _map.Map<CouponVM>(c);
        }

        public async Task<CouponVM> Create(CouponVM cvm)
        {
            var c = _map.Map<Coupon>(cvm);
            c = db.Coupons.Add(c);
            await db.SaveChangesAsync();
            return _map.Map<CouponVM>(c);
        }

        public async Task Delete(int id)
        {
            var c = await db.Coupons.SingleOrDefaultAsync(x => x.CouponId == id);
            if (c == null) return;
            db.Coupons.Remove(c);
            await db.SaveChangesAsync();
        }

        public async Task<bool> UniqueIdExisting(string id)
        {
            return await db.Coupons.AnyAsync(c => c.UniqueId == id);
        }

        public async Task<IList<HCategory>> GetHCategories()
        {
            return await db.HCategories.ToListAsync();
        }

        public async Task<IList<VCategory>> GetVCategories()
        {
            return await db.VCategories.ToListAsync();
        }

        public async Task<IList<Product>> GetProducts()
        {
            return await db.Products.ToListAsync();
        }
    }
}
