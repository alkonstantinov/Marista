using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.ViewModels
{
    public class RelatedProductVM
    {
        [Required]
        public int FromProductId { get; set; }
        public string FromProductName { get; set; }
        [Required]
        public int ToProductId { get; set; }
        public string ToProductName { get; set; }

        public RelatedProductVM()
        {

        }

        public RelatedProductVM(ProductVM from, ProductVM to)
        {
            FromProductId = from.ProductId;
            FromProductName = from.Name;
            ToProductId = to.ProductId;
            ToProductName = to.Name;
        }
    }
}
