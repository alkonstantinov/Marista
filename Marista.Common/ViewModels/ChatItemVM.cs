using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.ViewModels
{
    public class ChatItemVM
    {
        public int ChatId { get; set; }
        public int ChatItemId { get; set; }
        public int SiteUserId { get; set; }
        public string SiteUserUsername { get; set; }
        public DateTime OnDate { get; set; }
        public string Said { get; set; }
        public string FileName { get; set; }
        public byte[] Attachment { get; set; }
    }
}
