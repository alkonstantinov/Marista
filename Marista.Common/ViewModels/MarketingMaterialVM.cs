using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.ViewModels
{
    public class MarketingMaterialVM
    {

        public int? MarketingMaterialId { get; set; }

        public string Title { get; set; }

        public string FileName { get; set; }

        public byte[] Content { get; set; }
    }
}
