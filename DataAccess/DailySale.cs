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
    
    public partial class DailySale
    {
        public int Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public System.DateTime Date { get; set; }
    
        public virtual User AspNetUser { get; set; }
    }
}
