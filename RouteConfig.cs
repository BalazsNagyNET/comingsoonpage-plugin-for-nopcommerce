using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.ComingSoonPage
{
    public class RouteConfig : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routes)
        {
            routes.MapLocalizedRoute("Plugin.Misc.ComingSoonPage.Display",
                "storeclosed/",
                new { controller = "ComingSoonPage", action = "Display" },
                new { },
                new[] { "Plugin.Misc.ComingSoonPage.Controllers" }
                );
        }

        public int Priority
        {
            get { return 100; }
        }
    }
}
