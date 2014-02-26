namespace AndroidToolkitWeb.Migrations
{
    using AndroidToolkitWeb.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AndroidToolkitWeb.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "AndroidToolkitWeb.Models.ApplicationDbContext";
        }

        protected override void Seed(AndroidToolkitWeb.Models.ApplicationDbContext context)
        {
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Admin" };

                manager.Create(role);
            }

            if (!context.Users.Any(u => u.UserName == "gboduljak"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "gboduljak", PasswordHash = "gabideveloper8", Name = "Gabrijel", Surname = "Boduljak", Email = "gboduljak@outlook.com" };
                manager.Create(user, "thebest8");
                manager.AddToRole(user.Id.ToString(), "Admin");
            }
       
        }
    }
}
