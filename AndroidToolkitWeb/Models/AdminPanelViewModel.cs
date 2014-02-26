using AndroidToolkitWeb.DbContexts;
using AndroidToolkitWeb.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AndroidToolkitWeb.Models
{
    public class AdminPanelViewModel
    {
        public AdminPanelViewModel()
        {
            this.NewUserModel = new NewUserViewModel();
            this.EditUserModel = new EditUserViewModel();
            this.DeleteUserModel = new DeleteUserViewModel();
            this.NewAndroidToolkitVersion = new NewAndroidToolkitVersionViewModel();
            this.AndroidToolkitVersions = new List<AndroidToolkitVersion>();
        }
        #region Users

        public class NewUserViewModel : RegisterViewModel
        {

        }

        public NewUserViewModel NewUserModel { get; set; }

        public class EditUserViewModel
        {
            [Required]
            [Display(Name = "New UserName")]
            public string NewUserName { get; set; }

            [Required]
            [Display(Name = "Old UserName")]
            public string OldUserName { get; set; }

            [Required]
            [Display(Name = "User name")]
            public string UserName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

        }

        public EditUserViewModel EditUserModel { get; set; }

        public class DeleteUserViewModel
        {
            [Required]
            [Display(Name = "User name")]
            public string DeleteUserName { get; set; }
        }

        public DeleteUserViewModel DeleteUserModel { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string GiveAdminUsername { get; set; }
        [Required]
        [Display(Name = "User name")]
        public string DeleteAdminUsername { get; set; }
        #endregion

        #region AppModels
        public IEnumerable<BugReport> BugReports { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        public IEnumerable<BlogPost> BlogPosts { get; set; }
        [Required]
        public int BugReportID { get; set; }
        public string BugReportSolvedStatus { get; set; }

        public BlogPost NewBlogPost { get; set; }
        #endregion

        public int RegisteredUsers { get; set; }

        #region DbContexts
        public BlogDbContext BlogDB { get; set; }
        public BugReportsDbContext BugReportsDB { get; set; }
        #endregion

        public List<AndroidToolkitVersion> AndroidToolkitVersions { get; set; }

        public string AndroidToolkitVersionsXML { get; set; }
        public class NewAndroidToolkitVersionViewModel : AndroidToolkitVersion
        {

        }
        public NewAndroidToolkitVersionViewModel NewAndroidToolkitVersion { get; set; }
    }
}