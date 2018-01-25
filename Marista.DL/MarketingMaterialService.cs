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
    public class MarketingMaterialService
    {
        private readonly MaristaEntities db = new MaristaEntities();
        private IMapper _map;

        public MarketingMaterialService()
        {
            _map = VMMapper.Instance.Mapper;
        }

        public async Task<MarketingMaterialVM> Get(int id)
        {
            var b = await db.MarketingMaterials.SingleOrDefaultAsync(x => x.MarketingMaterialId== id);
            return _map.Map<MarketingMaterialVM>(b);
        }

        public async Task<IList<MarketingMaterialVM>> Get()
        {
            var b = await db.MarketingMaterials.ProjectToListAsync<MarketingMaterialVM>(_map.ConfigurationProvider);
            return b;
        }

        public async Task<MarketingMaterialVM> Create(MarketingMaterialVM mm)
        {
            var b = _map.Map<MarketingMaterial>(mm);
            b = db.MarketingMaterials.Add(b);
            await db.SaveChangesAsync();
            return _map.Map<MarketingMaterialVM>(b);
        }

       

        public async Task Delete(int id)
        {
            var b = await db.MarketingMaterials.SingleOrDefaultAsync(x => x.MarketingMaterialId == id);
            if (b == null) return;
            db.MarketingMaterials.Remove(b);
            await db.SaveChangesAsync();
        }

    }
}
