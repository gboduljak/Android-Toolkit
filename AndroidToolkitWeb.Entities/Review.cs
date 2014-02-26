using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AndroidToolkitWeb.Entities
{
    [Table("Reviews")]
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }

        [AllowHtml]
        [Required]
        [Display(Name = "Rating Description")]
        public string RatingDescription { get; set; }

        [Required]
        [Range(1,5)]
        public int Rating { get; set; }

        public string Author { get; set; }
        public DateTime Date { get; set; }
    }
}
