using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Havit.GoogleAnalytics.Measurements.Transactions;

    internal class MeasurementTransactionItemValidator : GoogleAnalyticsMeasurementValidatorBase<MeasurementTransactionItem>
    {
        public void Validate(MeasurementTransactionItem model)
        {
            List<GoogleAnalyticsValidationResult> validationResults = ValidateInternal(model).ToList();

            if (validationResults.Any())
            {
                string message = "Error occured while validating values for Google Analytics transaction item request" + Environment.NewLine;
                message += String.Join(Environment.NewLine, validationResults.Select(x => x.ToString()));

                throw new ValidationException(message);
            }
        }
    }
