using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Havit.GoogleAnalytics.Measurements.Events
{
    internal class MeasurementEventValidator : GoogleAnalyticsValidatorBase<MeasurementEvent>
    {
        public void Validate(MeasurementEvent model)
        {
            List<GoogleAnalyticsValidationResult> validationResults = ValidateModel(model).ToList();

            if (!ClientIdOrUserIdRequiredValidator.Validate(model))
            {
                validationResults.Add(new GoogleAnalyticsValidationResult
                {
                    MemberName = "ClientId, UserId",
                    Message = "At least one value required."
                });
            }

            if (validationResults.Any())
            {
                string message = "Error occured while validating values for Google Analytics event request" + Environment.NewLine;
                message += String.Join(Environment.NewLine, validationResults.Select(x => x.ToString()));

                throw new ValidationException(message);
            }
        }
    }
}
