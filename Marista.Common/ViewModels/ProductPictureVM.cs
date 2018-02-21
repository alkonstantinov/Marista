using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.ViewModels
{
    public class ProductPictureVM
    {
        public int ProductPictureId { get; set; }
        public int ProductId { get; set; }
        [Display(Name = "Picture")]
        public byte[] Picture { get; set; }
    }
}
