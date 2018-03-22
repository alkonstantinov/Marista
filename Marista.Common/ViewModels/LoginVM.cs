using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marista.Common.ViewModels
{
    public class LoginVM
    {
        [Required, MaxLength(50), Display(Name = "User Name / eMail")]
        public string Username { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

    }
}