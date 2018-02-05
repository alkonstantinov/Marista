using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.ViewModels
{
    public class SaleDetailVM
    {
        public int SaleDetailId { get; set; }

        public int SaleId { get; set; }


        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public int Quantity { get; set; }

        public decimal Total {
            get {
                return Price * Quantity * (100.0M - Discount) / 100.0M;
            }
        }
           
    }
}
