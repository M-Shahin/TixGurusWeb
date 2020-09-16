//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TixGurusWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Booking
    {
        public int BookingID { get; set; }
        public int EventID { get; set; }
        public Nullable<int> UserID { get; set; }
        public bool Booked { get; set; }
        [DataType(DataType.Date)]
        public System.DateTime BookingDate { get; set; }
        public Nullable<int> SeatBookingID { get; set; }
        public string DisplayName { get; set; }
    
        public virtual Event Event { get; set; }
        public virtual SeatBooking SeatBooking { get; set; }
        public virtual User User { get; set; }
    }
}