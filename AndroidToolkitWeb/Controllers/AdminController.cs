using AndroidToolkitWeb.DbContexts;
using AndroidToolkitWeb.Entities;
using AndroidToolkitWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Xml.Linq;
using System.IO;

namespace AndroidToolkitWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        protected readonly AdminPanelViewModel ViewModel;
        private readonly BlogDbContext _blogDb;
        private readonly BugReportsDbContext _bugReportsDb;
        private readonly ReviewsDbContext _reviewsDb;
        private readonly ApplicationDbContext _appDb;
        private BlogPost _latestBlogPost;
        private BugReport _latestBugReport;
        private Review _latestReview;
        public AdminController()
        {
            ViewModel = new AdminPanelViewModel();
            ViewModel.BugReports = new List<BugReport>();
            ViewModel.Reviews = new List<Review>();
            ViewModel.BlogPosts = new List<BlogPost>();
            ViewModel.Users = new List<ApplicationUser>();
            _appDb = new ApplicationDbContext();
            _blogDb = new BlogDbContext();
            _bugReportsDb = new BugReportsDbContext();
            _reviewsDb = new ReviewsDbContext();
        }

        public async Task<ActionResult> Panel()
        {
            var usernumber = await _appDb.Users.ToListAsync();
            ViewModel.RegisteredUsers = usernumber.Count;
            ViewBag.RegisteredUsers = ViewModel.RegisteredUsers;
            return View(ViewModel);
        }

        public PartialViewResult Upload()
        {
            return PartialView();
        }

        public async Task<PartialViewResult> Users()
        {
            ViewModel.Users = await _appDb.Users.ToListAsync();
            return PartialView(ViewModel);
        }


        public async Task<PartialViewResult> Latest()
        {
            await Task.WhenAll(LatestBlogPost(), LatestReview(), LatestBugReport());
            await Load();
            ViewBag.LatestBlogPost = _latestBlogPost;
            ViewBag.LatestReview = _latestReview;
            ViewBag.LatestBugReport = _latestBugReport;
            return PartialView(ViewModel);
        }

        public async Task<PartialViewResult> AndroidToolkit()
        {

            var doc = await ReadVersions();
            var versions = doc.Root.Descendants("AndroidToolkitDBVersion");
            foreach (var version in versions)
            {
                ViewModel.AndroidToolkitVersions.Add(new AndroidToolkitVersion() { Number = Convert.ToInt32(version.Attribute("Number").Value), Download = version.Attribute("Download").Value });
            }
            ViewModel.AndroidToolkitVersionsXML = doc.ToString();
            return PartialView(ViewModel);
        }

        public async Task<PartialViewResult> CreateVersion(AdminPanelViewModel vm)
        {
            if (await CheckVersions())
            {
                bool success = await InsertVersion(vm.NewAndroidToolkitVersion.Number, vm.NewAndroidToolkitVersion.Download);
                if (success)
                {
                    return PartialView("Success");
                }
                else
                {
                    return PartialView("Error");
                }
            }
            else
            {
                ModelState.AddModelError("", "Android Toolkit DB Version with same properties already exists.");
            }
            return PartialView("Error");
        }
        #region Tasks
        private async Task LatestBlogPost()
        {
            _latestBlogPost = await _blogDb.BlogPosts.OrderByDescending(p => p.Date).Take(1).FirstOrDefaultAsync();
        }
        private async Task LatestReview()
        {
            _latestReview = await _reviewsDb.Reviews.OrderByDescending(p => p.Date).Take(1).FirstOrDefaultAsync();
        }
        private async Task LatestBugReport()
        {
            _latestBugReport = await _bugReportsDb.BugReports.OrderByDescending(p => p.Date).Take(1).FirstOrDefaultAsync();
        }
        private async Task Load()
        {
            ViewModel.BugReports = await _bugReportsDb.BugReports.ToListAsync();
            ViewModel.Reviews = await _reviewsDb.Reviews.ToListAsync();
            ViewModel.BlogPosts = await _blogDb.BlogPosts.ToListAsync();
        }

        public async Task<ActionResult> MakeBugReportSolved(AdminPanelViewModel vm)
        {
            var bugreport = await _bugReportsDb.BugReports.FindAsync(vm.BugReportID);
            if (bugreport != null)
            {
                bugreport.IsSolved = true;
                _bugReportsDb.Entry(bugreport).State = EntityState.Modified;
                await _bugReportsDb.SaveChangesAsync();
                await Load();
                ViewBag.Success = true;
                return PartialView("Success");
            }
            return PartialView("Error");
        }

        #region Users
        public async Task<ActionResult> CreateUser(AdminPanelViewModel vm)
        {
            if (ModelState.IsValid)
            {
                PasswordHasher hasher = new PasswordHasher();
                _appDb.Users.Add(new ApplicationUser()
                {
                    Name = vm.NewUserModel.Name,
                    Surname = vm.NewUserModel.Surname,
                    UserName = vm.NewUserModel.UserName,
                    Email = vm.NewUserModel.Email,
                    PasswordHash = hasher.HashPassword(vm.NewUserModel.Password)
                });
                await _appDb.SaveChangesAsync();
                return PartialView("Success");
            }
            return PartialView("Error");
        }

        public async Task<ActionResult> ChangeUserUsername(AdminPanelViewModel vm)
        {
            var user2 = await _appDb.Users.Where(u => u.UserName == vm.EditUserModel.OldUserName).FirstOrDefaultAsync();
            if (user2 != null)
            {
                user2.UserName = vm.EditUserModel.NewUserName;
                await _appDb.SaveChangesAsync();
                return PartialView("Success");
            }
            return PartialView("Error");

        }

        public async Task<ActionResult> ChangeUserPassword(AdminPanelViewModel vm)
        {
            var user = await _appDb.Users.Where(u => u.UserName == vm.EditUserModel.UserName).FirstOrDefaultAsync();
            if (user != null)
            {
                PasswordHasher hasher = new PasswordHasher();
                user.PasswordHash = hasher.HashPassword(vm.EditUserModel.NewPassword);
                _appDb.Entry(user).State = EntityState.Modified;
                await _appDb.SaveChangesAsync();
                return PartialView("Success");
            }
            return PartialView("Error");
        }

        [Authorize(Users = "gboduljak")]
        public async Task<ActionResult> DeleteUser(AdminPanelViewModel vm)
        {
            var user = await _appDb.Users.Where(u => u.UserName == vm.DeleteUserModel.DeleteUserName).FirstOrDefaultAsync();
            if (user != null)
            {
                _appDb.Users.Remove(user);
                await _appDb.SaveChangesAsync();
                return PartialView("Success");
            }
            return PartialView("Error");
        }
        [Authorize(Users = "gboduljak")]
        public async Task<ActionResult> GiveAdmin(AdminPanelViewModel vm)
        {
            var user = await (from u in _appDb.Users
                              where u.UserName == vm.GiveAdminUsername
                              select u).FirstOrDefaultAsync();
            if (user != null)
            {
                var store = new UserStore<ApplicationUser>(_appDb);
                var manager = new UserManager<ApplicationUser>(store);
                await manager.AddToRoleAsync(user.Id.ToString(), "Admin");
                return PartialView("Success");
            }
            return PartialView("Error");
        }
        [Authorize(Users = "gboduljak")]
        public async Task<ActionResult> DeleteAdmin(AdminPanelViewModel vm)
        {
            var user = await (from u in _appDb.Users
                              where u.UserName == vm.DeleteAdminUsername
                              select u).FirstOrDefaultAsync();
            if (user != null)
            {
                var store = new UserStore<ApplicationUser>(_appDb);
                var manager = new UserManager<ApplicationUser>(store);
                await manager.RemoveFromRoleAsync(user.Id.ToString(), "Admin");
                return PartialView("Success");
            }
            return PartialView("Error");
        }
        #endregion

        private async Task<bool> CheckVersions()
        {
            var doc = await ReadVersions();
            var versions = doc.Root.Descendants("AndroidToolkitDBVersion");
            foreach (var version in versions)
            {
                ViewModel.AndroidToolkitVersions.Add(new AndroidToolkitVersion() { Number = Convert.ToInt32(version.Attribute("Number").Value), Download = version.Attribute("Download").Value });
            }
            return ViewModel.AndroidToolkitVersions.All(version => ViewModel.NewAndroidToolkitVersion.Number != version.Number && ViewModel.NewAndroidToolkitVersion.Download != version.Download);
        }

        private async Task<bool> InsertVersion(int number, string url)
        {
            try
            {
                var doc = await ReadVersions();
                XElement newNode = new XElement("AndroidToolkitDBVersion");
                newNode.Add(new XAttribute("Number", number));
                newNode.Add(new XAttribute("Download", url));
                doc.Root.Add(newNode);
                doc.Save(Server.MapPath(Path.Combine("~/Content", "Versions.xml")));
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<XDocument> ReadVersions()
        {
            var xdoc = await Task.Factory.StartNew(() => XDocument.Load(Server.MapPath("~/Content/Versions.xml")));
            return xdoc;

        }


        #endregion

    
    }
}