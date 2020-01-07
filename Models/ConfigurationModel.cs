using System.ComponentModel.DataAnnotations;
using System;
using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

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
        public DateTime OpeningDate { get; set; }
        public bool OpeningDate_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ComingSoonPage.DisplayCountdown")]
        public bool DisplayCountdown { get; set; }
        public bool DisplayCountdown_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ComingSoonPage.DisplayNewsletterBox")]
        public bool DisplayNewsletterBox { get; set; }
        public bool DisplayNewsletterBox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ComingSoonPage.DisplayLoginButton")]
        public bool DisplayLoginButton { get; set; }
        public bool DisplayLoginButton_OverrideForStore { get; set; }
    }
}
