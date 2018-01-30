using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Marista.Common.ViewModels
{
    public class CartVM
    {
        public List<SaleDetailVM> Products { get; set; }

        public decimal CountryPrice { get; set; }

        public string CountryId { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public string CouponUniqueId { get; set; }
    }
}
