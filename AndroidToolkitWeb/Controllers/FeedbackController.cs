using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using AndroidToolkitWeb.DbContexts;
using AndroidToolkitWeb.Entities;
using System.Net;

namespace AndroidToolkitWeb.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly BugReportsDbContext _reports;
        private readonly ReviewsDbContext _reviews;

        public FeedbackController()
        {
            _reports = new BugReportsDbContext();
            _reviews = new ReviewsDbContext();
        }

        public async Task<ActionResult> Reports()
        {
            return View(await _reports.BugReports.OrderByDescending(m => m.Date).ToListAsync());
        }
        public async Task<ActionResult> Reviews()
        {
            return View(await _reviews.Reviews.OrderByDescending(m => m.Date).ToListAsync());
        }

        #region Reviews
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create([Bind(Include = "ID,Name,RatingDescription,Rating,Author,Date")]Review review)
        {
            if (ModelState.IsValid)
            {
                review.Date = DateTime.Now;
                review.Author = User.Identity.Name;
                _reviews.Reviews.Add(review);
                await _reviews.SaveChangesAsync();
                return RedirectToAction("Reviews", "Feedback");
            }
            return View(review);
        }

        public async Task<ActionResult> Review(int id = 0, string name = null)
        {

            var review = await _reviews.Reviews.Where(r => r.ID == id).Where(r => r.Name == name).FirstOrDefaultAsync();
            if (review != null)
            {
                return View(review);
            }
            else
            {
                return HttpNotFound();
            }
        }

        #endregion

        #region BugReports

        [Authorize]
        public ActionResult Report()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Report([Bind(Include = "ID,Name,Description,ErrorLog,Date")] BugReport report)
        {
            if (ModelState.IsValid)
            {
                report.Date = DateTime.Now;
                report.IsSolved = false;
                _reports.BugReports.Add(report);
                await _reports.SaveChangesAsync();
                return RedirectToAction("Reports");
            }
            return View(report);
        }

        public async Task<ActionResult> BugReport(int? id, string name = null)
        {
            if (id == null && name == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BugReport report = await _reports.BugReports.Where(r => r.ID == id).Where(r => r.Name == name).FirstOrDefaultAsync();

            if (report == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            return View("BugReportDetails",report);
        }

        #endregion
    }
}