using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Marista.Common.ViewModels
{
    public class SearchBPVM
    {
        public string SS { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        [Display(Name = "Country")]
        public string CountryId { get; set; }

        [Display(Name = "Mail Text")]
        public string MailText { get; set; }


        public IList<BPRegVM> BPs { get; set; }
    }
}
