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
    
    public partial class ProductReceipt
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductReceipt()
        {
            this.ProductReceiptDetails = new HashSet<ProductReceiptDetail>();
        }
    
        public string ProductReceiptId { get; set; }
        public string StaffId { get; set; }
        public Nullable<double> Price { get; set; }
        public Nullable<System.DateTime> CreateAt { get; set; }
    
        public virtual Staff Staff { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductReceiptDetail> ProductReceiptDetails { get; set; }
    }
}
