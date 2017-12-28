using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.ViewModels
{
    public class ConstantVM
    {
        public int ConstantId { get; set; }
        public string Name { get; set; }
        [Required]
        public decimal Value { get; set; }
    }
}
