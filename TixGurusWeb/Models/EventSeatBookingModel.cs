using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace TixGurusWeb.Models
{
    public class EventSeatBookingModel
    {
        public Booking Booking { get; set; }
        public IEnumerable<SeatBooking> SeatList { get; set; }
        public Event EventDetails { get; set; }
        public IEnumerable<SelectListItem> LevelList { get; set; }
        public IEnumerable<SelectListItem> RowList { get; set; }
        public string SelectedLevel { get; set; }
        public string SelectedRow { get; set; }
        public string UserName { get; set; }
        public double ticketPrice { get; set; }
        public int? SeatNumber { get; set;  }
        public int? SeatBookingID { get; set; }
        [Required(ErrorMessage = "Please Provide Card Type")]
        public string CardType { get; set; }
        [RegularExpression("([0-9]+)", ErrorMessage = "Please Provide Your Card Number")]
        [Required(ErrorMessage = "Please Provide Your Card Number")]
        public string CardNumber { get; set; }
        //[DataAnnotationsExtensions.Integer(ErrorMessage = "Please Provide Your Card Security Code.")]
        [Required(ErrorMessage = "Please Provide Your Card Security Code")]
        public string SecurityNumber{ get; set; }
        [Required(ErrorMessage = "Please Provide Your Name", AllowEmptyStrings = false)]
        public string DisplayName { get; set; }
    }
}