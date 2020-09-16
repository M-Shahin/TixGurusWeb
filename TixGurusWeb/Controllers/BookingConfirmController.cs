using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TixGurusWeb.Models;
using Console = System.Diagnostics.Debug;


namespace TixGurusWeb.Controllers
{
    public class BookingConfirmController : Controller
    {
        private TixGurusDBEntities db = new TixGurusDBEntities();

        // GET: BookingConfirm
        public ActionResult Index()
        {
            return View();
        }

        private EventSeatBookingModel getModel(int eventID, int seatBookingID, string level, string row, int seatNumber)
        {
          var id = eventID;
            var eventDetail = db.Events.Find(id);
            var levels = db.Seats.Select(s => s.SeatLevel).Distinct().Select(l => new SelectListItem { Text = l, Value = l });
            var rows = db.Seats
                        .Where(s => s.SeatLevel == level || level == null)
                        .Select(s => s.SeatRow).Distinct()
                        .Select(l => new SelectListItem { Text = l, Value = l });
            var eventBooking = new EventSeatBookingModel();
            var newBooking = new Booking();
            eventBooking.Booking = newBooking;
            eventBooking.LevelList = levels;
            eventBooking.EventDetails = eventDetail;

            eventBooking.UserName = (string)Session["userFullName"];
            eventBooking.SeatBookingID = seatBookingID;
            eventBooking.SeatNumber = seatNumber;
            if (level != null)
            {
                eventBooking.SelectedLevel = level;
                eventBooking.RowList = rows;
            }
            else
            {
                eventBooking.RowList = new[] { new SelectListItem { Value = "", Text = "" } };
            }

            if (row != null)
            {
                eventBooking.SelectedRow = row;
            }

            if (level == "L3")
            {
                eventBooking.ticketPrice = eventDetail.BasePrice;
            }
            else if (level == "L2")
            {
                eventBooking.ticketPrice = eventDetail.BasePrice * 1.5;
            }
            else
            {
                eventBooking.ticketPrice = eventDetail.BasePrice * 1.5 * 1.5;
            }
            return eventBooking;
        }


        public ActionResult BookingProceed(int eventID, int? seatBookingID, string level, string row, int? seatNumber)
        {
            var id = eventID;
            var eventDetail = db.Events.Find(id);
            if (eventDetail == null)
            {
                return HttpNotFound();
            }
            var levels = db.Seats.Select(s => s.SeatLevel).Distinct().Select(l => new SelectListItem { Text = l, Value = l });
            var rows = db.Seats
                        .Where(s => s.SeatLevel == level || level == null)
                        .Select(s => s.SeatRow).Distinct()
                        .Select(l => new SelectListItem { Text = l, Value = l });
           var eventBooking = new EventSeatBookingModel();
            var newBooking = new Booking();
            eventBooking.Booking = newBooking;
            eventBooking.LevelList = levels;
            eventBooking.EventDetails = eventDetail;
            eventBooking.UserName = (string)Session["userFullName"];
            eventBooking.SeatBookingID = seatBookingID;
            eventBooking.SeatNumber = seatNumber;
            if (level != null)
            {
                eventBooking.SelectedLevel = level;
                eventBooking.RowList = rows;
            }
            else
            {
                eventBooking.RowList = new[] { new SelectListItem { Value = "", Text = "" } };
            }

            if (row != null)
            {
                eventBooking.SelectedRow = row;
            }

            if (level == "L3")
            {
                eventBooking.ticketPrice = eventDetail.BasePrice;
            }
            else if (level == "L2")
            {
                eventBooking.ticketPrice = eventDetail.BasePrice * 1.5;
            }
            else
            { 
                eventBooking.ticketPrice = eventDetail.BasePrice * 1.5 * 1.5;
            }
        
            return View(eventBooking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult BookingProceed(EventSeatBookingModel model)
        {
            var id = model.EventDetails.EventID;
            var eventDetail = db.Events.Find(id);
            if (eventDetail == null)
            {
                return HttpNotFound();
            }

            var eventBooking = new EventSeatBookingModel();
            var newBooking = new Booking();
            eventBooking.EventDetails = eventDetail;

            eventBooking.UserName = (string)Session["userFullName"];
            eventBooking.SeatBookingID = model.SeatBookingID;
            eventBooking.SeatNumber = model.SeatNumber;
            eventBooking.SelectedLevel = model.SelectedLevel;
            eventBooking.SelectedRow = model.SelectedRow;
            eventBooking.CardType = model.CardType;
            eventBooking.CardNumber = model.CardNumber;
            eventBooking.SecurityNumber = model.SecurityNumber;

            if (model.SelectedLevel == "L3")
            {
                eventBooking.ticketPrice = eventDetail.BasePrice;
            }
            else if (model.SelectedLevel == "L2")
            {
                eventBooking.ticketPrice = eventDetail.BasePrice * 1.5;
            }
            else
            {
                eventBooking.ticketPrice = eventDetail.BasePrice * 1.5 * 1.5;
            }
            int? userID = (int?)Session["userID"];
            if (userID != null && userID > 0)
            {
                model.DisplayName = model.UserName;
            }

            //For Museum
            if (model.SeatBookingID == null)
            {
                model.SeatBookingID = 0;
            }
           

            if (!eventDetail.EventLocation.ToUpper().Contains("MUSEUM"))
            {
                if (!ModelState.IsValid)
                {
                    return View(eventBooking);
                }
            }

            Booking booking = new Booking();
            booking.EventID = eventBooking.EventDetails.EventID;

            if (userID != null && userID > 0)
            {
                booking.UserID = userID.GetValueOrDefault();
                booking.DisplayName = eventBooking.UserName;
                eventBooking.DisplayName = eventBooking.UserName;
            }
            else
            {
                booking.UserID = null;
                booking.DisplayName = model.DisplayName;
                eventBooking.DisplayName = model.DisplayName;
            }
            booking.SeatBookingID = eventBooking.SeatBookingID;
            booking.Booked = true;
            booking.BookingDate = DateTime.Now;
            db.Bookings.Add(booking);
            SeatBooking seatBooking = db.SeatBookings.FirstOrDefault(x => x.SeatBookingID == eventBooking.SeatBookingID);
            if (seatBooking != null)
            {
                seatBooking.Booked = true;
            }
            try
            {
                // Your code...
                // Could also be before try if you know the exception occurs in SaveChanges

                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
                throw;
            }
            eventBooking.Booking = booking;
            eventBooking.EventDetails = eventDetail;
            TempData["BookingData"] = eventBooking;
            return RedirectToAction("ConfirmBooking");
        }

        public ActionResult ConfirmBooking()
        {
            EventSeatBookingModel model = (EventSeatBookingModel)TempData["BookingData"];
            return View(model);
        }

    }
}