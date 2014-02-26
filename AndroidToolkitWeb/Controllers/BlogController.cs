using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AndroidToolkitWeb.Entities;
using AndroidToolkitWeb.DbContexts;
using AndroidToolkitWeb.Helpers;
using AndroidToolkitWeb.Models;

namespace AndroidToolkitWeb.Controllers
{
    public class BlogController : Controller
    {
        private BlogDbContext db = new BlogDbContext();
        private readonly SearchHelper _searchHelper;

        public BlogController()
        {
            _searchHelper = new SearchHelper();
        }

        [Route("Blog/Posts")]
        public async Task<ActionResult> Index()
        {
            var model = await db.BlogPosts.OrderByDescending(b => b.Date).Take(15).ToListAsync();
            if (model.Count != 0)
            {
                ViewBag.LatestPost = model.ElementAt(0);
            }
            return View(model);
        }
             
        public async Task<ActionResult> Details(int? id)
        {
            ViewBag.LatestPost = await db.BlogPosts.OrderByDescending(p => p.Date).FirstOrDefaultAsync();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogpost = await db.BlogPosts.FindAsync(id);
            if (blogpost == null)
            {
                return HttpNotFound();
            }
            return View(blogpost);
        }


        [Authorize(Roles = "Admin")]
        [Route("Blog/Posts/Create")]
        public async Task<ActionResult> Create()
        {
            ViewBag.LatestPost = await db.BlogPosts.OrderByDescending(p => p.Date).FirstOrDefaultAsync();
            return View();
        }

        [Route("Blog/Posts/Create")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Name,Description,Post,Img1,Img2,Img3,Tags,Author,Date")] BlogPost blogpost)
        {
            if (ModelState.IsValid)
            {
                blogpost.Author = User.Identity.Name;
                blogpost.Date = DateTime.Now;
                db.BlogPosts.Add(blogpost);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(blogpost);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.LatestPost = await db.BlogPosts.OrderByDescending(p => p.Date).FirstOrDefaultAsync();
            BlogPost blogpost = await db.BlogPosts.FindAsync(id);
            if (blogpost == null)
            {
                return HttpNotFound();
            }
            return View(blogpost);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name,Description,Post,Img1,Img2,Img3,Tags,Author,Date")] BlogPost blogpost)
        {
            if (ModelState.IsValid)
            {
                blogpost.IsEdited = true;
                db.Entry(blogpost).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(blogpost);
        }
      

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogpost = await db.BlogPosts.FindAsync(id);
            if (blogpost == null)
            {
                return HttpNotFound();
            }
            return View(blogpost);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            BlogPost blogpost = await db.BlogPosts.FindAsync(id);
            db.BlogPosts.Remove(blogpost);
            await db.SaveChangesAsync();
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
    }
}
