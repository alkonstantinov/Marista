using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marista.Admin.ViewModels
{
    public class LoginVM
    {
        [Required, MaxLength(50)]
        public string Username { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

    }
}