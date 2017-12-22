using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.ViewModels
{
    public class ProductVM
    {
        public int ProductId { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public Nullable<decimal> PromotionalPrice { get; set; }
        [Required]
        public int VCategoryId { get; set; }
        public string VCategoryCategoryName { get; set; }
        [Required]
        public int HCategoryId { get; set; }
        public string HCategoryCategoryName { get; set; }
        public byte[] Picture { get; set; }
    }
}
