using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using AndroidToolkitWeb.Entities;

namespace AndroidToolkitWeb.DbContexts
{
    public class ReviewsDbContext : DbContext
    {
        public ReviewsDbContext()
            : base("name=DefaultConnection")
        {

        }
        public DbSet<Review> Reviews { get; set; }
    }
}