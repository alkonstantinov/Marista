using AutoMapper;
using Marista.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.DL
{
    public class BonusSizeService
    {
        private readonly MaristaEntities db = new MaristaEntities();
        private IMapper _map;

        public BonusSizeService()
        {
            _map = VMMapper.Instance.Mapper;
        }

        public async Task<BonusSizeVM> Get(int id)
        {
            var b = await db.BonusSizes.SingleOrDefaultAsync(x => x.BonusSizeId == id);
            return _map.Map<BonusSizeVM>(b);
        }

        public async Task<IList<BonusSizeVM>> Get()
        {
            var b = await db.BonusSizes.ProjectToListAsync<BonusSizeVM>(_map.ConfigurationProvider);
            return b;
        }

        public async Task<BonusSizeVM> Create(BonusSizeVM bvm)
        {
            var b = _map.Map<BonusSize>(bvm);
            b = db.BonusSizes.Add(b);
            await db.SaveChangesAsync();
            return _map.Map<BonusSizeVM>(b);
        }

        public async Task<BonusSizeVM> Update(BonusSizeVM bvm)
        {
            var b = await db.BonusSizes.SingleOrDefaultAsync(x => x.BonusSizeId == bvm.BonusSizeId);
            _map.Map<BonusSizeVM, BonusSize>(bvm, b);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            return bvm;
        }

        public async Task Delete(int id)
        {
            var b = await db.BonusSizes.SingleOrDefaultAsync(x => x.BonusSizeId == id);
            if (b == null) return;
            db.BonusSizes.Remove(b);
            await db.SaveChangesAsync();
        }

    }
}
