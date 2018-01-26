using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.ViewModels
{
    public class MyTeamReportVM
    {
        public PyramidResultVM MyResult { get; set; }

        public IList<TeamReportVM> Team { get; set; }

        public IList<SaleVM> Sales { get; set; }
    }
}
