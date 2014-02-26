using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AndroidToolkitWeb
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
			config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
               name: "Api2",
               routeTemplate: "api/{controller}/{id}/{opt}",
               defaults: new { id = RouteParameter.Optional, opt = RouteParameter.Optional }
           );
        }
    }
}
