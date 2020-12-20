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
using Nop.Services.Logging;
using Nop.Web.Models.Customer;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework;
using Nop.Core.Domain.Security;
using Nop.Core.Events;
using System.Threading.Tasks;

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
        private readonly IStaticCacheManager _staticCacheManager;
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
            IStaticCacheManager staticCacheManager,
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
            this._staticCacheManager = staticCacheManager;
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

        protected async Task<string> GetBackgroundUrlAsync(int backgroundId)
        {
            var cacheKey = _staticCacheManager.PrepareKey(ModelCacheEventConsumer.BACKGROUND_URL_MODEL_KEY, backgroundId);
            return await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                //little hack here. nulls aren't cacheable so set it to ""
                var url = await _pictureService.GetPictureUrlAsync(backgroundId, showDefaultPicture: false) ?? "";
                return url;
            });
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var comingSoonPageSettings = await _settingService.LoadSettingAsync<ComingSoonPageSettings>(storeScope);
            var model = new ConfigurationModel();
            model.BackgroundId = comingSoonPageSettings.BackgroundId;
            model.OpeningDate = comingSoonPageSettings.OpeningDate;
            model.DisplayCountdown = comingSoonPageSettings.DisplayCountdown;
            model.DisplayNewsletterBox = comingSoonPageSettings.DisplayNewsletterBox;
            model.DisplayLoginButton = comingSoonPageSettings.DisplayLoginButton;
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.BackgroundId_OverrideForStore = await _settingService.SettingExistsAsync(comingSoonPageSettings, x => x.BackgroundId, storeScope);
                model.OpeningDate_OverrideForStore = await _settingService.SettingExistsAsync(comingSoonPageSettings, x => x.OpeningDate, storeScope);
                model.DisplayCountdown_OverrideForStore = await _settingService.SettingExistsAsync(comingSoonPageSettings, x => x.DisplayCountdown, storeScope);
                model.DisplayNewsletterBox_OverrideForStore = await _settingService.SettingExistsAsync(comingSoonPageSettings, x => x.DisplayNewsletterBox, storeScope);
                model.DisplayLoginButton_OverrideForStore = await _settingService.SettingExistsAsync(comingSoonPageSettings, x => x.DisplayLoginButton, storeScope);
            }

            return View("~/Plugins/Misc.ComingSoonPage/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var comingSoonPageSettings = await _settingService.LoadSettingAsync<ComingSoonPageSettings>(storeScope);
            comingSoonPageSettings.BackgroundId = model.BackgroundId;
            comingSoonPageSettings.OpeningDate = model.OpeningDate;
            comingSoonPageSettings.DisplayCountdown = model.DisplayCountdown;
            comingSoonPageSettings.DisplayNewsletterBox = model.DisplayNewsletterBox;
            comingSoonPageSettings.DisplayLoginButton = model.DisplayLoginButton;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(comingSoonPageSettings, x => x.BackgroundId, model.BackgroundId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(comingSoonPageSettings, x => x.OpeningDate, model.OpeningDate_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(comingSoonPageSettings, x => x.DisplayCountdown, model.DisplayCountdown_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(comingSoonPageSettings, x => x.DisplayNewsletterBox, model.DisplayNewsletterBox_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(comingSoonPageSettings, x => x.DisplayLoginButton, model.DisplayLoginButton_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        public async Task<IActionResult> Display()
        {
            if (TempData.ContainsKey("ModelState"))
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);

            var comingSoonPageSettings = await _settingService.LoadSettingAsync<ComingSoonPageSettings>((await _storeContext.GetCurrentStoreAsync()).Id);

            var model = new PublicInfoModel();
            model.BackgroundUrl = await GetBackgroundUrlAsync(comingSoonPageSettings.BackgroundId);
            model.OpeningDate = comingSoonPageSettings.OpeningDate.ToString("yyyy/MM/dd hh:mm:ss");
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
        public async Task<IActionResult> Login(LoginModel model, string returnUrl, bool captchaValid)
        {
            TempData["errors"] = await _localizationService.GetResourceAsync("Account.Login.Unsuccessful");
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            {
                AddErrorMessage(await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
                return RedirectToAction("Display");
            }

            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                var loginResult = await _customerRegistrationService.ValidateCustomerAsync(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled ? await _customerService.GetCustomerByUsernameAsync(model.Username) : await _customerService.GetCustomerByEmailAsync(model.Email);

                            //migrate shopping cart
                            await _shoppingCartService.MigrateShoppingCartAsync(await  _workContext.GetCurrentCustomerAsync(), customer, true);

                            //sign in new customer
                            await _authenticationService.SignInAsync(customer, model.RememberMe);

                            //raise event
                            await _eventPublisher.PublishAsync(new CustomerLoggedinEvent(customer));

                            //activity log
                            await _customerActivityService.InsertActivityAsync("PublicStore.Login", await _localizationService.GetResourceAsync("ActivityLog.PublicStore.Login"), customer);

                            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                return RedirectToRoute("HomePage");

                            return Redirect(returnUrl);
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        AddErrorMessage(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.CustomerNotExist"));
                        break;
                    case CustomerLoginResults.Deleted:
                        AddErrorMessage(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case CustomerLoginResults.NotActive:
                        AddErrorMessage(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case CustomerLoginResults.NotRegistered:
                        AddErrorMessage(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:
                        AddErrorMessage(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials"));
                        break;
                }
            }

            //If we got this far, something failed, redirect to Display with error mesages in TempData
            return RedirectToAction("Display");
        }

        private void AddErrorMessage(string message)
        {
            TempData["errors"] += $"<br />{message}";
        }
    }
}
