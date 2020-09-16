using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TixGurusWeb.Models;

namespace TixGurusWeb.Controllers
{
    public class EventsController : Controller
    {
        private TixGurusDBEntities db = new TixGurusDBEntities();

        // GET: Events
        public ActionResult Index(string search, int? typeId)
        {
            ViewBag.EventType = new SelectList(db.EventTypes, "TypeID", "TypeName");
            var results = db.Events.Where(x => x.EventTitle.Contains(search) || search == null).Include(x => x.EventType);
            if (typeId != null)
            {
                results = results.Where(x => x.TypeID == typeId);
            }
            return View(results.ToList());
        }

        public ActionResult Book(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Create", "Bookings", new { id = @event.EventID });
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            ViewBag.TypeID = new SelectList(db.EventTypes, "TypeID", "TypeName");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Include = "EventID,EventTitle,EventLocation,EventDate,EventDescription,BasePrice, TypeID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                var seats = db.Seats;
                foreach (var seat in seats)
                {
                    SeatBooking sb = new SeatBooking
                    {
                        SeatID = seat.SeatID,
                        EventID = @event.EventID,
                        Booked = false
                    };
                    db.SeatBookings.Add(sb);
                }
                db.SaveChanges();
                ViewBag.TypeID = new SelectList(db.EventTypes, "TypeID", "TypeName", @event.TypeID);

                return RedirectToAction("Index");
            }

            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            ViewBag.TypeID = new SelectList(db.EventTypes, "TypeID", "TypeName", @event.TypeID);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit([Bind(Include = "EventID,EventTitle,EventLocation,EventDate,EventDescription,BasePrice,TypeID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TypeID = new SelectList(db.EventTypes, "TypeID", "TypeName", @event.TypeID);
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            var seatBookings = db.SeatBookings.Where(s => s.EventID == id);
            foreach (SeatBooking sb in seatBookings)
            {
                db.SeatBookings.Remove(sb);
            }

            var bookings = db.Bookings.Where(b => b.EventID == id);            
            foreach (Booking booking in bookings)
            {
                db.Bookings.Remove(booking);
            }
            db.Events.Remove(@event);
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
