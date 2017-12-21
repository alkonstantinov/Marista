using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.DL
{
    public class ProductService
    {
        private readonly MaristaEntities db = new MaristaEntities();

        public async Task<IList<Product>> Get(string search = null)
        {
            var query = db.Products
                .Include(x => x.HCategory)
                .Include(x => x.VCategory);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search));
            }
                
            return await query.ToListAsync();
        }

        public async Task<Product> Get(int id)
        {
            return await db.Products.SingleOrDefaultAsync(x => x.ProductId == id);
        }

        public async Task<Product> Create(Product p)
        {
            p = db.Products.Add(p);
            await db.SaveChangesAsync();
            return p;
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
