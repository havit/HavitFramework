using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics.Measurements.ValueValidators
{
    internal static class ClientIdOrUserIdRequiredValidator
    {
        public static bool Validate<TModel>(TModel model)
            where TModel : MeasurementModelBase
        {
            return !String.IsNullOrEmpty(model.ClientId) || !String.IsNullOrEmpty(model.UserId);
        }
    }
}
