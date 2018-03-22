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
        [Display(Name="Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Short Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Price without VAT")]
        public decimal Price { get; set; }

        [Display(Name = "Promotional price")]
        public Nullable<decimal> PromotionalPrice { get; set; }

        [Required]
        [Display(Name = "Vertical category")]
        public int VCategoryId { get; set; }

        public string VCategoryCategoryName { get; set; }

        [Required]
        [Display(Name = "Horizontal category")]
        public int HCategoryId { get; set; }

        public string HCategoryCategoryName { get; set; }

        //required, enforced by the controller
        [Display(Name = "Picture")]
        public byte[] Picture { get; set; }

        [Required]
        [Display(Name = "Barcode")]
        public string Barcode { get; set; }

        [Required]
        [Display(Name = "Available Quantity")]
        public int Available { get; set; }

        [Required]
        [Display(Name = "Minimum Available Quantity")]
        public int MinQuantity { get; set; }

        [Required]
        [Display(Name = "Weight[grams]")]
        public decimal Weight { get; set; }

        public string PictureUrls { get; set; }

        public IList<ProductVM> RelatedProducts { get; set; }


        [Required]
        [Display(Name = "Long Description")]
        public string LongDescription { get; set; }

        [Required]
        [Display(Name = "Benefits")]
        public string Benefits { get; set; }

    }
}
