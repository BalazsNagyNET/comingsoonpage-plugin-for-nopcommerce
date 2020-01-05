using Nop.Core.Caching;
using Nop.Core.Domain.Configuration;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.ComingSoonPage.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer :
        IConsumer<EntityInserted<Setting>>,
        IConsumer<EntityUpdated<Setting>>,
        IConsumer<EntityDeleted<Setting>>
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// </remarks>
        public const string BACKGROUND_URL_MODEL_KEY = "Nop.plugins.misc.comingsoonpage.pictureurl-{0}";
        public const string BACKGROUND_URL_PATTERN_KEY = "Nop.plugins.misc.comingsoonpage";

        private readonly ICacheManager _cacheManager;

        public ModelCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
        }

        public void HandleEvent(EntityInserted<Setting> eventMessage)
        {
            _cacheManager.RemoveByPattern(BACKGROUND_URL_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Setting> eventMessage)
        {
            _cacheManager.RemoveByPattern(BACKGROUND_URL_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Setting> eventMessage)
        {
            _cacheManager.RemoveByPattern(BACKGROUND_URL_PATTERN_KEY);
        }
    }
}
