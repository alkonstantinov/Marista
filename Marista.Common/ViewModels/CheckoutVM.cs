using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Display(Name = "Customer name"), Required]
        public string CustomerName { get; set; }

        [Display(Name = "eMail"), Required]
        public string CustomerEmail { get; set; }

        [Display(Name = "Phone"), Required, EmailAddress]
        public string CustomerPhone { get; set; }

        public string BillingCountryId { get; set; }

        [Display(Name = "Address"), Required]
        public string BillingAddress { get; set; }

        [Display(Name = "City"), Required]
        public string BillingCity { get; set; }

        [Display(Name = "Zip"), Required]
        public string BillingZip { get; set; }

        public string DeliveryCountryId { get; set; }

        [Display(Name = "Address"), Required]
        public string DeliveryAddress { get; set; }

        [Display(Name = "City"), Required]
        public string DeliveryCity { get; set; }

        [Display(Name = "Zip"), Required]
        public string DeliveryZip { get; set; }

        public string Note { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public List<SaleDetailVM> Details { get; set; }


        
    }
}
