using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TixGurusWeb.Models;

namespace TixGurusWeb.Controllers
{
    public class BookingsController : Controller
    {
        private TixGurusDBEntities db = new TixGurusDBEntities();
        private Event eventDetails;

        // GET: Bookings
        public ActionResult Index()
        {
            var bookings = db.Bookings.Include(b => b.Event).Include(b => b.User);
            return View(bookings.ToList());
        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // GET: Bookings/Create
        public ActionResult Create(int? id, string level, string row)
        {
            //ViewBag.EventID = new SelectList(db.Events, "EventID", "EventTitle");
            //ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }           
            var newBooking = new Booking();
            var eventDetail = db.Events.Find(id);
            eventDetails = eventDetail;
            if (eventDetail == null)
            {
                return HttpNotFound();
            }
            var levels = db.Seats.Select(s => s.SeatLevel).Distinct().Select(l => new SelectListItem { Text = l, Value = l });
            var rows = db.Seats
                        .Where(s => s.SeatLevel == level || level == null)
                        .Select(s => s.SeatRow).Distinct()
                        .Select(l => new SelectListItem { Text = l, Value = l });
            var seatBookings = db.SeatBookings
                                .Include(s => s.Event)
                                .Include(s => s.Seat)
                                .Where(s => s.EventID == id && (s.Seat.SeatLevel == level || level == null) && (s.Seat.SeatRow == row || row == null))
                                .ToList();
            var eventBooking = new EventSeatBookingModel();
            eventBooking.Booking = newBooking;
            eventBooking.SeatList = seatBookings;
            eventBooking.LevelList = levels;
            eventBooking.EventDetails = eventDetail;
            if (level != null)
            {
                eventBooking.SelectedLevel = level;
                eventBooking.RowList = rows;
            } else
            {
                eventBooking.RowList = new[] { new SelectListItem { Value = "", Text = "" } };
            }

            if (row != null)
            {
                eventBooking.SelectedRow = row;
            }
            if (level == "L1")
            {
                eventBooking.ticketPrice = eventDetail.BasePrice;
            } else if (level == "L2")
            {
                eventBooking.ticketPrice = eventDetail.BasePrice * 1.5;
            } else
            {
                eventBooking.ticketPrice = eventDetail.BasePrice * 1.5 * 1.5;
            }
            return View(eventBooking);
        }

        [HttpGet]
        public JsonResult GetRows(string level)
        {
            var rows = db.Seats
                       .Where(s => s.SeatLevel == level || level == null)
                       .Select(s => s.SeatRow).Distinct()
                       .Select(l => new SelectListItem { Text = l, Value = l });
            System.Diagnostics.Debug.WriteLine(rows);
            return new JsonResult { Data = rows.ToArray(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };// Json(rows.ToArray(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [MultipleButton(ActionName = "ShowSeats")]
        public ActionResult ShowSeats(EventSeatBookingModel model)
        {
            var id = model.EventDetails.EventID;
            var level = model.SelectedLevel;
            var row = model.SelectedRow; 
            var eventDetail = db.Events.Find(id);
            eventDetails = eventDetail;
            if (eventDetail == null)
            {
                return HttpNotFound();
            }
            var levels = db.Seats.Select(s => s.SeatLevel).Distinct().Select(l => new SelectListItem { Text = l, Value = l });
            var rows = db.Seats
                        .Where(s => s.SeatLevel == level || level == null)
                        .Select(s => s.SeatRow).Distinct()
                        .Select(l => new SelectListItem { Text = l, Value = l });
            var seatBookings = db.SeatBookings
                                .Include(s => s.Event)
                                .Include(s => s.Seat)
                                .Where(s => s.EventID == id && (s.Seat.SeatLevel == level || level == null) && (s.Seat.SeatRow == row || row == null))
                                .ToList();
            var eventBooking = new EventSeatBookingModel();
            var newBooking = new Booking();
            eventBooking.Booking = newBooking;
            eventBooking.SeatList = seatBookings;
            eventBooking.LevelList = levels;
            eventBooking.EventDetails = eventDetail;
            eventBooking.UserName = (string)Session["userName"];
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
            return View(eventBooking);
        }

        public ActionResult Book(int eventID, int? seatBookingID, string level, string row, int? seatNumber)
        {
            return RedirectToAction("BookingProceed", "BookingConfirm", new { eventID = eventID, seatBookingID = seatBookingID, level = level, row = row, seatNumber = seatNumber });
        }

        public ActionResult RowSelected(int id, string level, string row)
        {
            return RedirectToAction("Create", "Bookings", new { id = id, level = level, row = row });
        }
       
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventTitle", booking.EventID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName", booking.UserID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingID,EventID,UserID,Booked,BookingDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventTitle", booking.EventID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName", booking.UserID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
