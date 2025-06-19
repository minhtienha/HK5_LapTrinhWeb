using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Http;
using System.Xml.Linq;

namespace ThietBiDienTu
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
