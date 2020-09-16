using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TixGurusWeb.Models;


namespace TixGurusWeb.Controllers
{

    public class ReportController : Controller
    {
        private TixGurusDBEntities db = new TixGurusDBEntities();

        // GET: Report
        public ActionResult Index(string topEvent, string dateRange)
        {
            var events = db.Events;
            BookingReportList reportList = new BookingReportList();
            List<BookingReport> reports = new List<BookingReport>();
            DateTime fromDate;
            if ((topEvent != null && topEvent == "weekly") || (dateRange != null && dateRange == "weekly"))
            {
                fromDate = DateTime.Today.AddDays(-7);
            }
            else if ((topEvent != null && topEvent == "monthly") || (dateRange != null && dateRange == "monthly"))
            {
                fromDate = DateTime.Today.AddDays(-30);
            }
            else
            {
                fromDate = DateTime.Today.AddDays(-365);
            }

            foreach (var eventDetail in events)
            {
                BookingReport bookingReport = new BookingReport();
                bookingReport.eventDetails = eventDetail;
                var bookings = db.Bookings.Where(x => x.EventID == eventDetail.EventID);
                if (topEvent != null || dateRange != null)
                {
                    bookings = bookings.Where(b => b.BookingDate >= fromDate && b.BookingDate <= DateTime.Today);
                }
                bookingReport.count = bookings.Count();
                double totalSale = 0.0;
                foreach (var booking in bookings)
                {
                    if (!eventDetail.EventLocation.ToUpper().Contains("MUSEUM"))
                    {
                        var seatBooking = db.SeatBookings.Find(booking.SeatBookingID);
                        var seat = db.Seats.Find(seatBooking.SeatID);
                        if (seat.SeatLevel == "L2")
                        {
                            totalSale += eventDetail.BasePrice * 1.5;
                        }
                        else if (seat.SeatLevel == "L1")
                        {
                            totalSale += eventDetail.BasePrice * 3;
                        }
                        else
                        {
                            totalSale += eventDetail.BasePrice;
                        }
                    }
                    else
                    {
                        totalSale += eventDetail.BasePrice;

                    }
                }
                bookingReport.totalSale = totalSale;
                reports.Add(bookingReport);
            }
            reports = reports.OrderByDescending(x => x.count).ToList();
            if (topEvent != null)
            {
                if (topEvent == "weekly" || topEvent == "monthly")
                {
                    if (reports.Count() > 5)
                    {
                        reports = (List<BookingReport>)reports.Take(5).ToList();
                    }
                }
                else
                {
                    if (reports.Count() > 10)
                    {
                        reports = (List<BookingReport>)reports.Take(10).ToList();
                    }
                }
            }
            reportList.Reports = reports;
            return View(reportList);
        }
    }
}