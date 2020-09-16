using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TixGurusWeb.Models;

namespace TixGurusWeb.Controllers
{
    public class SeatBookingsController : Controller
    {
        private TixGurusDBEntities db = new TixGurusDBEntities();

        // GET: SeatBookings
        public ActionResult Index()
        {
            var seatBookings = db.SeatBookings.Include(s => s.Event).Include(s => s.Seat).Where(s => s.EventID == '2');
            return View(seatBookings.ToList());
        }

        // GET: SeatBookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SeatBooking seatBooking = db.SeatBookings.Find(id);
            if (seatBooking == null)
            {
                return HttpNotFound();
            }
            return View(seatBooking);
        }

        // GET: SeatBookings/Create
        public ActionResult Create()
        {
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventTitle");
            ViewBag.SeatID = new SelectList(db.Seats, "SeatID", "SeatNumber");
            return View();
        }

        // POST: SeatBookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SeatBookingID,EventID,Booked,SeatID")] SeatBooking seatBooking)
        {
            if (ModelState.IsValid)
            {
                db.SeatBookings.Add(seatBooking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventTitle", seatBooking.EventID);
            ViewBag.SeatID = new SelectList(db.Seats, "SeatID", "SeatNumber", seatBooking.SeatID);
            return View(seatBooking);
        }

        // GET: SeatBookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SeatBooking seatBooking = db.SeatBookings.Find(id);
            if (seatBooking == null)
            {
                return HttpNotFound();
            }
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventTitle", seatBooking.EventID);
            ViewBag.SeatID = new SelectList(db.Seats, "SeatID", "SeatNumber", seatBooking.SeatID);
            return View(seatBooking);
        }

        // POST: SeatBookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SeatBookingID,EventID,Booked,SeatID")] SeatBooking seatBooking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(seatBooking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventTitle", seatBooking.EventID);
            ViewBag.SeatID = new SelectList(db.Seats, "SeatID", "SeatNumber", seatBooking.SeatID);
            return View(seatBooking);
        }

        // GET: SeatBookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SeatBooking seatBooking = db.SeatBookings.Find(id);
            if (seatBooking == null)
            {
                return HttpNotFound();
            }
            return View(seatBooking);
        }

        // POST: SeatBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SeatBooking seatBooking = db.SeatBookings.Find(id);
            db.SeatBookings.Remove(seatBooking);
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
