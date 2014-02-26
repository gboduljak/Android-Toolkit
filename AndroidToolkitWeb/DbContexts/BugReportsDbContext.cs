using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using AndroidToolkitWeb.Entities;

namespace AndroidToolkitWeb.DbContexts
{
    public class BugReportsDbContext : DbContext
    {
        public BugReportsDbContext()
            : base("name=DefaultConnection")
        {

        }
        public DbSet<BugReport> BugReports { get; set; }


    }
}