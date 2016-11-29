using Nop.Plugin.Misc.ComingSoonPage.Infrastructure;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Misc.ComingSoonPage
{
    public class RouteConfig : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapLocalizedRoute("Plugin.Misc.ComingSoonPage.Display",
                "storeclosed/",
                new { controller = "ComingSoonPage", action = "Display" },
                new[] { "Plugin.Misc.ComingSoonPage.Controllers" }
                );

            routes.MapLocalizedRoute("Plugin.Misc.ComingSoonPage.Configure",
                "ComingSoonPage/Configure",
                new { controller = "ComingSoonPage", action = "Configure" },
                new[] { "Plugin.Misc.ComingSoonPage.Controllers" }
                );
        }


        public int Priority
        {
            get { return 100; }
        }
    }
}
