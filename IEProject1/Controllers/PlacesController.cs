using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IEProject1.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IEProject1.Controllers
{
    public class PlacesController : Controller
    {
        private IEProjectEntities db = new IEProjectEntities();

        // GET: Places
        public ActionResult Index(string searchString)
        {
            Place place = new Place();

            //GetData(place);

            var places = db.Place.Include(p => p.Field).OrderByDescending(a => a.Rating);
            var ps = from s in places
                     select s;
            if (!(String.IsNullOrEmpty(searchString)))
            {
                ps = ps.Where(s => s.Adress.Contains(searchString));
            }
            return View(ps.ToList());
        }

        // GET: Places/Details/5
        public ActionResult Details(string id)
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
        public ActionResult Create([Bind(Include = "Id,Name,Adress,Rating,Total_rating_people,Photo_reference,Field_id")] Place place)
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
        public ActionResult Edit([Bind(Include = "Id,Name,Adress,Rating,Total_rating_people,Photo_reference,Field_id")] Place place)
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

        public void GetData(Place place)
        {
            //db.Places.clear();

            string url = @"https://maps.googleapis.com/maps/api/place/textsearch/json?query=fitness+center+in+caulfield+east&key=AIzaSyD9LUlNJjOHpdMmBFzYkLpuC91VlO5McLg";

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
                place.Id = item.place_id;
                place.Name = item.name;
                //place.Photo_reference = item.photos["photo_reference"];
                place.Rating = Convert.ToSingle(item.rating);
                place.Adress = item.formatted_address;
                place.Total_rating_people = Convert.ToInt32(item.user_ratings_total);

                Place place2 = db.Place.FirstOrDefault(m => m.Id == place.Id);

                if (place2 != null)
                {
                    db.Place.Add(place);
                    db.SaveChanges();
                }

            }

            response.Close();

        }
    }
}
