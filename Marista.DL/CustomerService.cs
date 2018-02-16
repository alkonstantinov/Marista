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
    public class CustomerService
    {
        private readonly MaristaEntities db = new MaristaEntities();
        private IMapper _map;

        public CustomerService()
        {
            _map = VMMapper.Instance.Mapper;
        }

        public async Task<CustomerVM> Get(int id)
        {
            var cmr = await db.Customers.FirstAsync(c => c.CustomerId == id);
            return _map.Map<CustomerVM>(cmr);
        }

        public void ChangePass(CustomerVM model)
        {
            var cmr = db.Customers.First(c => c.CustomerId == model.CustomerId);
            cmr.Password = model.NewPasswordMD5;
            db.SaveChanges();

        }

        public CustomerVM Login(CustomerVM model)
        {
            var cmr = db.Customers.FirstOrDefault(c => c.Username == model.Username && c.Password == model.PasswordMD5);
            if (cmr == null)
                return null;
            else return _map.Map<CustomerVM>(cmr);
        }


        public bool ResetPassword(CustomerVM model)
        {
            var cmr = db.Customers.FirstOrDefault(c => c.Username == model.Username);
            if (cmr == null)
                return false;
            cmr.Password = model.NewPasswordMD5;
            db.SaveChanges();
            return true;
        }

        public IList<SaleVM> GetSales(int customerId)
        {
            List<SaleVM> l = new List<SaleVM>();
            foreach (var s in db.Sales.Where(sale => sale.CustomerId == customerId))
            {
                var data = new SaleVM();
                data.OnDate = s.OnDate;
                data.SaleId = s.SaleId;
                data.Total = s.DeliveryPrice;
                foreach (var sd in db.SaleDetails.Where(i => i.SaleId == s.SaleId))
                {
                    data.ItemCount += sd.Quantity;
                    data.Total += sd.Price * sd.Quantity * (100.0M - sd.Discount) / 100.0M;
                }
                l.Add(data);
            }
            return l;
        }


        public CheckoutVM GetSale(int saleId)
        {
            var data = db.Sales.First(s => s.SaleId == saleId);

            CheckoutVM sale = new CheckoutVM()
            {
                BillingAddress = data.BillingAddress,
                BillingCity = data.BillingCity,
                BillingCountry = db.Countries.First(i => i.CountryId == data.BillingCountryId).CountryName,
                BillingZip = data.BillingZip,
                CustomerEmail = data.CustomerEmail,
                CustomerId = data.CustomerId,
                CustomerName = data.CustomerName,
                CustomerPhone = data.CustomerPhone,
                DeliveryAddress = data.DeliveryAddress,
                DeliveryCity = data.DeliveryCity,
                DeliveryCountry = db.Countries.First(i => i.CountryId == data.BillingCountryId).CountryName,
                DeliveryPrice = data.DeliveryPrice,
                DeliveryZip = data.DeliveryZip,
                Note = data.Note,
                OnDate = data.OnDate,
                Details = new List<SaleDetailVM>(),
                SaleId = saleId

            };

            foreach (var sd in db.SaleDetails.Where(item => item.SaleId == saleId))
                sale.Details.Add(
                    new SaleDetailVM()
                    {
                        Discount = sd.Discount,
                        Price = sd.Price,
                        ProductName = db.Products.First(i => i.ProductId == sd.ProductId).Name,
                        Quantity = sd.Quantity
                    }
                    );

            return sale;

        }

        //za izprashtane

        public IList<SaleVM> GetSalesForDispatch()
        {
            return db.Sales.Where(s => s.SaleStatusId == 1).ProjectToList<SaleVM>(_map.ConfigurationProvider);
        }
        public IList<CheckoutVM> GetSalesForPost()
        {
            List<CheckoutVM> l = new List<CheckoutVM>();
            foreach (var s in db.Sales.Where(s => s.SaleStatusId == 1))
            {
                l.Add(GetSale(s.SaleId));
            }
            return l;
        }


        //smeni statusa
        public void SetDispatched(int saleId)
        {
            db.Sales.First(s => s.SaleId == saleId).SaleStatusId = 2;
            db.SaveChanges();
        }
    }
}
