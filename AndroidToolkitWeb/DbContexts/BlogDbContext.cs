using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using AndroidToolkitWeb.Entities;

namespace AndroidToolkitWeb.DbContexts
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext() : base("DefaultConnection")
        {

        }

        public DbSet<BlogPost> BlogPosts { get; set; }
    }
}