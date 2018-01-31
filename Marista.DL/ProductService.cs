using AutoMapper;
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
    public class ProductService
    {
        private readonly MaristaEntities db = new MaristaEntities();
        private IMapper _map;

        public ProductService()
        {
            _map = VMMapper.Instance.Mapper;
        }

        public async Task<IList<ProductVM>> Get(string search = null)
        {
            var query = db.Products
                .Include(x => x.HCategory)
                .Include(x => x.VCategory);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search) || x.Description.Contains(search));
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
            catch (Exception ex)
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

        public async Task<IList<ProductVM>> GetRelatedTo(int productId)
        {
            var p = await db.Products.Include(x => x.RelatedToProducts).SingleOrDefaultAsync(x => x.ProductId == productId);
            if (p == null) return new List<ProductVM>();
            return _map.Map<IList<ProductVM>>(p.RelatedToProducts);
        }

        public async Task<IList<ProductVM>> GetRelatedFrom(int productId)
        {
            var p = await db.Products.Include(x => x.RelatedFromProducts).SingleOrDefaultAsync(x => x.ProductId == productId);
            if (p == null) return new List<ProductVM>();
            return _map.Map<IList<ProductVM>>(p.RelatedFromProducts);
        }

        public async Task<IList<RelatedProductVM>> GetAllRelationships(string search = null)
        {
            var products = await db.Products.Include(x => x.RelatedToProducts).Where(x => x.RelatedToProducts.Any()).ToListAsync();
            var list = new List<RelatedProductVM>();
            foreach (var p in products)
            {
                foreach (var rp in p.RelatedToProducts)
                {
                    list.Add(new RelatedProductVM(_map.Map<ProductVM>(p), _map.Map<ProductVM>(rp)));
                }
            }
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToUpper();
                list = list.Where(x => x.FromProductName.ToUpper().Contains(search) || x.ToProductName.ToUpper().Contains(search)).ToList();
            }
            return list;
        }

        public async Task CreateRelated(RelatedProductVM rp)
        {
            var fromProduct = await db.Products.SingleOrDefaultAsync(x => x.ProductId == rp.FromProductId);
            var toProduct = await db.Products.SingleOrDefaultAsync(x => x.ProductId == rp.ToProductId);
            if (fromProduct == null || toProduct == null)
            {
                throw new ArgumentException("Both products are required");
            }
            if (fromProduct.ProductId == toProduct.ProductId)
            {
                throw new ArgumentException("Cannot create a relationship between the same product");
            }
            fromProduct.RelatedToProducts.Add(toProduct);
            await db.SaveChangesAsync();
        }

        public async Task DeleteRelated(int fromProductId, int toProductId)
        {
            var fromProduct = await db.Products.Include(x => x.RelatedToProducts).SingleOrDefaultAsync(x => x.ProductId == fromProductId);
            if (fromProduct == null) return;
            fromProduct.RelatedToProducts.Remove(fromProduct.RelatedToProducts.Single(x => x.ProductId == toProductId));
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

        public IEnumerable<SelectListItem> GetCountries()
        {
            return db.Countries.OrderBy(o => o.CountryName).Select(c => new SelectListItem() { Text = c.CountryName, Value = c.CountryId.ToString() }).ToList();

        }

        public decimal GetCountryDeliveryPrice(string countryId)
        {
            return db.Countries.First(c => c.CountryId == countryId).DeliveryPrice;
        }

        public CouponVM GetCouponInfo(string uniqueId)
        {
            var cpn = db.Coupons.FirstOrDefault(c => c.UniqueId == uniqueId && c.Expires > DateTime.Now && c.Sales.Count == 0);
            if (cpn == null)
                return null;
            else
                return _map.Map<CouponVM>(cpn);
        }

        public void GetShopContent(ShopVM shop)
        {
            var allResults = db.Products.Where(p => (!shop.HCategoryId.HasValue || shop.HCategoryId.Value == p.HCategoryId) && (!shop.VCategoryId.HasValue || shop.VCategoryId.Value == p.VCategoryId));
            int rc = allResults.Count();
            shop.PagesCount = rc / shop.PageSizeId + (rc % shop.PageSizeId > 0 ? 1 : 0);
            switch (shop.SortById)
            {
                case 1: allResults = allResults.OrderBy(i => i.Name); break;
                case 2: allResults = allResults.OrderBy(i => i.Price); break;

            }
            int toSkip = (shop.Page - 1) * shop.PageSizeId;
            var page = allResults.Skip<Product>(toSkip).Take(shop.PageSizeId);
            shop.Products = page.ProjectToList<ShopItemVM>(_map.ConfigurationProvider);
            shop.HCategory = db.HCategories.Select(c => new SelectListItem() { Text = c.CategoryName, Value = c.HCategoryId.ToString() }).ToList();
            shop.VCategory = db.VCategories.Select(c => new SelectListItem() { Text = c.CategoryName, Value = c.VCategoryId.ToString() }).ToList();



        }
    }
}
