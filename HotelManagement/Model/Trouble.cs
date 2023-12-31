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
    
    public partial class Trouble
    {
        public string TroubleId { get; set; }
        public string StaffId { get; set; }
        public string Title { get; set; }
        public byte[] Avatar { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> FixedDate { get; set; }
        public Nullable<System.DateTime> FinishDate { get; set; }
        public string Status { get; set; }
        public Nullable<double> Price { get; set; }
        public string Level { get; set; }
    
        public virtual Staff Staff { get; set; }
    }
}
