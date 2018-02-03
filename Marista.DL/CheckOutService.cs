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
    public class CheckoutService
    {
        private readonly MaristaEntities db = new MaristaEntities();
        private IMapper _map;

        public CheckoutService()
        {
            _map = VMMapper.Instance.Mapper;
        }

        //da zapishe sale
        public bool CustomerExists(string email)
        {
            return db.Customers.Any(c => c.Username == email);

        }
        public CheckoutVM GetLastCheckoutData(int customerId)
        {
            var sale = db.Sales.LastOrDefault(s => s.CustomerId == customerId);
            if (sale == null)
                return new CheckoutVM();
            return _map.Map<CheckoutVM>(_map.ConfigurationProvider);
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

    }
}
