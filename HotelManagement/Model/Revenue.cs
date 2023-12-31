//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HotelManagement.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Revenue
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Revenue()
        {
            this.RevenueProducts = new HashSet<RevenueProduct>();
            this.RevenueRoomTypes = new HashSet<RevenueRoomType>();
        }
    
        public int RevenueId { get; set; }
        public Nullable<int> Month { get; set; }
        public Nullable<int> Year { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RevenueProduct> RevenueProducts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RevenueRoomType> RevenueRoomTypes { get; set; }
    }
}
