using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace AndroidToolkitWeb.Controllers
{
    public class UploadController : ApiController
    {
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage Upload(string location = null)
        {
            if (!string.IsNullOrEmpty(location))
            {
                HttpPostedFile file = HttpContext.Current.Request.Files[0];
                string savedFileName = Path.Combine(HttpContext.Current.Server.MapPath(location), Path.GetFileName(file.FileName));
                file.SaveAs(savedFileName);
                HttpContext.Current.Response.ContentType = "text/plain";
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var result = new { name = file.FileName };
                HttpContext.Current.Response.Write(serializer.Serialize(result));
                HttpContext.Current.Response.StatusCode = 200;
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            HttpPostedFile file2 = HttpContext.Current.Request.Files[0];
            string savedFileName2 = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Uploads"), Path.GetFileName(file2.FileName));
            file2.SaveAs(savedFileName2);
            HttpContext.Current.Response.ContentType = "text/plain";
            var serializer2 = new System.Web.Script.Serialization.JavaScriptSerializer();
            var result2 = new { name = file2.FileName };
            HttpContext.Current.Response.Write(serializer2.Serialize(result2));
            HttpContext.Current.Response.StatusCode = 200;
            return new HttpResponseMessage(HttpStatusCode.OK);

        }


    }
}

