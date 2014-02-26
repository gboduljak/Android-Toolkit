using System.ComponentModel.DataAnnotations;
public enum SearchResultCategory
{
    [Display(Name="REVIEW")]
    Review,
    [Display(Name="BUG REPORT")]
    BugReport,
    [Display(Name="BLOG POST")]
    BlogPost
}