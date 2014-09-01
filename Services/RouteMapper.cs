using System;
using System.Web.Routing;
using DotNetNuke.Web.Api;

namespace Bitboxx.DNNModules.BBBooking.Services
{
    public class RouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("BBBooking",
                                         "default",
                                         "{controller}/{action}",
                                         new[] { "Bitboxx.DNNModules.BBBooking.Services" });
        }
    }
}