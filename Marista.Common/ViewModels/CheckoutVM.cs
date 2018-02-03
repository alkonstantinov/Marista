using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Marista.Common.ViewModels
{
    public class CheckoutVM
    {

        public int CustomerId { get; set; }
        public DateTime OnDate { get; set; }
        public int? CouponId { get; set; }
        public decimal DeliveryPrice { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string BillingCountryId { get; set; }
        public string BillingAddress { get; set; }
        public string BillingCity { get; set; }
        public string BillingZip { get; set; }
        public string DeliveryCountryId { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryZip { get; set; }
        public string Note { get; set; }
        public IList<SelectListItem> Countries { get; set; }
        public List<SaleDetailVM> SaleDetails { get; set; }

    }
}
