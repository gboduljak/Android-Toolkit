using AndroidToolkitWeb.Models;
using AndroidToolkitWeb.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;



namespace AndroidToolkitWeb.Helpers
{
    public class SearchHelper
    {
        private readonly ReviewsDbContext _reviewsDb;
        private readonly BugReportsDbContext _reportsDb;
        private readonly BlogDbContext _blogDb;
        public SearchHelper()
        {
            _reviewsDb = new ReviewsDbContext();
            _reportsDb = new BugReportsDbContext();
            _blogDb = new BlogDbContext();
            _results = new List<SearchResult>();
        }
        private readonly List<SearchResult> _results;

        public IEnumerable<SearchResult> Search(bool reviews, bool reports, bool blog, string term)
        {
            _results.Clear();
            if (!string.IsNullOrEmpty(term))
            {
                if (reviews)
                {
                    foreach (var item in _reviewsDb.Reviews.Where(item => item.Name.ToLower().Contains(term.ToLower()) || item.Name.ToLower().Contains(term.ToLower()) || item.RatingDescription.ToLower().Contains(term.ToLower())))
                    {
                        _results.Add(new SearchResult() { DbId = item.ID, Name = item.Name, Category = SearchResultCategory.Review });
                    }
                }
                if (reports)
                {
                    foreach (var item in _reportsDb.BugReports.Where(item => item.Name.ToLower().Contains(term.ToLower()) || item.Name.ToLower().Contains(term.ToLower()) || item.Description.ToLower().Contains(term.ToLower())))
                    {
                        _results.Add(new SearchResult() { DbId = item.ID, Name = item.Name, Category = SearchResultCategory.BugReport });
                    }
                }
                if (blog)
                {
                    foreach (var item in _blogDb.BlogPosts.Where(item => item.Name.ToLower().Contains(term.ToLower()) || item.Name.ToLower().Contains(term.ToLower()) || item.Description.ToLower().Contains(term.ToLower()) || item.Post.Contains(term.ToLower())))
                    {
                        _results.Add(new SearchResult() { DbId = item.ID, Name = item.Name, Category = SearchResultCategory.BlogPost });
                    }
                }

            }
            return _results;
        }



    }
}