using Nop.Core.Configuration;
using System;

namespace Nop.Plugin.Misc.ComingSoonPage
{
    public class ComingSoonPageSettings : ISettings
    {
        public int BackgroundId { get; set; }
        public DateTime OpeningDate { get; set; }
        public bool DisplayCountdown { get; set; }
        public bool DisplayNewsletterBox { get; set; }
    }
}