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
    public class ProductService
    {
        private readonly MaristaEntities db = new MaristaEntities();
        private readonly IMapper _map;

        public ProductService()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductVM>().ReverseMap().PreserveReferences()
                .ForMember(dest => dest.Picture, 
                    y => y.Condition(src => src.Picture != null && src.Picture.Length > 0))
                .ForMember(dest => dest.HCategory, y => y.Ignore())
                .ForMember(dest => dest.VCategory, y => y.Ignore())
            );
            _map = config.CreateMapper();
        }

        public async Task<IList<ProductVM>> Get(string search = null)
        {
            var query = db.Products
                .Include(x => x.HCategory)
                .Include(x => x.VCategory);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search));
            }
                
            return await query.ProjectToListAsync<ProductVM>(_map.ConfigurationProvider);
        }

        public async Task<ProductVM> Get(int id)
        {
            var p = await db.Products
                .Include(x => x.HCategory)
                .Include(x => x.VCategory)
                .SingleOrDefaultAsync(x => x.ProductId == id);
            return _map.Map<ProductVM>(p);
        }

        public async Task<ProductVM> Create(ProductVM pvm)
        {
            var p = _map.Map<Product>(pvm);
            p = db.Products.Add(p);
            await db.SaveChangesAsync();
            return _map.Map<ProductVM>(p);
        }

        public async Task<ProductVM> Update(ProductVM pvm)
        {
            var p = await db.Products.SingleOrDefaultAsync(x => x.ProductId == pvm.ProductId);
            _map.Map<ProductVM, Product>(pvm, p);
            try
            {
                await db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            return pvm;
        }

        public async Task Delete(int id)
        {
            var p = await db.Products.SingleOrDefaultAsync(x => x.ProductId == id);
            if (p == null) return;
            db.Products.Remove(p);
            await db.SaveChangesAsync();
        }

        public async Task<IList<HCategory>> GetHCategories()
        {
            return await db.HCategories.ToListAsync();
        }

        public async Task<IList<VCategory>> GetVCategories()
        {
            return await db.VCategories.ToListAsync();
        }
    }
}
