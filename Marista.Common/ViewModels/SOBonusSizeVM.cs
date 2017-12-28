using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.ViewModels
{
    public class SOBonusSizeVM
    {
        public int SOBonusSizeId { get; set; }
        [Required]
        public int FromSO { get; set; }
        [Required]
        public int ToSO { get; set; }
        [Required]
        public decimal BonusPercent { get; set; }
    }
}
