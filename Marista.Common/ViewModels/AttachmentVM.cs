using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.ViewModels
{
    public class AttachmentVM
    {
        public string Guid { get; set; }

        public string Filename { get; set; }

        public byte[] Content { get; set; }
    }
}
