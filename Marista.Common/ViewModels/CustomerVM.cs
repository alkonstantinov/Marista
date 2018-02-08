using Marista.Common.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.ViewModels
{
    public class CustomerVM
    {
        public int CustomerId { get; set; }

        public int? BPId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        [Display(Name = "New Password")]
        [Required]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm New")]
        [Required]
        [Compare("NewPassword")]
        public string RePassword { get; set; }

        public string NewPasswordMD5
        {
            get
            {
                return MD5.ConvertToMD5(NewPassword);
            }
        }
        public string PasswordMD5
        {
            get
            {
                return MD5.ConvertToMD5(Password);
            }
        }

        public string CustomerName { get; set; }

        public string Address { get; set; }

        public string CountryId { get; set; }

        public string City { get; set; }

    }
}
