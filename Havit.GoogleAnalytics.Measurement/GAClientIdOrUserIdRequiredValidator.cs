using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics.Measurements
{
    internal static class GAClientIdOrUserIdRequiredValidator
    {
        public static bool Validate<TModel>(TModel model)
            where TModel : GAMeasurementModelBase
        {
            return !String.IsNullOrEmpty(model.ClientId) || !String.IsNullOrEmpty(model.UserId);
        }
    }
}
