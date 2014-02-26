using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AndroidToolkitWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            #region Custom
            routes.MapRoute(
                name: "Reviews",
                url: "Feedback/Reviews/{action}/{id}/{name}",
                defaults: new { controller = "Feedback", action = "Reviews", id = UrlParameter.Optional, name = UrlParameter.Optional }
                );

            routes.MapRoute(
               name: "Reports",
               url: "Feedback/BugReports/{action}/{id}/{name}",
               defaults: new { controller = "Feedback", action = "Reports", id = UrlParameter.Optional, name = UrlParameter.Optional }
                );

            routes.MapRoute(
             name: "Blog",
             url: "Blog/Post/{action}/{id}/{name}",
             defaults: new { controller = "Blog", action="Details", id = UrlParameter.Optional, name = UrlParameter.Optional }
             );

            routes.MapRoute(
            name: "Blog2",
            url: "Blog/Posts/{action}",
            defaults: new {controller="Blog", id = UrlParameter.Optional, name = UrlParameter.Optional }
            );
            #endregion
            routes.MapRoute(
             name: "Default",
             url: "{controller}/{action}/{id}",
             defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
             );



        }
    }
}
