using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validation
{
    public class ValidCardPin : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            try
            {
                if (!(value.ToString().Length != 3))
                    return new ValidationResult("the card must have 16 digits");

                ulong.Parse(value.ToString());
                return ValidationResult.Success;

            }
            catch
            {
                return new ValidationResult("the card must only contain digits");
            }
        }
    }
}
