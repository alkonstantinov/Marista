using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Marista.Common.ViewModels
{
    public class BPRegVM
    {

        public int BPId { get; set; }

        [Display(Name = "Name"), Required]
        public string BPName { get; set; }

        [Display(Name = "eMail"), Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EMail { get; set; }

        [Display(Name = "Paypal account"), Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string PayPal { get; set; }

        [Display(Name = "Password"), Required]
        public string Password { get; set; }

        [Display(Name = "Address"), Required]
        public string Address { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        [Display(Name = "Country")]
        public int CountryId { get; set; }

        public IEnumerable<SelectListItem> Leaders { get; set; }

        [Display(Name = "Leader")]
        public int? LeaderId { get; set; }

        public byte[] Files { get; set; }

        public string FileName { get; set; }

    }
}
