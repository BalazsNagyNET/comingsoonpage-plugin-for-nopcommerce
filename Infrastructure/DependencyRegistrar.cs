using Autofac;
using Autofac.Core;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Configuration;
using Nop.Plugin.Misc.ComingSoonPage.Controllers;
using Nop.Core.Caching;

namespace Nop.Plugin.Misc.ComingSoonPage.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //we cache presentation models between requests
            builder.RegisterType<ComingSoonPageController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"));
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
