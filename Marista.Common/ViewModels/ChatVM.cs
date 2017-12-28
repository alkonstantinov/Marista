using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.ViewModels
{
    public class ChatVM
    {
        public int ChatId { get; set; }
        public int SiteUserId { get; set; }
        public string SiteUserUsername { get; set; }

        public ICollection<ChatItemVM> Items { get; set; }
    }
}
