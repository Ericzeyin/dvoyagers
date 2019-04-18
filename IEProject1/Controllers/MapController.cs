using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IEProject1.Controllers
{
    public class MapController : Controller
    {
        // GET: Map
        public ActionResult Index(float Lat, float Lng)
        {
            float[] location = { Lat, Lng };
            ViewBag.location = location;
            return View();
        }

        public ActionResult Details()
        {
            return View();
        }
    }
}