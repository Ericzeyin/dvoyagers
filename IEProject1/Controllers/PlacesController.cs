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
    public class PlacesController : Controller
    {
        private IEProject1Entities db = new IEProject1Entities();

        // GET: Places
        public ActionResult Index(string searchString)
        {
            GetData(searchString);

            Place place = new Place();

            var places = db.Place.Include(p => p.Field).Where(s => s.Field_id == 1).OrderByDescending(a => a.Rating);

            var ps = from s in places
                     select s;
            if (!(String.IsNullOrEmpty(searchString)))
            {
                ps = ps.Where(s => s.Adress.Contains(searchString));
            }

            Place pl = ps.FirstOrDefault();

            if (pl == null)
            {
                ViewData["Message"] = "Sorry, there is no club in your suburb!You can select Melbourne to see all";
            }
            else
            {
                ViewData["Message"] = null;
            }
            ViewData["Places"] = ps;

            using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/sportsTwitterSearch.json")))
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

                    stwitters.Add(stwitter);

                }
                //var trends = JsonConvert.DeserializeObject<List<Trend>>(trend).Take(12);

                ViewData["sportsTwitter"] = stwitters;
            }

            return View();
        }

        // GET: Places/Details/5
        //Get food type data
        public ActionResult Details(string searchString, string searchString1)
        {
            GetFoodData(searchString, searchString1);

            Place place = new Place();

            var places = db.Place.Include(p => p.Field).Where(s => s.Field_id == 4).OrderByDescending(a => a.Rating);

            var ps = from s in places
                     select s;
            if (!(String.IsNullOrEmpty(searchString) && String.IsNullOrEmpty(searchString1)))
            {
                ps = ps.Where(s => s.Adress.Contains(searchString) && s.Photo_reference.Contains(searchString1));
            }

            Place pl = ps.FirstOrDefault();

            if (pl == null)
            {
                ViewData["Message"] = "Sorry, there is no clubs in your suburb!You can select Melbourne to see all";
            }
            else
            {
                ViewData["Message"] = null;
            }
            ViewData["Places"] = ps;

            //using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/yogaTwitterSearch.json")))
            //{

            //    string result = reader.ReadToEnd();
            //    //var json = JObject.Parse(result);
            //    var json = JObject.Parse(result);
            //    var trend = json["statuses"];

            //    var stwitters = new List<Stwitter>();

            //    dynamic dynJson = JsonConvert.DeserializeObject(trend.ToString());

            //    foreach (var item in dynJson)
            //    {
            //        Stwitter stwitter = new Stwitter();
            //        stwitter.Screen_name = item["user"]["screen_name"];
            //        stwitter.Created_at = item["created_at"];
            //        stwitter.Url = item["user"]["url"];
            //        stwitter.Text = item["text"];
            //        stwitter.User_description = item["user"]["description"];
            //        stwitter.Followers_count = item["user"]["followers_count"];
            //        stwitter.Friends_count = item["user"]["friends_count"];
            //        stwitter.Profile_image_url = item["user"]["profile_image_url"];

            //        if (!String.IsNullOrEmpty(stwitter.Url))
            //        {
            //            stwitters.Add(stwitter);
            //        }


            //    }
            //    //var trends = JsonConvert.DeserializeObject<List<Trend>>(trend).Take(12);

            //    ViewData["sportsTwitter"] = stwitters;
            //}

            return View();
        }

        // GET: Places/Create
        public ActionResult Create()
        {
            ViewBag.Field_id = new SelectList(db.Field, "Id", "FieldName");
            return View();
        }

        // POST: Places/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Adress,Rating,latitude,longitude,Total_rating_people,Photo_reference,Field_id")] Place place)
        {
            if (ModelState.IsValid)
            {
                db.Place.Add(place);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Field_id = new SelectList(db.Field, "Id", "FieldName", place.Field_id);
            return View(place);
        }

        // GET: Places/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Place place = db.Place.Find(id);
            if (place == null)
            {
                return HttpNotFound();
            }
            ViewBag.Field_id = new SelectList(db.Field, "Id", "FieldName", place.Field_id);
            return View(place);
        }

        // POST: Places/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Adress,Rating,latitude,longitude,Total_rating_people,Photo_reference,Field_id")] Place place)
        {
            if (ModelState.IsValid)
            {
                db.Entry(place).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Field_id = new SelectList(db.Field, "Id", "FieldName", place.Field_id);
            return View(place);
        }

        // GET: Places/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Place place = db.Place.Find(id);
            if (place == null)
            {
                return HttpNotFound();
            }
            return View(place);
        }

        // POST: Places/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Place place = db.Place.Find(id);
            db.Place.Remove(place);
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

        public void GetData(string input)
        {
            //db.Places.clear();
            //string url = @"https://maps.googleapis.com/maps/api/place/textsearch/json?query=fitness+center+in+caulfield+east&key=AIzaSyD9LUlNJjOHpdMmBFzYkLpuC91VlO5McLg";

            string str1 = "https://maps.googleapis.com/maps/api/place/textsearch/json?query=";
            string type = "fitness+center+";
            string inornear = "in+";
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
                Place place = new Place();
                place.Id = item["id"];
                place.Name = item["name"];
                place.Photo_reference = "sports";
                place.latitude = item["geometry"]["location"]["lat"];
                place.longitude = item["geometry"]["location"]["lng"];
                place.Rating = (decimal)Convert.ToSingle(item.rating);
                place.Adress = item.formatted_address;
                place.Total_rating_people = Convert.ToInt32(item.user_ratings_total);
                place.Field_id = 1;

                Place place2 = db.Place.FirstOrDefault(m => m.Id == place.Id /*&& m.Name == place.Name && m.Adress == place.Adress*/);

                if (place2 == null)
                {
                    db.Place.Add(place);
                    db.SaveChanges();
                }

            }
            response.Close();

        }


        public void GetFoodData(string input, string input1)
        {
            //db.Places.clear();
            //string url = @"https://maps.googleapis.com/maps/api/place/textsearch/json?query=fitness+center+in+caulfield+east&key=AIzaSyD9LUlNJjOHpdMmBFzYkLpuC91VlO5McLg";

            string str1 = "https://maps.googleapis.com/maps/api/place/textsearch/json?query=";
            if (String.IsNullOrEmpty(input1))
            {
                input1 = "cafe";
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
                Place place = new Place();
                place.Id = item["id"];
                place.Name = item["name"];
                place.Photo_reference = input1;
                place.latitude = item["geometry"]["location"]["lat"];
                place.longitude = item["geometry"]["location"]["lng"];
                place.Rating = (decimal)Convert.ToSingle(item.rating);
                place.Adress = item.formatted_address;
                place.Total_rating_people = Convert.ToInt32(item.user_ratings_total);
                place.Field_id = 4;

                Place place2 = db.Place.FirstOrDefault(m => m.Id == place.Id /*&& m.Name == place.Name && m.Adress == place.Adress*/);

                if (place2 == null)
                {
                    db.Place.Add(place);
                    db.SaveChanges();
                }

            }
            response.Close();

        }

    }
}
