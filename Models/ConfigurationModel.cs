using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Plugin.Misc.ComingSoonPage.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }


        [NopResourceDisplayName("Plugins.Misc.ComingSoonPage.Background")]
        [UIHint("Picture")]
        public int BackgroundId { get; set; }
        public bool BackgroundId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ComingSoonPage.OpeningDate")]
        [AllowHtml]
        public DateTime OpeningDate { get; set; }
        public bool OpeningDate_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ComingSoonPage.DisplayCountdown")]
        [AllowHtml]
        public bool DisplayCountdown { get; set; }
        public bool DisplayCountdown_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ComingSoonPage.DisplayNewsletterBox")]
        [AllowHtml]
        public bool DisplayNewsletterBox { get; set; }
        public bool DisplayNewsletterBox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ComingSoonPage.DisplayLoginButton")]
        [AllowHtml]
        public bool DisplayLoginButton { get; set; }
        public bool DisplayLoginButton_OverrideForStore { get; set; }
    }
}
