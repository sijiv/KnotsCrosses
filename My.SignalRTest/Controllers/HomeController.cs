using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace My.SignalRTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult TestChat()
        {
            ViewBag.Message = "My Chat page.";

            return View();
        }
        [Authorize]
        public ActionResult KnotsAndCross()
        {
            ViewBag.Message = "Knots & Crosses Game Challenge";

            return View();
        }
    }
}