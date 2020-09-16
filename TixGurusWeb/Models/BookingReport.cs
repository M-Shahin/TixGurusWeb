using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TixGurusWeb.Models;

namespace TixGurusWeb.Models
{
    public class BookingReport
    {
        public Event eventDetails { get; set; }
        public int count { get; set; }
        public double totalSale { get; set; }
    }
}