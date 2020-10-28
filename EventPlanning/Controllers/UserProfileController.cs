using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using EventPlanning.Models;
using Microsoft.AspNet.Identity;

namespace EventPlanning.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserProfile
        public ActionResult Index()
        {
            string UserIdentityId = User.Identity.GetUserId().ToString();
            UserProfile userProfile = db.UserProfiles.Where(m => m.UserID == UserIdentityId).FirstOrDefault();
            return View(userProfile);
        }


        // GET: UserProfile/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProfile userProfile = db.UserProfiles.Find(id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            return View(userProfile);
        }

        // POST: UserProfile/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,LastName,MiddleName,Age")] UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                string userIdentityID = User.Identity.GetUserId().ToString();
                userProfile.UserID = userIdentityID;
                db.Entry(userProfile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userProfile);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Import(HttpPostedFileBase fileExcel)
        {
            if (fileExcel != null)
            {
                string UserIdentityId = User.Identity.GetUserId().ToString();
                UserProfile userProfile = db.UserProfiles.Where(m => m.UserID == UserIdentityId).FirstOrDefault();
                
                using (XLWorkbook workBook = new XLWorkbook(fileExcel.InputStream, XLEventTracking.Disabled))
                {
                    string LastName = workBook.Worksheet(1).Column(2).Cell(1).Value.ToString();
                    string Name = workBook.Worksheet(1).Column(2).Cell(2).Value.ToString();
                    string MiddleName = workBook.Worksheet(1).Column(2).Cell(3).Value.ToString();
                    int Age = Convert.ToInt32(workBook.Worksheet(1).Column(2).Cell(4).Value.ToString());
                    userProfile.Name = Name;
                    userProfile.LastName = LastName;
                    userProfile.MiddleName = MiddleName;
                    userProfile.Age = Age;
                }
                db.Entry(userProfile).State = EntityState.Modified;
                db.SaveChanges();
            }            
            return RedirectToAction("Index", "UserProfile");            
        }

        public ActionResult Export()
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var worksheet = workbook.Worksheets.Add("Event Planner");

                worksheet.Cell("A1").Value = "Фамилия";
                worksheet.Cell("A2").Value = "Имя";
                worksheet.Cell("A3").Value = "Отчество";
                worksheet.Cell("A4").Value = "Возраст";
                worksheet.Column(1).Style.Font.Bold = true;
                
                worksheet.Cells("A1:B4").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cells("A1:B4").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                worksheet.Cells("A1:B4").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                worksheet.Cells("A1:B4").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"example_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }
    }
}
