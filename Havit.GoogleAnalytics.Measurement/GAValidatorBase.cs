using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics
{
    internal abstract class GAValidatorBase<TModel>
    {
        protected IEnumerable<GAValidationResult> ValidateModel(TModel model)
        {
            foreach (var property in model.GetType().GetProperties())
            {
                if (!GAValueRequiredValidator.Validate(property.GetValue(model), property)) 
                {
                    yield return new GAValidationResult
                    {
                        MemberName = property.Name,
                        Message = "Value is required"
                    };
                }
            }
        }
    }
}
