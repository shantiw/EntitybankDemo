using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.DataAnnotations
{
    public static partial class ValidationHelper
    {
        public static ValidationException CreateValidationException(string errorMessage)
        {
            ValidationResult validationResult = new ValidationResult(errorMessage);
            return CreateValidationException(validationResult);
        }

        public static ValidationException CreateValidationException(string memberName, IEnumerable<string> errorMessages)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            foreach (string errorMessage in errorMessages)
            {
                ValidationResult validationResult = new ValidationResult(errorMessage, new List<string>() { memberName });
                validationResults.Add(validationResult);
            }
            return CreateValidationException(validationResults);
        }

        public static ValidationException CreateValidationException(ValidationResult validationResult)
        {
            return CreateValidationException(new List<ValidationResult>() { validationResult });
        }

        public static ValidationException CreateValidationException(IEnumerable<ValidationResult> validationResults)
        {
            List<ICollection<ValidationResult>> list = new List<ICollection<ValidationResult>>
            {
                validationResults.ToList()
            };

            return CreateValidationException(list.ToArray());
        }

        public static ValidationException CreateValidationException(ICollection<ValidationResult>[] validationResultCollections)
        {
            return ValidationExceptionHelper.CreateValidationException(validationResultCollections);
        }
    }
}
