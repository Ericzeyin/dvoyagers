using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace IEProject1.Controllers
{
    public class OAuthController : Controller
    {
        // GET: OAuth
        public ActionResult Index()
        {
            return View();
        }

    }
}