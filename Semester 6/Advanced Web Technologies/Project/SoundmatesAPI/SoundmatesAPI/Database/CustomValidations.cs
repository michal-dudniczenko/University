using System.ComponentModel.DataAnnotations;

public class BirthYearAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is int year)
        {
            int currentYear = DateTime.Now.Year;
            if (year >= 1900 && year <= currentYear)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult($"Birth year must be between 1900 and {currentYear}.");
        }
        return new ValidationResult("Invalid birth year.");
    }
}

public class PasswordAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string password)
        {
            if (password.Length < 8 || password.Length > 32)
            {
                return new ValidationResult("Password must be between 8 and 32 characters long.");
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]"))
            {
                return new ValidationResult("Password must contain at least one lowercase letter.");
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]"))
            {
                return new ValidationResult("Password must contain at least one uppercase letter.");
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"\d"))
            {
                return new ValidationResult("Password must contain at least one digit.");
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[\W_]"))
            {
                return new ValidationResult("Password must contain at least one special character.");
            }
            return ValidationResult.Success;
        }
        return new ValidationResult("Invalid password.");
    }
}
