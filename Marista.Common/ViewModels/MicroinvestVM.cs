using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.ViewModels
{
    public class MicroinvestVM
    {
        public int SaleId { get; set; }

        public DateTime OnDate { get; set; }

        public string CustomerName { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public string CountryName { get; set; }

        public decimal Total { get; set; }


    }
}
