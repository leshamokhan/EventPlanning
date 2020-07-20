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
    [Authorize(Roles = "Пользователь")]
    public class EventUserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EventUser
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

        public bool UserSubscribedBool(int EventID, int UserProfileID)
        {
            try
            {
                UserProfile userProfileFind = db.UserProfiles.Include(m => m.Events).First(m => m.Id == UserProfileID);
                Event @event = userProfileFind.Events.First(m => m.Id == EventID);
                return true;
            }
            catch
            {
                return false;
            }            
        }


        public ActionResult GetAll()
        {
            try
            {
                bool state;
                string userIdentityID = User.Identity.GetUserId().ToString();
                UserProfile userProfile = db.UserProfiles.Where(s => s.UserID == userIdentityID).FirstOrDefault();

                DateTime Date = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                var viewEvent = db.Events.Include(m => m.Fields).Include(m => m.UserProfiles).Include(m => m.UserProfileCreate).Where(m => m.Date >= Date).ToList();
                List<EventView> EventView = new List<EventView>();

                foreach(Event ev in viewEvent)
                {
                    if(UserSubscribedBool(ev.Id, userProfile.Id))
                    {
                        state = true;
                    }
                    else
                    {
                        state = false;
                    }
                    EventView.Add(new EventView() { Id = ev.Id, Name = ev.Name, Count = ev.Count, Date = ev.Date, Time = ev.Time , State = state, Fields = ev.Fields , UserProfileCreate = ev.UserProfileCreate, CountNow = ev.UserProfiles.Count});
                }
                return Json((from obj in EventView select new { Id = obj.Id, Name = obj.Name, Date = obj.Date.ToShortDateString(), Time = obj.Time.ToShortTimeString(), Count = obj.Count, State = obj.State , Fields = from o in obj.Fields select new { Name = o.Name, Description = o.Description } , UserCreate = obj.UserProfileCreate.LastName + ' ' + obj.UserProfileCreate.Name + ' ' + obj.UserProfileCreate.MiddleName , CountNow = obj.CountNow }), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Subscribe(int id)
        {
            try
            {
                string userIdentityID = User.Identity.GetUserId().ToString();
                UserProfile userProfile = db.UserProfiles.Where(s => s.UserID == userIdentityID).FirstOrDefault();
                Event @event = db.Events.Include(m => m.UserProfiles).Where(m => m.Id == id).FirstOrDefault();

                if(@event.Count == @event.UserProfiles.Count)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

                @event.UserProfiles.Add(userProfile);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }            
        }


        public ActionResult UnSubscribe(int id)
        {
            try
            {
                string userIdentityID = User.Identity.GetUserId().ToString();
                UserProfile userProfileOne = db.UserProfiles.Where(s => s.UserID == userIdentityID).FirstOrDefault();
                UserProfile userProfileDel = db.UserProfiles.Include(m => m.Events).First(m =>m .Id == userProfileOne.Id);
                Event @event = userProfileDel.Events.First(m => m.Id == id);
                @event.UserProfiles.Remove(userProfileDel);
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
