using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TixGurusWeb.Models;

namespace TixGurusWeb.Models
{
    public class BookingReportList
    {
        public IEnumerable<BookingReport> Reports { get; set; }
    }
}