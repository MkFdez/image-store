//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class Message
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int ChatId { get; set; }
        public System.DateTime Date { get; set; }
        public string Content { get; set; }
        public string FileId { get; set; }
    
        public virtual User AspNetUser { get; set; }
        public virtual AttachedFile AttachedFile { get; set; }
        public virtual Chat Chat { get; set; }
    }
}
