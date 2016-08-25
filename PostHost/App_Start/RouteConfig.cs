using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PostHost
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{C_id}",
                defaults: new { controller = "Home", action = "Index", C_id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "AddComment",
                url: "{controller}/{action}/{comment}/{id}",
                defaults: new { controller = "Home", action = "CommentCreator", comment = "", id = "" }
            );
            routes.MapRoute(
                name: "LkeDislike",
                url: "{controller}/{action}/{value}/{toMod}",
                defaults: new { controller = "Home", action = "likeModifier", comment = "", id = "" }
            );
        }
    }
}
