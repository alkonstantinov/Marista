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
    public class ConstantService
    {
        private readonly MaristaEntities db = new MaristaEntities();
        private IMapper _map;

        public ConstantService()
        {
            _map = VMMapper.Instance.Mapper;
        }

        public async Task<ConstantVM> Get(int id)
        {
            var c = await db.Constants.SingleOrDefaultAsync(x => x.ConstantId == id);
            return _map.Map<ConstantVM>(c);
        }

        public async Task<IList<ConstantVM>> Get()
        {
            var c = await db.Constants.ProjectToListAsync<ConstantVM>(_map.ConfigurationProvider);
            return c;
        }

        public async Task<ConstantVM> Update(ConstantVM cvm)
        {
            var c = await db.Constants.SingleOrDefaultAsync(x => x.ConstantId == cvm.ConstantId);
            c.Value = cvm.Value;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            return cvm;
        }

    }
}
