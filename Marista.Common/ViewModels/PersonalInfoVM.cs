using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Marista.Common.ViewModels
{
    public class PersonalInfoVM
    {

        [Display(Name = "Customer name"), Required]
        public string CustomerName { get; set; }

        [Display(Name = "eMail"), Required, EmailAddress]
        public string CustomerEmail { get; set; }

        [Display(Name = "Password"), Required]
        public string Password { get; set; }

        public string BillingCountryId { get; set; }
        
        public IEnumerable<SelectListItem> Countries { get; set; }

        

    }
}
