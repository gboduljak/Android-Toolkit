using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace AndroidToolkitWeb.Entities
{

    [Table("BlogPosts")]
    public class BlogPost
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }


        [Required]
        [Display(Name = "Post Description")]
        [AllowHtml]
        public string Description { get; set; }

        [Required]
        [AllowHtml]
        public string Post { get; set; }

        [Display(Name = "First Image")]
        public string Img1 { get; set; }

        [Display(Name = "Second Image")]
        public string Img2 { get; set; }

        [Display(Name = "Third Image")]
        public string Img3 { get; set; }

        [Required]
        public string Tags { get; set; }

        public bool IsEdited { get; set; }

        public string Author { get; set; }

        public DateTime Date { get; set; }
    }
}
