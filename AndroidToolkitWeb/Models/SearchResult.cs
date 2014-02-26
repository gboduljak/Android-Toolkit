using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AndroidToolkitWeb.Models
{
    public class SearchResult
    {
        public int DbId { get; set; }
        public string Name { get; set; }
        public SearchResultCategory Category { get; set; }
    }
}