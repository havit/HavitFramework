using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics.Measurements
{
    internal class GoogleAnalyticsMeasurementValidatorBase<TModel> : GoogleAnalyticsValidatorBase<TModel>
        where TModel : MeasurementModelBase
    {
        protected override IEnumerable<GoogleAnalyticsValidationResult> ValidateInternal(TModel model)
        {
            List<GoogleAnalyticsValidationResult> validationResults = new List<GoogleAnalyticsValidationResult>();
            validationResults.AddRange(base.ValidateInternal(model));

            if (!ClientIdOrUserIdRequiredValidator.Validate(model))
            {
                validationResults.Add(new GoogleAnalyticsValidationResult
                {
                    MemberName = "ClientId, UserId",
                    Message = "At least one value required."
                });
            }

            return validationResults;
        }
    }
}
