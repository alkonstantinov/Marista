using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.ViewModels
{
    public class MyProfileReportVM
    {
        public string LeaderName { get; set; }

        public string LeaderEmail { get; set; }

        public IList<SaleVM> Sales { get; set; }

        public IList<BonusVM> FirstLevel { get; set; }

        public IList<BonusVM> Bonuses { get; set; }

        public IList<BonusVM> BonusesPerYear { get; set; }
    }
}
