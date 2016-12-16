using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Plugin.Misc.ComingSoonPage.Models
{
    public class PublicInfoModel : BaseNopModel
    {
        public string BackgroundUrl { get; set; }
        public string OpeningDate { get; set; }
        public bool DisplayCountdown { get; set; }
        public bool DisplayNewsletterBox { get; set; }
        public bool DisplayLoginButton { get; set; }
        public bool UsernamesEnabled { get; set; }
        public bool DisplayCaptcha { get; set; }
        
    }
}
