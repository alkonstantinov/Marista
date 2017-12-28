using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.ViewModels
{
    public class BonusSizeVM
    {
        public int BonusSizeId { get; set; }
        [Required]
        public decimal FromBonus { get; set; }
        [Required]
        public decimal ToBonus { get; set; }
        [Required]
        public decimal BonusPercent { get; set; }
    }
}
