using AndroidToolkitWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AndroidToolkitWeb.DbContexts;
using System.Data.Entity;
using System.Threading;
using System.IO;
using System.Net;
using Microsoft.AspNet.Identity;

namespace AndroidToolkitWeb.Controllers
{
    public class HomeController : Controller
    {
        #region Fields
        protected internal BlogPost _latestBlogPost;
        protected internal Review _latestReview;
        protected internal BugReport _latestBugReport;

        private readonly BlogDbContext _blogDb;
        private readonly BugReportsDbContext _bugReportsDb;
        private readonly ReviewsDbContext _reviewsDb;
        #endregion

        public HomeController()
        {
            _blogDb = new BlogDbContext();
            _bugReportsDb = new BugReportsDbContext();
            _reviewsDb = new ReviewsDbContext();
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
        #endregion

        public async Task<ActionResult> Index()
        {
            await Task.WhenAll(LatestBlogPost(), LatestReview(), LatestBugReport());
            ViewBag.LatestBlogPost = _latestBlogPost;
            ViewBag.LatestReview = _latestReview;
            ViewBag.LatestBugReport = _latestBugReport;
            return View();
        }

        public ActionResult Author()
        {
            ViewBag.Title = "Author";
            return View();
        }

        public ActionResult Toolkit()
        {
            ViewBag.Title = "Toolkit";
            return View();
        }

        public ActionResult Download()
        {
            return File(Path.Combine(Server.MapPath("~/Content/Uploads"), "AndroidToolkitPublic.zip"), "application/zip", "AndroidToolkitPublic.zip");
        }

        public ActionResult Versions()
        {
            return Content(System.IO.File.ReadAllText(Server.MapPath("~/Content/Download/Versions.xml")));
        }

        [Authorize]
        public ActionResult Chat()
        {
            return View();
        }
    }
}