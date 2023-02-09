//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataRepository
{
    using System;
    using System.Collections.Generic;
    
    public partial class Publication
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Publication()
        {
            this.Comments1 = new HashSet<Comment>();
            this.Categories = new HashSet<Category>();
            this.OwnerUser = new HashSet<User>();
        }
    
        public int PublicationId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public System.DateTime DateOfCreated { get; set; }
        public string HeaderPath { get; set; }
        public int StatusId { get; set; }
        public string Guid { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Category> Categories { get; set; }
        public virtual Status Status { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> OwnerUser { get; set; }
        public virtual User User { get; set; }
    }
}
