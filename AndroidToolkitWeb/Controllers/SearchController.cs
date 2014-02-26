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
    public class SearchController : Controller
    {
        private readonly SearchHelper _searchHelper;
        public SearchController()
        {
            _searchHelper = new SearchHelper();
        }
        public async Task<PartialViewResult> SearchAll(string term)
        {
            ViewBag.Style = "Global";
            ViewBag.Term = term;
            var model = await Task.Factory.StartNew(() => _searchHelper.Search(true, true, true, term));
            return PartialView("Search", model);
        }
     
        public async Task<PartialViewResult> SearchBlog(string term)
        {
            ViewBag.Term = term;
            ViewBag.Style = "Blog";
            var model = await Task.Factory.StartNew(() => _searchHelper.Search(false, false, true, term));
            return PartialView("Search", model);
        }
    }
}