using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TixGurusWeb.Models;

namespace TixGurusWeb.Controllers
{
    public class UsersController : Controller
    {
        private TixGurusDBEntities db = new TixGurusDBEntities();

        // GET: Users
        public ActionResult Index()
        {
            return View();
        }

        // GET: Users
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(TixGurusWeb.Models.User userModel)
        {
            var userDetails = db.Users.Where(user => user.UserName == userModel.UserName && user.Password == userModel.Password).FirstOrDefault();
            if (userDetails == null)
            {
                userModel.LoginErrorMessage = "Wrong user name or password.";
                return View("Login", userModel);
            }
            else
            {
                Session["userID"] = userDetails.UserID;
                Session["userName"] = userDetails.UserName;
                Session["userFullName"] = userDetails.Name;

                if (userDetails.Role.Equals("Admin"))
                {
                    Session["isAdmin"] = true;
                } else
                {
                    Session["isAdmin"] = false;
                }
                return RedirectToAction("Details", "Users");
                //return RedirectToAction("Index", "Events");
            }
        }

        // GET: Users/Details/5
        public ActionResult Details()
        {
            int? id = (int?)Session["userID"];
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var isAdmin = (bool)(Session["isAdmin"]);
            var booking = db.Bookings.Include(b => b.Event).Where(b => b.UserID == id);
            ViewBag.Booking = booking;
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,UserName,Name,Email,Address,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                Session["userID"] = user.UserID;
                Session["userName"] = user.UserName;
                Session["isAdmin"] = false;
                return RedirectToAction("Details");
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,UserName,Name,Email,Address,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
