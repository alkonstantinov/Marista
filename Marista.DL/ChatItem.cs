//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Marista.DL
{
    using System;
    using System.Collections.Generic;
    
    public partial class ChatItem
    {
        public int ChatItemId { get; set; }
        public int ChatId { get; set; }
        public int SiteUserId { get; set; }
        public System.DateTime OnDate { get; set; }
        public string Said { get; set; }
        public byte[] Attachment { get; set; }
    
        public virtual Chat Chat { get; set; }
        public virtual SiteUser SiteUser { get; set; }
    }
}