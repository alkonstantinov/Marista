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
    public class SOBonusSizeService
    {
        private readonly MaristaEntities db = new MaristaEntities();
        private IMapper _map;

        public SOBonusSizeService()
        {
            _map = VMMapper.Instance.Mapper;
        }

        public async Task<SOBonusSizeVM> Get(int id)
        {
            var b = await db.SOBonusSizes.SingleOrDefaultAsync(x => x.SOBonusSizeId == id);
            return _map.Map<SOBonusSizeVM>(b);
        }

        public async Task<IList<SOBonusSizeVM>> Get()
        {
            var b = await db.SOBonusSizes.ProjectToListAsync<SOBonusSizeVM>(_map.ConfigurationProvider);
            return b;
        }

        public async Task<SOBonusSizeVM> Create(SOBonusSizeVM bvm)
        {
            var b = _map.Map<SOBonusSize>(bvm);
            b = db.SOBonusSizes.Add(b);
            await db.SaveChangesAsync();
            return _map.Map<SOBonusSizeVM>(b);
        }

        public async Task<SOBonusSizeVM> Update(SOBonusSizeVM bvm)
        {
            var b = await db.SOBonusSizes.SingleOrDefaultAsync(x => x.SOBonusSizeId == bvm.SOBonusSizeId);
            _map.Map<SOBonusSizeVM, SOBonusSize>(bvm, b);
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
            var b = await db.SOBonusSizes.SingleOrDefaultAsync(x => x.SOBonusSizeId == id);
            if (b == null) return;
            db.SOBonusSizes.Remove(b);
            await db.SaveChangesAsync();
        }

    }
}
