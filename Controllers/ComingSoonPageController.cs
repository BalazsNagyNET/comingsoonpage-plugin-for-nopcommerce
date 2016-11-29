using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Misc.ComingSoonPage.Infrastructure.Cache;
using Nop.Plugin.Misc.ComingSoonPage.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Core.Domain.Customers;
using Nop.Services.Messages;
using Nop.Core.Domain.Messages;
using System;
using Nop.Web.Framework;

namespace Nop.Plugin.Misc.ComingSoonPage.Controllers
{
    public class ComingSoonPageController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly ICacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IWorkflowMessageService _workflowMessageService;

        public ComingSoonPageController(IWorkContext workContext,
            IStoreContext storeContext,
            IStoreService storeService,
            IPictureService pictureService,
            ISettingService settingService,
            ICacheManager cacheManager,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IWorkflowMessageService workflowMessageService)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._cacheManager = cacheManager;
            this._localizationService = localizationService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._workflowMessageService = workflowMessageService;
        }

        protected string GetBackgroundUrl(int backgroundId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.BACKGROUND_URL_MODEL_KEY, backgroundId);
            return _cacheManager.Get(cacheKey, () =>
            {
                var url = _pictureService.GetPictureUrl(backgroundId, showDefaultPicture: false);
                //little hack here. nulls aren't cacheable so set it to ""
                if (url == null)
                    url = "";

                return url;
            });
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var comingSoonPageSettings = _settingService.LoadSetting<ComingSoonPageSettings>(storeScope);
            var model = new ConfigurationModel();
            model.BackgroundId = comingSoonPageSettings.BackgroundId;
            model.DisplayCountdown = comingSoonPageSettings.DisplayCountdown;
            model.DisplayNewsletterBox = comingSoonPageSettings.DisplayNewsletterBox;
            model.OpeningDate = comingSoonPageSettings.OpeningDate;
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.BackgroundId_OverrideForStore = _settingService.SettingExists(comingSoonPageSettings, x => x.BackgroundId, storeScope);
                model.DisplayCountdown_OverrideForStore = _settingService.SettingExists(comingSoonPageSettings, x => x.DisplayCountdown, storeScope);
                model.DisplayNewsletterBox_OverrideForStore = _settingService.SettingExists(comingSoonPageSettings, x => x.DisplayNewsletterBox, storeScope);
                model.OpeningDate_OverrideForStore = _settingService.SettingExists(comingSoonPageSettings, x => x.OpeningDate, storeScope);
            }

            return View("~/Plugins/Misc.ComingSoonPage/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var comingSoonPageSettings = _settingService.LoadSetting<ComingSoonPageSettings>(storeScope);
            comingSoonPageSettings.BackgroundId = model.BackgroundId;
            comingSoonPageSettings.DisplayCountdown = model.DisplayCountdown;
            comingSoonPageSettings.DisplayNewsletterBox = model.DisplayNewsletterBox;
            comingSoonPageSettings.OpeningDate = model.OpeningDate;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(comingSoonPageSettings, x => x.BackgroundId, model.BackgroundId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(comingSoonPageSettings, x => x.DisplayCountdown, model.DisplayCountdown_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(comingSoonPageSettings, x => x.DisplayNewsletterBox, model.DisplayNewsletterBox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(comingSoonPageSettings, x => x.OpeningDate, model.OpeningDate_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }

        public ActionResult Display()
        {
            var comingSoonPageSettings = _settingService.LoadSetting<ComingSoonPageSettings>(_storeContext.CurrentStore.Id);

            var model = new PublicInfoModel();
            model.BackgroundUrl = GetBackgroundUrl(comingSoonPageSettings.BackgroundId);
            model.DisplayCountdown = comingSoonPageSettings.DisplayCountdown;
            model.DisplayNewsletterBox = comingSoonPageSettings.DisplayNewsletterBox;
            model.OpeningDate = comingSoonPageSettings.OpeningDate;

            return View("~/Plugins/Misc.ComingSoonPage/Views/Display.cshtml", model);
        }

        [HttpPost]
        [StoreClosed(true)]
        public ActionResult Subscribe(string email) {
            string result;
            bool success = false;

            if (!CommonHelper.IsValidEmail(email))
            {
                result = _localizationService.GetResource("Newsletter.Email.Wrong");
            }
            else
            {
                email = email.Trim();

                var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(email, _storeContext.CurrentStore.Id);
                if (subscription != null)
                {
                        if (!subscription.Active)
                        {
                            _workflowMessageService.SendNewsLetterSubscriptionActivationMessage(subscription, _workContext.WorkingLanguage.Id);
                        }
                        result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                }
                else
                {
                    subscription = new NewsLetterSubscription
                    {
                        NewsLetterSubscriptionGuid = Guid.NewGuid(),
                        Email = email,
                        Active = false,
                        StoreId = _storeContext.CurrentStore.Id,
                        CreatedOnUtc = DateTime.UtcNow
                    };
                    _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
                    _workflowMessageService.SendNewsLetterSubscriptionActivationMessage(subscription, _workContext.WorkingLanguage.Id);

                    result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                }
                success = true;
            }

            return Json(new
            {
                Success = success,
                Result = result,
            });
        }
    }
}
