using DoAnChuyenNganh.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DoAnChuyenNganh
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                 name: "ThanhCong",
                 url: "GioHangs/ThanhCong/{id}",
                 defaults: new { controller = "GioHangs", action = "ThanhCong", id = UrlParameter.Optional },
                 namespaces: new[] { "DoAnChuyenNganh.Controllers" }
            );

            routes.MapRoute(
             name: "GioHangs",
             url: "gio-hang",
             defaults: new { controller = "GioHangs", action = "Index"},
             namespaces: new[] { "DoAnChuyenNganh.Controllers" }
            );

            routes.MapRoute(
             name: "DetailSanPham",
             url: "chi-tiet/{slug}-p{id}",
             defaults: new { controller = "Products", action = "Detail"/*, slug = UrlParameter.Optional*/ },
             namespaces: new[] { "DoAnChuyenNganh.Controllers" }
            );

            routes.MapRoute(
               name: "DanhMucSanPham",
               url: "san-pham/{slug}",
               defaults: new { controller = "Products", action = "Index", slug = UrlParameter.Optional },
               namespaces: new[] { "DoAnChuyenNganh.Controllers" }
           );

            


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "DoAnChuyenNganh.Controllers" }
            );
        }

    }


}
