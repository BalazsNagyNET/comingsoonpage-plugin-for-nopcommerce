using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Misc.ComingSoonPage.Infrastructure.Cache;
using Nop.Plugin.Misc.ComingSoonPage.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Services.Messages;
using System;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Authentication;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Web.Models.Customer;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework;
using System.Collections.Generic;
using Nop.Core.Domain.Security;
using System.Globalization;

namespace Nop.Plugin.Misc.ComingSoonPage.Controllers
{
    public class ComingSoonPageController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly INotificationService _notificationService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly ICacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;

        //needed for subscription action (will be not necessary from nopCommerce 3.90)
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IWorkflowMessageService _workflowMessageService;

        //needed for login action
        private readonly CustomerSettings _customerSettings;
        private readonly CaptchaSettings _captchaSettings;
        private ICustomerRegistrationService _customerRegistrationService;
        private ICustomerService _customerService;
        private IShoppingCartService _shoppingCartService;
        private IAuthenticationService _authenticationService;
        private IEventPublisher _eventPublisher;
        private ICustomerActivityService _customerActivityService;

        public ComingSoonPageController(IWorkContext workContext,
            INotificationService notificationService,
            IStoreContext storeContext,
            IStoreService storeService,
            IPictureService pictureService,
            ISettingService settingService,
            ICacheManager cacheManager,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IWorkflowMessageService workflowMessageService,

            CustomerSettings customerSettings,
            CaptchaSettings captchaSettings,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            IShoppingCartService shoppingCartService,
            IAuthenticationService authenticationService,
            IEventPublisher eventPublisher,
            ICustomerActivityService customerActivityService)
        {
            this._workContext = workContext;
            this._notificationService = notificationService;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._cacheManager = cacheManager;
            this._localizationService = localizationService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._workflowMessageService = workflowMessageService;

            this._customerSettings = customerSettings;
            this._captchaSettings = captchaSettings;

            this._customerRegistrationService = customerRegistrationService;
            this._customerService = customerService;
            this._shoppingCartService = shoppingCartService;
            this._authenticationService = authenticationService;
            this._eventPublisher = eventPublisher;
            this._customerActivityService = customerActivityService;
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

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var comingSoonPageSettings = _settingService.LoadSetting<ComingSoonPageSettings>(storeScope);
            var model = new ConfigurationModel();
            model.BackgroundId = comingSoonPageSettings.BackgroundId;
            model.OpeningDate = comingSoonPageSettings.OpeningDate;
            model.DisplayCountdown = comingSoonPageSettings.DisplayCountdown;
            model.DisplayNewsletterBox = comingSoonPageSettings.DisplayNewsletterBox;
            model.DisplayLoginButton = comingSoonPageSettings.DisplayLoginButton;
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.BackgroundId_OverrideForStore = _settingService.SettingExists(comingSoonPageSettings, x => x.BackgroundId, storeScope);
                model.OpeningDate_OverrideForStore = _settingService.SettingExists(comingSoonPageSettings, x => x.OpeningDate, storeScope);
                model.DisplayCountdown_OverrideForStore = _settingService.SettingExists(comingSoonPageSettings, x => x.DisplayCountdown, storeScope);
                model.DisplayNewsletterBox_OverrideForStore = _settingService.SettingExists(comingSoonPageSettings, x => x.DisplayNewsletterBox, storeScope);
                model.DisplayLoginButton_OverrideForStore = _settingService.SettingExists(comingSoonPageSettings, x => x.DisplayLoginButton, storeScope);
            }

            return View("~/Plugins/Misc.ComingSoonPage/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var comingSoonPageSettings = _settingService.LoadSetting<ComingSoonPageSettings>(storeScope);
            comingSoonPageSettings.BackgroundId = model.BackgroundId;
            comingSoonPageSettings.OpeningDate = model.OpeningDate;
            comingSoonPageSettings.DisplayCountdown = model.DisplayCountdown;
            comingSoonPageSettings.DisplayNewsletterBox = model.DisplayNewsletterBox;
            comingSoonPageSettings.DisplayLoginButton = model.DisplayLoginButton;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(comingSoonPageSettings, x => x.BackgroundId, model.BackgroundId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(comingSoonPageSettings, x => x.OpeningDate, model.OpeningDate_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(comingSoonPageSettings, x => x.DisplayCountdown, model.DisplayCountdown_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(comingSoonPageSettings, x => x.DisplayNewsletterBox, model.DisplayNewsletterBox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(comingSoonPageSettings, x => x.DisplayLoginButton, model.DisplayLoginButton_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }

        public IActionResult Display()
        {
            if (TempData.ContainsKey("ModelState"))
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);

            var comingSoonPageSettings = _settingService.LoadSetting<ComingSoonPageSettings>(_storeContext.CurrentStore.Id);

            var model = new PublicInfoModel();
            model.BackgroundUrl = GetBackgroundUrl(comingSoonPageSettings.BackgroundId);
            model.OpeningDate = Convert.ToDateTime(comingSoonPageSettings.OpeningDate).ToString("G", new CultureInfo("en-US"));
            model.DisplayCountdown = comingSoonPageSettings.DisplayCountdown;
            model.DisplayNewsletterBox = comingSoonPageSettings.DisplayNewsletterBox;
            model.DisplayLoginButton = comingSoonPageSettings.DisplayLoginButton;
            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage;

            return View("~/Plugins/Misc.ComingSoonPage/Views/Display.cshtml", model);
        }

        [HttpPost]
        [ValidateCaptcha]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public IActionResult Login(LoginModel model, string returnUrl, bool captchaValid)
        {
            var errorMessages = new List<string>() {_localizationService.GetResource("Account.Login.Unsuccessful")};
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            {
                errorMessages.Add(_localizationService.GetResource("Common.WrongCaptchaMessage"));
                TempData["errors"] = string.Join("<br />", errorMessages);
                return RedirectToAction("Display");
            }

            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled ? _customerService.GetCustomerByUsername(model.Username) : _customerService.GetCustomerByEmail(model.Email);

                            //migrate shopping cart
                            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

                            //sign in new customer
                            _authenticationService.SignIn(customer, model.RememberMe);

                            //raise event
                            _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

                            //activity log
                            _customerActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);

                            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                return RedirectToRoute("HomePage");

                            return Redirect(returnUrl);
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        errorMessages.Add(_localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
                        break;
                    case CustomerLoginResults.Deleted:
                        errorMessages.Add(_localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case CustomerLoginResults.NotActive:
                        errorMessages.Add(_localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case CustomerLoginResults.NotRegistered:
                        errorMessages.Add(_localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:
                        errorMessages.Add(_localizationService.GetResource("Account.Login.WrongCredentials"));
                        break;
                }
            }

            //If we got this far, something failed, redirect to Display with error mesages in TempData
            TempData["errors"] = string.Join("<br />", errorMessages);
            return RedirectToAction("Display");
        }
    }
}
