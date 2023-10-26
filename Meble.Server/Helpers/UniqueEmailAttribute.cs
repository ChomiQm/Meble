using Meble.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace Meble.Server.Helpers
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Validation error - Can't find value.");
            }

            if (validationContext == null)
            {
                return new ValidationResult("Validation error - ValidationContext is null.");
            }

            var dbContext = (ModelContext?)validationContext.GetService(typeof(ModelContext));

            if (dbContext == null)
            {
                return new ValidationResult("Validation error - cannot find access to database.");
            }

            var email = (string)value;

            var existingUser = dbContext.Users.SingleOrDefault(u => u.Email == email);

            if (existingUser != null)
            {
                return new ValidationResult("Validation error - email exists");
            }

            return new ValidationResult(string.Empty);
        }
    }
}
