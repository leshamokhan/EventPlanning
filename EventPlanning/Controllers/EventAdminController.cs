using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventPlanning.Models;
using Microsoft.AspNet.Identity;

namespace EventPlanning.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class EventAdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EventAdmin
        public ActionResult Index()
        {
            ViewBag.Message = Status();
            return View();
        }

        public bool Status()
        {
            string Userid = User.Identity.GetUserId();
            var user = db.UserProfiles.Where(p => p.UserID == Userid).FirstOrDefault();

            try
            {
                string Name = user.Name;
                string LastName = user.LastName;
                string MiddleName = user.MiddleName;
                int Age = (int)user.Age;

                if (Name != null && LastName != null && MiddleName != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return true;
            }
        }

        public ActionResult GetAll()
        {
            try
            {
                string userIdentityID = User.Identity.GetUserId().ToString();
                var userProfile = db.UserProfiles.Where(s => s.UserID == userIdentityID).FirstOrDefault();
                var view = db.Events.Include(m => m.Fields).Include(m => m.UserProfiles).Where(m => m.UserProfileCreateId == userProfile.Id).ToList();
                return Json((from obj in view select new { Id = obj.Id, Name = obj.Name, DateTime = obj.Date.ToShortDateString() + " " + obj.Time.ToShortTimeString(), Count = obj.Count, CountNow = obj.UserProfiles.Count,  Fields = from o in obj.Fields select new { Name = o.Name, Description = o.Description }, UserProfiles = from o in obj.UserProfiles select new { Name = o.Name, LastName = o.LastName, MiddleName = o.MiddleName } }), JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json(null, JsonRequestBehavior.AllowGet);

            }
        }


        // GET: EventAdmin/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: EventAdmin/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                List<Field> listField = @event.Fields.ToList();
                List<int> listIndex = new List<int>();

                foreach (Field modelField in listField)
                {
                    if(modelField.Name == null || modelField.Description == null)
                    {
                        int ind = listField.IndexOf(modelField);
                        listIndex.Add(ind);
                    }
                }

                for(int i = listIndex.Count - 1; i >= 0; i--)
                {
                    listField.RemoveAt(listIndex[i]);
                }

                @event.Fields = listField;

                string userIdentityID = User.Identity.GetUserId().ToString();
                var userProfile = db.UserProfiles.Where(s => s.UserID == userIdentityID).FirstOrDefault();
                @event.UserProfileCreateId = userProfile.Id;

                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@event);
        }

        

        // GET: EventAdmin/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Event @event = db.Events.Find(id);
        //    if (@event == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(@event);
        //}

        // POST: EventAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var @event = db.Events.Include(m => m.Fields).Where(m => m.Id == id);
                db.Events.RemoveRange(@event);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
