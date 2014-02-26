using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AndroidToolkitWeb.Models
{
    public class AndroidToolkitVersion
    {
        [Range(1,int.MaxValue)]
        [Required]
        public int Number { get; set; }
        [Required]
        [Url]
        public string Download { get; set; }
    }
}