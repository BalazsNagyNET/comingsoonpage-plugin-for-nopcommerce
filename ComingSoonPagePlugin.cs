using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using System;
using System.IO;
using System.Web.Routing;

namespace Nop.Plugin.Misc.ComingSoonPage
{
    public class ComingSoonPagePlugin : BasePlugin, IMiscPlugin
    {
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;

        public ComingSoonPagePlugin(IPictureService pictureService,
            ISettingService settingService)
        {
            this._pictureService = pictureService;
            this._settingService = settingService;
        }

        public bool Authenticate()
        {
            return true;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
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
                DisplayCountdown = true,
                DisplayNewsletterBox = true,
                OpeningDate = new DateTime(2016, 12, 12)
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
