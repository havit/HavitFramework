using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Havit.GoogleAnalytics.Measurements.Events
{
    internal class GAMeasurementEventValidator : GAValidatorBase<GAMeasurementEvent>
    {
        public void Validate(GAMeasurementEvent model)
        {
            List<GAValidationResult> validationResults = ValidateModel(model).ToList();

            if (!GAClientIdOrUserIdRequiredValidator.Validate(model))
            {
                validationResults.Add(new GAValidationResult
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
