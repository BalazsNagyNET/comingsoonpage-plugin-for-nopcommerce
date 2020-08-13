using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.ComingSoonPage
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(ComingSoonPageDefaults.Display, "storeclosed/",
                new { controller = "ComingSoonPage", action = "Display" });
        }

        public int Priority
        {
            get { return 100; }
        }
    }
}
