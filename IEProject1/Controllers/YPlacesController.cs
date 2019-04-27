using IEProject1.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace IEProject1.Controllers
{
    public class YPlacesController : Controller
    {
        private IEProject1Entities db = new IEProject1Entities();

        // GET: YPlaces
        public ActionResult Index(string searchString, string searchString1)
        {
            GetData(searchString, searchString1);

            YPlace yplace = new YPlace();

            var yplaces = db.YPlace.Include(p => p.Field).OrderByDescending(a => a.Rating);

            var ps = from s in yplaces
                     select s;
            if (!(String.IsNullOrEmpty(searchString) && String.IsNullOrEmpty(searchString1)))
            {
                ps = ps.Where(s => s.Adress.Contains(searchString) && s.Photo_reference.Contains(searchString1));
            }

            YPlace pl = ps.FirstOrDefault();

            if (pl == null)
            {
                ViewData["Message"] = "Sorry, there is no clubs in your suburb!You can select Melbourne to see all";
            }
            else
            {
                ViewData["Message"] = null;
            }
            ViewData["Places"] = ps;

            using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/yogaTwitterSearch.json")))
            {

                string result = reader.ReadToEnd();
                //var json = JObject.Parse(result);
                var json = JObject.Parse(result);
                var trend = json["statuses"];

                var stwitters = new List<Stwitter>();

                dynamic dynJson = JsonConvert.DeserializeObject(trend.ToString());

                foreach (var item in dynJson)
                {
                    Stwitter stwitter = new Stwitter();
                    stwitter.Screen_name = item["user"]["screen_name"];
                    stwitter.Created_at = item["created_at"];
                    stwitter.Url = item["user"]["url"];
                    stwitter.Text = item["text"];
                    stwitter.User_description = item["user"]["description"];
                    stwitter.Followers_count = item["user"]["followers_count"];
                    stwitter.Friends_count = item["user"]["friends_count"];
                    stwitter.Profile_image_url = item["user"]["profile_image_url"];

                    if(!String.IsNullOrEmpty(stwitter.Url))
                    {
                        stwitters.Add(stwitter);
                    }
                    

                }
                //var trends = JsonConvert.DeserializeObject<List<Trend>>(trend).Take(12);

                ViewData["sportsTwitter"] = stwitters;
            }

            return View();
        }

        // GET: YPlaces/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            YPlace yPlace = db.YPlace.Find(id);
            if (yPlace == null)
            {
                return HttpNotFound();
            }
            return View(yPlace);
        }

        // GET: YPlaces/Create
        public ActionResult Create()
        {
            ViewBag.Field_id = new SelectList(db.Field, "Id", "FieldName");
            return View();
        }

        // POST: YPlaces/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Adress,Rating,latitude,longitude,Total_rating_people,Photo_reference,Field_id")] YPlace yPlace)
        {
            if (ModelState.IsValid)
            {
                db.YPlace.Add(yPlace);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Field_id = new SelectList(db.Field, "Id", "FieldName", yPlace.Field_id);
            return View(yPlace);
        }

        // GET: YPlaces/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            YPlace yPlace = db.YPlace.Find(id);
            if (yPlace == null)
            {
                return HttpNotFound();
            }
            ViewBag.Field_id = new SelectList(db.Field, "Id", "FieldName", yPlace.Field_id);
            return View(yPlace);
        }

        // POST: YPlaces/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Adress,Rating,latitude,longitude,Total_rating_people,Photo_reference,Field_id")] YPlace yPlace)
        {
            if (ModelState.IsValid)
            {
                db.Entry(yPlace).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Field_id = new SelectList(db.Field, "Id", "FieldName", yPlace.Field_id);
            return View(yPlace);
        }

        // GET: YPlaces/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            YPlace yPlace = db.YPlace.Find(id);
            if (yPlace == null)
            {
                return HttpNotFound();
            }
            return View(yPlace);
        }

        // POST: YPlaces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            YPlace yPlace = db.YPlace.Find(id);
            db.YPlace.Remove(yPlace);
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

        public void GetData(string input, string input1)
        {
            //db.Places.clear();
            //string url = @"https://maps.googleapis.com/maps/api/place/textsearch/json?query=fitness+center+in+caulfield+east&key=AIzaSyD9LUlNJjOHpdMmBFzYkLpuC91VlO5McLg";

            string str1 = "https://maps.googleapis.com/maps/api/place/textsearch/json?query=";
            if (String.IsNullOrEmpty(input1))
            {
                input1 = "yoga";
            }
            string type = input1.Replace(' ', '+');

            string inornear = "+near+";
            if (String.IsNullOrEmpty(input))
            {
                input = "melbourne";
            }
            string suburb = input.Replace(' ', '+');
            string key = "&key=AIzaSyD9LUlNJjOHpdMmBFzYkLpuC91VlO5McLg";
            string str = str1 + type + inornear + suburb + key;

            string url = @str;

            WebRequest request = WebRequest.Create(url);

            WebResponse response = request.GetResponse();

            Stream data = response.GetResponseStream();

            StreamReader reader = new StreamReader(data);

            // json-formatted string from maps api
            string responseFromServer = reader.ReadToEnd();

            var jsonObject = JObject.Parse(responseFromServer);
            var json = jsonObject["results"];

            dynamic dynJson = JsonConvert.DeserializeObject(json.ToString());

            foreach (var item in dynJson)
            {
                YPlace place = new YPlace();
                place.Id = item["id"];
                place.Name = item["name"];
                place.Photo_reference = input1;
                place.latitude = item["geometry"]["location"]["lat"];
                place.longitude = item["geometry"]["location"]["lng"];
                place.Rating = (decimal)Convert.ToSingle(item.rating);
                place.Adress = item.formatted_address;
                place.Total_rating_people = Convert.ToInt32(item.user_ratings_total);
                place.Field_id = 3;

                YPlace place2 = db.YPlace.FirstOrDefault(m => m.Id == place.Id /*&& m.Name == place.Name && m.Adress == place.Adress*/);

                if (place2 == null)
                {
                    db.YPlace.Add(place);
                    db.SaveChanges();
                }

            }
            response.Close();
        }
    }
}
