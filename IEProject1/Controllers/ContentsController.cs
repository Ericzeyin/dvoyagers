using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using IEProject1.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IEProject1.Controllers
{
    public class ContentsController : Controller
    {
        private IEProject1Entities db = new IEProject1Entities();

        // GET: Contents
        public ActionResult Index()
        {
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/twitterTrends.json")))
            {

                string result = reader.ReadToEnd();
                //var json = JObject.Parse(result);
                JArray jsonArray = JArray.Parse(result);
                var trend = jsonArray[0]["trends"].ToString();

                var trends = JsonConvert.DeserializeObject<List<Trend>>(trend).Take(12);

                ViewData["trends"] = trends;
            }


            var contents = db.Content.Include(c => c.Field).Where(s => s.Rank < 9).OrderBy(a => a.Rank);

            ViewData["contents"] = contents;
            return View();

           // return View(contents.ToList());
        }

        // GET: Contents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Content content = db.Content.Find(id);
            if (content == null)
            {
                return HttpNotFound();
            }
            return View(content);
        }

        // GET: Contents/Create
        public ActionResult Create()
        {
            ViewBag.TypeId = new SelectList(db.Field, "Id", "FieldName");
            return View();
        }

        // POST: Contents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ContentName,ImageName,Popularity,Rank,TypeId,Comment")] Content content)
        {
            if (ModelState.IsValid)
            {
                db.Content.Add(content);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TypeId = new SelectList(db.Field, "Id", "FieldName", content.TypeId);
            return View(content);
        }

        // GET: Contents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (id == 1)
            {
                ViewData["Message"] = "Places";
            }
            else if (id == 2)
            {
                ViewData["Message"] = "RPlaces";
            }
            else if (id == 3)
            {
                ViewData["Message"] = "YPlaces";
            }
            //else if (id == 4)
            //{
            //    ViewData["Message"] = "Culture";
            //}
            //Content content = db.Contents.Find(id);
            var content = db.Content.Where(s => s.TypeId == id);
            
            if (content == null)
            {
                return HttpNotFound();
            }
            ViewData["content"] = content;
            //ViewBag.TypeId = new SelectList(db.Fields, "Id", "FieldName", content.TypeId);
            //return View(content.ToList());
            return View();
        }

        // POST: Contents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ContentName,ImageName,Popularity,Rank,TypeId,Comment")] Content content)
        {
            if (ModelState.IsValid)
            {
                db.Entry(content).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TypeId = new SelectList(db.Field, "Id", "FieldName", content.TypeId);
            return View(content);
        }

        // GET: Contents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Content content = db.Content.Find(id);
            if (content == null)
            {
                return HttpNotFound();
            }
            return View(content);
        }

        // POST: Contents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Content content = db.Content.Find(id);
            db.Content.Remove(content);
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

        public ActionResult Page1()
        {

            using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/twitterTrends.json")))
            {

                string result = reader.ReadToEnd();
                //var json = JObject.Parse(result);
                JArray jsonArray = JArray.Parse(result);
                var trend = jsonArray[0]["trends"].ToString();

                var trends = JsonConvert.DeserializeObject<List<Trend>>(trend).Take(12);

                ViewData["trends"] = trends;
            }


            var contents = db.Content.Include(c => c.Field).Where(s => s.Rank < 9).OrderBy(a => a.Rank);

            ViewData["contents"] = contents;
            return View();

            // return View(contents.ToList());
        }

        public string GetTwitterTrends()
        {
            // oauth application keys
            string oauth_token = "278606942-WGdc2l5nXNtYENK14v3bBBQlUfZvYJSDfBFV8IZ1";
            string oauth_token_secret = "8gyATwlCkj26HlPhZCRfkWSu3OdVVg6bsL85jd3l1IFSQ";

            string oauth_consumer_key = "OhOhEUTcnQAnJpNg8NvzwY9bh";
            string oauth_consumer_secret = "qBgfNz1q9jrb1QFvuoXzpuT6H6qmeALpf2nd7MAIgJ56QSulm8";

            var oauth_version = "1.0";
            var oauth_signature_method = "HMAC-SHA1";
            var oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();
            var resource_url = "https://api.twitter.com/1.1/trends/place.json?id=1103816";

            //encrypted oAuth signature
            var baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
                "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}";

            var baseString = string.Format(baseFormat,
                                        oauth_consumer_key,
                                        oauth_nonce,
                                        oauth_signature_method,
                                        oauth_timestamp,
                                        oauth_token,
                                        oauth_version
                                        );

            baseString = string.Concat("GET&", Uri.EscapeDataString(resource_url),
                         "&", Uri.EscapeDataString(baseString));

            //Encrypt data

            var compositeKey = string.Concat(Uri.EscapeDataString(oauth_consumer_secret),
                        "&", Uri.EscapeDataString(oauth_token_secret));

            string oauth_signature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {
                oauth_signature = Convert.ToBase64String(hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }

            //Finish Authentification header

            var headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                   "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                   "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                   "oauth_version=\"{6}\"";

            var authHeader = string.Format(headerFormat,
                                    Uri.EscapeDataString(oauth_nonce),
                                    Uri.EscapeDataString(oauth_signature_method),
                                    Uri.EscapeDataString(oauth_timestamp),
                                    Uri.EscapeDataString(oauth_consumer_key),
                                    Uri.EscapeDataString(oauth_token),
                                    Uri.EscapeDataString(oauth_signature),
                                    Uri.EscapeDataString(oauth_version)
                            );

            //Disable exprect 100 continue header
            ServicePointManager.Expect100Continue = false;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resource_url);
            request.UseDefaultCredentials = true;
            request.PreAuthenticate = true;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Headers.Add("Authorization", authHeader);
            request.Method = "GET";

            WebResponse response = request.GetResponse();
            string responseData;

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                responseData = reader.ReadToEnd();
            }

            return responseData; // Or do whatever you want with the response
        }

    }


}
