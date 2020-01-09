using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using System;
using System.IO;

namespace Nop.Plugin.Misc.ComingSoonPage
{
    public class ComingSoonPagePlugin : BasePlugin, IMiscPlugin
    {
        private readonly ILocalizationService _localizationService;
        private readonly INopFileProvider _fileProvider;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public ComingSoonPagePlugin(ILocalizationService localizationService,
            INopFileProvider fileProvider,
            IPictureService pictureService,
            ISettingService settingService, IWebHelper webHelper)
        {
            this._localizationService = localizationService;
            this._fileProvider = fileProvider;
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._webHelper = webHelper;
        }


        public bool Authenticate()
        {
            return true;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/ComingSoonPage/Configure";
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ComingSoonPage";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Misc.ComingSoonPage.Controllers" }, { "area", null } };
        }

        public override void Install()
        {
            //background
            var sampleBackgroundPath = _fileProvider.MapPath("~/Plugins/Misc.ComingSoonPage/Content/comingsoonpage/background.jpg");

            //settings
            var settings = new ComingSoonPageSettings
            {
                BackgroundId = _pictureService.InsertPicture(File.ReadAllBytes(sampleBackgroundPath), MimeTypes.ImagePJpeg, "background").Id,
                OpeningDate = DateTime.Now.AddDays(7),
                DisplayCountdown = true,
                DisplayNewsletterBox = true,
                DisplayLoginButton = true
            };
            _settingService.SaveSetting(settings);

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.PageTitle", "Coming soon!");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.ComingSoon", "Our new webshop is coming soon!");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.ComingSoon.Hint", "Subscribe To Get Notified");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.Background", "Background");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.Background.Hint", "Fullscreen background image.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.OpeningDate", "Opening date");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.OpeningDate.Hint", "Date and time when shop opens (countdown is displayed based on this setting).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayCountdown", "Display countdown");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayCountdown.Hint", "Check to display countdown based on the opening date.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayNewsletterBox", "Allow subscription");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayNewsletterBox.Hint", "Check to display input for visitors to subscribe.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayLoginButton", "Display login button");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayLoginButton.Hint", "Check to display login button, so administrators can still log in.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Day", "Day");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Days", "Days");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Week", "Week");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Weeks", "Weeks");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Year", "Year");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Years", "Years");


            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<ComingSoonPageSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.PageTitle");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.ComingSoon");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.ComingSoon.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.Background");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.Background.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.OpeningDate");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.OpeningDate.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayCountdown");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayCountdown.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayNewsletterBox");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayNewsletterBox.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayLoginButton");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayLoginButton.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Day");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Days");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Week");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Weeks");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Year");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Years");
            base.Uninstall();
        }

        //public void ManageSiteMap(SiteMapNode rootNode)
        //{
        //    var menuItem = new SiteMapNode()
        //    {
        //        SystemName = "ComingSoonPage",
        //        Title = "Coming soon page",
        //        ControllerName = "ComingSoonPage",
        //        ActionName = "Configure",
        //        Visible = true,
        //        RouteValues = new RouteValueDictionary() { { "area", null } },
        //    };
        //    var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
        //    if (pluginNode != null)
        //        pluginNode.ChildNodes.Add(menuItem);
        //    else
        //        rootNode.ChildNodes.Add(menuItem);
        //}


    }
}
