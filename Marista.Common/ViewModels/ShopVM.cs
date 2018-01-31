using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Marista.Common.ViewModels
{
    public class ShopVM
    {
        public int SortById { get; set; }
        public List<SelectListItem> SortBy { get; set; }

        public bool ShowList { get; set; }

        public int? HCategoryId { get; set; }
        public List<SelectListItem> HCategory { get; set; }

        public int? VCategoryId { get; set; }
        public List<SelectListItem> VCategory { get; set; }

        public int PageSizeId { get; set; }
        public List<SelectListItem> PageSize { get; set; }

        public int Page { get; set; }
        public int PagesCount { get; set; }

        public IList<ShopItemVM> Products { get; set; }




        public ShopVM()
        {
            this.SortBy = new List<SelectListItem>();
            this.SortBy.Add(new SelectListItem() { Text = "Name", Value = "1" });
            this.SortBy.Add(new SelectListItem() { Text = "Price", Value = "2" });
            this.SortById = 1;

            this.PageSize = new List<SelectListItem>();
            this.PageSize.Add(new SelectListItem() { Text = "12", Value = "12" });
            this.PageSize.Add(new SelectListItem() { Text = "24", Value = "24" });
            this.PageSize.Add(new SelectListItem() { Text = "36", Value = "36" });
            this.PageSizeId = 12;

            this.Page = 1;

        }
    }
}
