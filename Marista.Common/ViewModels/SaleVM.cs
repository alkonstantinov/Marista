using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.ViewModels
{
    public class SaleVM
    {
        public int SaleId { get; set; }

        public string CustomerName { get; set; }

        public DateTime OnDate { get; set; }

        public IList<SaleDetailVM> SaleDetails { get; set; }

        public decimal Total { get; set; }

        public int ItemCount { get; set; }
    }    
}
