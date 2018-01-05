using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Marista.Common.ViewModels
{
    public class CouponVM
    {
        public int CouponId { get; set; }

        [MaxLength(10)]
        public string UniqueId { get; set; }

        public int SiteUserId { get; set; }

        [Required]
        [Display(Name = "Expiration date")]
        public System.DateTime Expires { get; set; }

        [Display(Name = "Horizontal category")]
        public int? HCategoryId { get; set; }

        [Display(Name = "Vertical category")]
        public int? VCategoryId { get; set; }

        [Display(Name = "Product")]
        public int? ProductId { get; set; }

        [Required]
        [DefaultValue("false")]
        [Display(Name = "For all products")]
        public bool ForAll { get; set; }

        [Required]
        [Range(1, 23)]
        [DefaultValue(1)]
        public int Discount { get; set; }

        [Required]
        [DefaultValue("false")]
        public bool Used { get; set; }

        public byte[] Img { get; set; }

        public string ProductName { get; set; }
        public string HCategoryCategoryName { get; set; }
        public string VCategoryCategoryName { get; set; }

        /// <summary>
        /// Returns true only if EXACTLY one of the options is true
        /// </summary>
        public bool IsOnlyOneOptionSelected
        {
            get
            {
                return ForAll ^ ProductId.HasValue ^ VCategoryId.HasValue ^ HCategoryId.HasValue;
            }
        }
    }
}
