using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EventPlanning.Models
{
    public class DbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var roleUser = new IdentityRole { Name = "Пользователь" };
            var roleAdmin = new IdentityRole { Name = "Администратор" };
            roleManager.Create(roleUser);
            roleManager.Create(roleAdmin);


            var admin = new ApplicationUser { Email = "admin@mail.ru", UserName = "admin@mail.ru", EmailConfirmed = true };
            string password = "admin@mail.ru";
            var result = userManager.Create(admin, password);

            if (result.Succeeded)
            {
                userManager.AddToRole(admin.Id, roleAdmin.Name);
            }

            UserProfile userProfile = new UserProfile { UserID = admin.Id, Name = "Аввакуум", LastName = "Некрасов"};
            context.UserProfiles.Add(userProfile);




            var user = new ApplicationUser { Email = "user@mail.ru", UserName = "user@mail.ru", EmailConfirmed = true };
            password = "user@mail.ru";
            var userresult = userManager.Create(user, password);

            if (userresult.Succeeded)
            {
                userManager.AddToRole(user.Id, roleUser.Name);
            }

            userProfile = new UserProfile { UserID = user.Id, Name = "Иван", LastName = "Максимов" };
            context.UserProfiles.Add(userProfile);

            context.SaveChanges();

            base.Seed(context);
        }
    }
}