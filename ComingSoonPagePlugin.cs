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
using System.Threading.Tasks;
using System.Collections.Generic;

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
            ISettingService settingService,
            IWebHelper webHelper)
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

        public override async Task InstallAsync()
        {
            //background
            var sampleBackgroundPath = _fileProvider.MapPath("~/Plugins/Misc.ComingSoonPage/Content/comingsoonpage/background.jpg");

            //settings
            var settings = new ComingSoonPageSettings
            {
                BackgroundId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(sampleBackgroundPath), MimeTypes.ImagePJpeg, "background")).Id,
                OpeningDate = DateTime.Now.AddDays(7),
                DisplayCountdown = true,
                DisplayNewsletterBox = true,
                DisplayLoginButton = true
            };
            await _settingService.SaveSettingAsync(settings);

            //locales
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Misc.ComingSoonPage.PageTitle"] = "Coming soon!",
                ["Plugins.Misc.ComingSoonPage.ComingSoon"] = "Our new webshop is coming soon!",
                ["Plugins.Misc.ComingSoonPage.ComingSoon.Hint"] = "Subscribe To Get Notified",
                ["Plugins.Misc.ComingSoonPage.Background"] = "Background",
                ["Plugins.Misc.ComingSoonPage.Background.Hint"] = "Fullscreen background image.",
                ["Plugins.Misc.ComingSoonPage.OpeningDate"] = "Opening date",
                ["Plugins.Misc.ComingSoonPage.OpeningDate.Hint"] = "Date and time when shop opens [countdown is displayed based on this setting).",
                ["Plugins.Misc.ComingSoonPage.DisplayCountdown"] = "Display countdown",
                ["Plugins.Misc.ComingSoonPage.DisplayCountdown.Hint"] = "Check to display countdown based on the opening date.",
                ["Plugins.Misc.ComingSoonPage.DisplayNewsletterBox"] = "Allow subscription",
                ["Plugins.Misc.ComingSoonPage.DisplayNewsletterBox.Hint"] = "Check to display input for visitors to subscribe.",
                ["Plugins.Misc.ComingSoonPage.DisplayLoginButton"] = "Display login button",
                ["Plugins.Misc.ComingSoonPage.DisplayLoginButton.Hint"] = "Check to display login button, so administrators can still log in.",
                ["Plugins.Misc.ComingSoonPage.Countdown.Day"] = "Day",
                ["Plugins.Misc.ComingSoonPage.Countdown.Days"] = "Days",
                ["Plugins.Misc.ComingSoonPage.Countdown.Week"] = "Week",
                ["Plugins.Misc.ComingSoonPage.Countdown.Weeks"] = "Weeks",
            });

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<ComingSoonPageSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.ComingSoonPage");

            await base.UninstallAsync();
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
