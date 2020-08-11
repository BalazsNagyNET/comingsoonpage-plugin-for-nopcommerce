using Nop.Core.Caching;
using Nop.Core.Domain.Configuration;
using Nop.Core.Events;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.ComingSoonPage.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer :
        IConsumer<EntityInsertedEvent<Setting>>,
        IConsumer<EntityUpdatedEvent<Setting>>,
        IConsumer<EntityDeletedEvent<Setting>>
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// </remarks>
        public static CacheKey BACKGROUND_URL_MODEL_KEY = new CacheKey("Nop.plugins.misc.comingsoonpage.pictureurl-{0}", BACKGROUND_URL_PATTERN_KEY);
        public const string BACKGROUND_URL_PATTERN_KEY = "Nop.plugins.misc.comingsoonpage";

        private readonly IStaticCacheManager _staticCacheManager;

        public ModelCacheEventConsumer(IStaticCacheManager staticCacheManager)
        {
            this._staticCacheManager = staticCacheManager;
        }

        public void HandleEvent(EntityInsertedEvent<Setting> eventMessage)
        {
            _staticCacheManager.RemoveByPrefix(BACKGROUND_URL_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Setting> eventMessage)
        {
            _staticCacheManager.RemoveByPrefix(BACKGROUND_URL_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Setting> eventMessage)
        {
            _staticCacheManager.RemoveByPrefix(BACKGROUND_URL_PATTERN_KEY);
        }
    }
}
