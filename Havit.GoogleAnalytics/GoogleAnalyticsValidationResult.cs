using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics
{
    internal class GoogleAnalyticsValidationResult
    {
        public string MemberName { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{MemberName}: {Message}";
        }
    }
}
