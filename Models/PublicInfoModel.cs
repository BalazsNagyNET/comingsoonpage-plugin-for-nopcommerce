using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Plugin.Misc.ComingSoonPage.Models
{
    public class PublicInfoModel : BaseNopModel
    {
        public string BackgroundUrl { get; set; }
        public DateTime OpeningDate { get; set; }
        public bool DisplayCountdown { get; set; }
        public bool DisplayNewsletterBox { get; set; }
    }
}
