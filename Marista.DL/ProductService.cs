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
                .Include(x => x.RelatedFromProducts)
                .SingleOrDefaultAsync(x => x.ProductId == id);

            var related = p.RelatedFromProducts;
            List<ProductVM> lp = new List<ProductVM>();
            foreach (var rel in related)
                lp.Add(_map.Map<ProductVM>(rel));
            var result = _map.Map<ProductVM>(p);
            result.RelatedProducts = lp;

            StringBuilder sb = new StringBuilder();
            sb.Append("['/product/Picture?id=" + id + "'");
            foreach (var pp in db.ProductPictures.Where(item => item.ProductId == id))
                sb.Append(",'/product/ProductPicture?productPictureId=" + pp.ProductPictureId + (pp.IsVideo ? "&video=1" : "") + "'");
            sb.Append("]");
            result.PictureUrls = sb.ToString();
            return result;
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

        public decimal GetCountryDeliveryPrice(string countryId, decimal weight)
        {
            var country = db.Countries.First(c => c.CountryId == countryId);

            return db.CountryDeliveries.First(cd => cd.CountryTypeId == country.CountryTypeId && cd.FromWeight <= weight && cd.ToWeight >= weight).Price.Value;
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

        public void SaveSale(CheckoutVM model)
        {
            Sale s = _map.Map<Sale>(model);
            s.OnDate = DateTime.Now;
            s.SaleStatusId = 1;
            db.Sales.Add(s);
            db.SaveChanges();
            int saleId = s.SaleId;
            decimal PBV = 0;
            decimal SalesBonus = 0;

            foreach (var sd in model.Details)
            {
                sd.SaleId = saleId;
                var item = new SaleDetail()
                {
                    ProductId = sd.ProductId,
                    Discount = sd.Discount,
                    Price = sd.Price,
                    Quantity = sd.Quantity,
                    SaleId = saleId
                };
                db.SaleDetails.Add(item);
                PBV += sd.Price * sd.Quantity;
                SalesBonus += sd.Price * sd.Quantity * (23.0M - sd.Discount) / 100.0M;
                //db.SaveChanges();
            }
            if (model.CouponId.HasValue)
            {
                var co = db.Coupons.First(c => c.CouponId == model.CouponId.Value);
                var py = db.Pyramids.First(item => item.SiteUserId == co.SiteUserId);

                py.SaleBonus += SalesBonus;
                py.PBV += PBV;
            }

            db.SaveChanges();


        }

        public bool CustomerExists(string email)
        {
            return db.Customers.Any(c => c.Username == email);

        }
        public CheckoutVM GetLastCheckoutData(int customerId)
        {
            var sale = db.Sales.Where(s => s.CustomerId == customerId).OrderByDescending(s => s.SaleId).FirstOrDefault();
            if (sale == null)
                return new CheckoutVM();
            return _map.Map<CheckoutVM>(sale);
        }

        public void FillCountries(CheckoutVM model)
        {
            model.Countries = db.Countries.OrderBy(o => o.CountryName).Select(c => new SelectListItem() { Text = c.CountryName, Value = c.CountryId.ToString() }).ToList();
        }

        public NewCustomerVM CreateCustomer(CheckoutVM model)
        {
            string password = DateTime.Now.Millisecond.ToString().PadLeft(6, '0');
            Customer c = new Customer()
            {
                Address = model.BillingAddress,
                City = model.BillingCity,
                CountryId = model.BillingCountryId,
                CustomerName = model.CustomerName,
                Password = MD5.ConvertToMD5(password),
                Username = model.CustomerEmail
            };

            db.Customers.Add(c);
            db.SaveChanges();
            return new NewCustomerVM()
            {
                CustomerId = c.CustomerId,
                Password = password
            };
        }

        private void SavePyramidValues(Pyramid pyramid, CheckoutVM model)
        {
            pyramid.PBV += model.SubTotal;
            decimal sales = 0;
            foreach (var p in model.Details)
                sales += (0.23M - p.Discount) * p.Price * p.Quantity / 100M;
            pyramid.SaleBonus += sales;
            db.SaveChanges();
        }

        public void AddPyramidValuesByCoupon(CheckoutVM model)
        {

            var coupon = db.Coupons.First(c => c.CouponId == model.CouponId);
            var pyramid = db.Pyramids.First(p => p.SiteUserId == coupon.SiteUserId);

            SavePyramidValues(pyramid, model);
        }


        public void AddPyramidValuesToBP(CheckoutVM model, int customerId)
        {

            var customer = db.Customers.First(c => c.CustomerId == customerId);
            var bp = db.BPs.First(b => b.BPId == customer.BPId);
            var pyramid = db.Pyramids.First(p => p.SiteUserId == bp.SiteUserId);
            SavePyramidValues(pyramid, model);

        }

        public void AddPyramidValuesRandom(CheckoutVM model)
        {

            var pyramid = db.Pyramids.Where(p => p.PyramidParentId == null).OrderBy(p => Guid.NewGuid()).FirstOrDefault();
            if (pyramid != null)
                SavePyramidValues(pyramid, model);
        }

        public IList<ProductPictureVM> GetProductPictures(int productId)
        {
            return db.ProductPictures.Where(p => p.ProductId == productId).ProjectToList<ProductPictureVM>(_map.ConfigurationProvider);
        }

        public void AddProductPicrtue(ProductPictureVM model)
        {
            db.ProductPictures.Add(_map.Map<ProductPicture>(model));
            db.SaveChanges();
        }

        public byte[] GetProductPicture(int productPictureId)
        {
            return db.ProductPictures.First(p => p.ProductPictureId == productPictureId).Picture;
        }

        public int DelProductPicture(int productPictureId)
        {
            var rec = db.ProductPictures.First(p => p.ProductPictureId == productPictureId);
            int result = rec.ProductId;
            db.ProductPictures.Remove(rec);
            db.SaveChanges();
            return result;
        }

    }
}
