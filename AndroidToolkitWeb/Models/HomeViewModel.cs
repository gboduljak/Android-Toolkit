using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AndroidToolkitWeb.DbContexts;
using AndroidToolkitWeb.Entities;

namespace AndroidToolkitWeb.Models
{
    public class HomeViewModel
    {
        public Review LatestReview { get; set; }
        public BugReport LatestBugReport { get; set; }

    }
}