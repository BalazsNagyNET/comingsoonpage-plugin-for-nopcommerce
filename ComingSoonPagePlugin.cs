﻿using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Plugins;
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
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public ComingSoonPagePlugin(IPictureService pictureService,
            ISettingService settingService, IWebHelper webHelper)
        {
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
            var sampleBackgroundPath = CommonHelper.MapPath("~/Plugins/Misc.ComingSoonPage/Content/comingsoonpage/background.jpg");

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
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.PageTitle", "Coming soon!");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.ComingSoon", "Our new webshop is coming soon!");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.ComingSoon.Hint", "Subscribe To Get Notified");

            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.Background", "Background");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.Background.Hint", "Fullscreen background image.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.OpeningDate", "Opening date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.OpeningDate.Hint", "Date and time when shop opens (countdown is displayed based on this setting).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayCountdown", "Display countdown");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayCountdown.Hint", "Check to display countdown based on the opening date.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayNewsletterBox", "Allow subscription");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayNewsletterBox.Hint", "Check to display input for visitors to subscribe.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayLoginButton", "Display login button");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayLoginButton.Hint", "Check to display login button, so administrators can still log in.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Day", "Day");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Days", "Days");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Week", "Week");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Weeks", "Weeks");


            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<ComingSoonPageSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.PageTitle");
            this.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.ComingSoon");
            this.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.ComingSoon.Hint");
            this.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.Background");
            this.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.Background.Hint");
            this.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.OpeningDate");
            this.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.OpeningDate.Hint");
            this.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayCountdown");
            this.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayCountdown.Hint");
            this.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayNewsletterBox");
            this.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayNewsletterBox.Hint");
            this.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayLoginButton");
            this.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.DisplayLoginButton.Hint");
            this.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Day");
            this.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Days");
            this.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Week");
            this.DeletePluginLocaleResource("Plugins.Misc.ComingSoonPage.Countdown.Weeks");
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
