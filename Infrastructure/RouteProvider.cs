using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.ComingSoonPage
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("Plugin.Misc.ComingSoonPage.Display",
                "storeclosed/",
                new { controller = "ComingSoonPage", action = "Display" },
                new { },
                new[] { "Plugin.Misc.ComingSoonPage.Controllers" }
                );

            endpointRouteBuilder.MapControllerRoute("Plugin.Misc.ComingSoonPage.Login",
                "login/",
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
