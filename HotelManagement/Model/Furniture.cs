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
    
    public partial class Furniture
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Furniture()
        {
            this.FurnitureReceiptDetails = new HashSet<FurnitureReceiptDetail>();
            this.RoomTypeFurnitures = new HashSet<RoomTypeFurniture>();
        }
    
        public string FurnitureId { get; set; }
        public string FurnitureName { get; set; }
        public string FurnitureType { get; set; }
        public byte[] FurnitureAvatar { get; set; }
        public Nullable<int> QuantityOfStorage { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FurnitureReceiptDetail> FurnitureReceiptDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoomTypeFurniture> RoomTypeFurnitures { get; set; }
    }
}
