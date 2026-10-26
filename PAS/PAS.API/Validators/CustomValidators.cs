using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PAS.API.Validators;

public static class CustomValidators
{

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }
        catch
        {
            return false;
        }
    }

    public static System.ComponentModel.DataAnnotations.ValidationResult? IsValidEmail(object? value, ValidationContext context)
    {
        var input = value as string;
        return IsValidEmail(input ?? string.Empty)
            ? System.ComponentModel.DataAnnotations.ValidationResult.Success
            : new System.ComponentModel.DataAnnotations.ValidationResult("Invalid email format.");
    }

  
    public static bool IsValidPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        var regex = new Regex(@"^[\+]?[(]?[0-9]{1,3}[)]?[-\s\.]?[(]?[0-9]{1,4}[)]?[-\s\.]?[0-9]{3,4}[-\s\.]?[0-9]{3,4}$");
        return regex.IsMatch(phone);
    }

    public static System.ComponentModel.DataAnnotations.ValidationResult? IsValidPhone(object? value, ValidationContext context)
    {
        var input = value as string;
        if (string.IsNullOrWhiteSpace(input)) return System.ComponentModel.DataAnnotations.ValidationResult.Success;
        return IsValidPhone(input)
            ? System.ComponentModel.DataAnnotations.ValidationResult.Success
            : new System.ComponentModel.DataAnnotations.ValidationResult("Invalid phone format.");
    }

    public static bool IsValidTinNumber(string tin)
    {
        if (string.IsNullOrWhiteSpace(tin))
            return false;

        var regex = new Regex(@"^\d{9,15}$");
        return regex.IsMatch(tin);
    }

    public static bool IsValidSku(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return false;

        var regex = new Regex(@"^[A-Z0-9\-_]{3,50}$");
        return regex.IsMatch(sku);
    }

   
    public static bool IsValidTagNumber(string tagNumber)
    {
        if (string.IsNullOrWhiteSpace(tagNumber))
            return false;

        var regex = new Regex(@"^[A-Z0-9\-_]{3,50}$");
        return regex.IsMatch(tagNumber);
    }

    
    public static bool IsValidGrnNumber(string grnNumber)
    {
        if (string.IsNullOrWhiteSpace(grnNumber))
            return false;

        var regex = new Regex(@"^GRN-\d{4}-\d{6}$");
        return regex.IsMatch(grnNumber);
    }

    
    public static bool IsValidSrNumber(string srNumber)
    {
        if (string.IsNullOrWhiteSpace(srNumber))
            return false;

        var regex = new Regex(@"^SR-\d{4}-\d{6}$");
        return regex.IsMatch(srNumber);
    }

    public static bool IsValidSivNumber(string sivNumber)
    {
        if (string.IsNullOrWhiteSpace(sivNumber))
            return false;

        var regex = new Regex(@"^SIV-\d{4}-\d{6}$");
        return regex.IsMatch(sivNumber);
    }

    public static bool IsNotFutureDate(DateTime date)
    {
        return date <= DateTime.Now;
    }

    
    public static bool IsNotPastDate(DateTime date)
    {
        return date >= DateTime.Now;
    }

    public static bool IsValidDateRange(DateTime from, DateTime to)
    {
        return from <= to;
    }

   
    public static bool HasMinLength(string value, int minLength)
    {
        return !string.IsNullOrWhiteSpace(value) && value.Length >= minLength;
    }

    public static bool HasMaxLength(string value, int maxLength)
    {
        return value == null || value.Length <= maxLength;
    }

   
    public static bool IsWithinRange(decimal value, decimal min, decimal max)
    {
        return value >= min && value <= max;
    }

    
    public static bool IsValidQuantity(int quantity)
    {
        return quantity > 0;
    }

    
    public static bool IsValidPrice(decimal price)
    {
        return price >= 0;
    }

   
    public static bool IsValidPercentage(decimal percentage)
    {
        return percentage >= 0 && percentage <= 100;
    }

    public static bool IsAlphaOnly(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var regex = new Regex(@"^[a-zA-Z\s]+$");
        return regex.IsMatch(value);
    }

    public static System.ComponentModel.DataAnnotations.ValidationResult? IsAlphaOnly(object? value, ValidationContext context)
    {
        var input = value as string;
        return IsAlphaOnly(input ?? string.Empty)
            ? System.ComponentModel.DataAnnotations.ValidationResult.Success
            : new System.ComponentModel.DataAnnotations.ValidationResult("Only alphabetic characters are allowed.");
    }

    public static bool IsAlphanumeric(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var regex = new Regex(@"^[a-zA-Z0-9]+$");
        return regex.IsMatch(value);
    }

    public static System.ComponentModel.DataAnnotations.ValidationResult? IsAlphanumeric(object? value, ValidationContext context)
    {
        var input = value as string;
        return IsAlphanumeric(input ?? string.Empty)
            ? System.ComponentModel.DataAnnotations.ValidationResult.Success
            : new System.ComponentModel.DataAnnotations.ValidationResult("Only alphanumeric characters are allowed.");
    }

    public static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    
    public static bool IsValidGuid(string guid)
    {
        return Guid.TryParse(guid, out _);
    }

   
    public static bool IsValidPostalCode(string postalCode, string country = "US")
    {
        if (string.IsNullOrWhiteSpace(postalCode))
            return false;

        var regex = country.ToUpper() switch
        {
            "US" => new Regex(@"^\d{5}(-\d{4})?$"),
            "UK" => new Regex(@"^[A-Z]{1,2}\d{1,2}[A-Z]?\s?\d[A-Z]{2}$"),
            "CA" => new Regex(@"^[A-Z]\d[A-Z]\s?\d[A-Z]\d$"),
            _ => new Regex(@"^[A-Z0-9\s-]{3,10}$")
        };

        return regex.IsMatch(postalCode.ToUpper());
    }

    public static bool IsValidCreditCard(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return false;

        cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

        if (!cardNumber.All(char.IsDigit))
            return false;

        var sum = 0;
        var alternate = false;

        for (var i = cardNumber.Length - 1; i >= 0; i--)
        {
            var digit = int.Parse(cardNumber[i].ToString());

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }

  
    public static bool IsStrongPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;

        var hasUpper = password.Any(char.IsUpper);
        var hasLower = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        var strengthCount = new[] { hasUpper, hasLower, hasDigit, hasSpecial }.Count(x => x);
        return strengthCount >= 2;
    }

    public static System.ComponentModel.DataAnnotations.ValidationResult? IsStrongPassword(object? value, ValidationContext context)
    {
        var input = value as string;
        return IsStrongPassword(input ?? string.Empty)
            ? System.ComponentModel.DataAnnotations.ValidationResult.Success
            : new System.ComponentModel.DataAnnotations.ValidationResult("Password is not strong enough.");
    }

 
    public static bool IsValidUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        var regex = new Regex(@"^[a-zA-Z0-9_]{3,20}$");
        return regex.IsMatch(username);
    }

    public static System.ComponentModel.DataAnnotations.ValidationResult? IsValidUsername(object? value, ValidationContext context)
    {
        var input = value as string;
        return IsValidUsername(input ?? string.Empty)
            ? System.ComponentModel.DataAnnotations.ValidationResult.Success
            : new System.ComponentModel.DataAnnotations.ValidationResult("Invalid username format.");
    }

    
    public static bool IsValidFileExtension(string fileName, string[] allowedExtensions)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }

    public static bool IsValidFileSize(long fileSize, long maxSizeInBytes)
    {
        return fileSize <= maxSizeInBytes;
    }

  
    public static bool IsValidEnumValue<TEnum>(string value) where TEnum : struct
    {
        return Enum.TryParse<TEnum>(value, true, out _);
    }

    public static bool IsNotNull(object? obj)
    {
        return obj != null;
    }

 
    public static bool IsNotEmpty<T>(IEnumerable<T>? collection)
    {
        return collection != null && collection.Any();
    }

    
    public static bool IsNotNullOrWhiteSpace(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    // Explicit DataAnnotations callback methods (unique names)
    public static System.ComponentModel.DataAnnotations.ValidationResult? ValidateUsernameAttribute(object? value, ValidationContext context)
    {
        var input = value as string;
        return IsValidUsername(input ?? string.Empty)
            ? System.ComponentModel.DataAnnotations.ValidationResult.Success
            : new System.ComponentModel.DataAnnotations.ValidationResult("Invalid username format.");
    }

    public static System.ComponentModel.DataAnnotations.ValidationResult? ValidateAlphanumericAttribute(object? value, ValidationContext context)
    {
        var input = value as string;
        return IsAlphanumeric(input ?? string.Empty)
            ? System.ComponentModel.DataAnnotations.ValidationResult.Success
            : new System.ComponentModel.DataAnnotations.ValidationResult("Only alphanumeric characters are allowed.");
    }

    public static System.ComponentModel.DataAnnotations.ValidationResult? ValidateAlphaOnlyAttribute(object? value, ValidationContext context)
    {
        var input = value as string;
        return IsAlphaOnly(input ?? string.Empty)
            ? System.ComponentModel.DataAnnotations.ValidationResult.Success
            : new System.ComponentModel.DataAnnotations.ValidationResult("Only alphabetic characters are allowed.");
    }

    public static System.ComponentModel.DataAnnotations.ValidationResult? ValidateStrongPasswordAttribute(object? value, ValidationContext context)
    {
        var input = value as string;
        return IsStrongPassword(input ?? string.Empty)
            ? System.ComponentModel.DataAnnotations.ValidationResult.Success
            : new System.ComponentModel.DataAnnotations.ValidationResult("Password is not strong enough.");
    }

    public static System.ComponentModel.DataAnnotations.ValidationResult? ValidateEmailAttribute(object? value, ValidationContext context)
    {
        var input = value as string;
        if (string.IsNullOrWhiteSpace(input)) return System.ComponentModel.DataAnnotations.ValidationResult.Success;
        return IsValidEmail(input)
            ? System.ComponentModel.DataAnnotations.ValidationResult.Success
            : new System.ComponentModel.DataAnnotations.ValidationResult("Invalid email format.");
    }

    public static System.ComponentModel.DataAnnotations.ValidationResult? ValidatePhoneAttribute(object? value, ValidationContext context)
    {
        var input = value as string;
        if (string.IsNullOrWhiteSpace(input)) return System.ComponentModel.DataAnnotations.ValidationResult.Success;
        return IsValidPhone(input)
            ? System.ComponentModel.DataAnnotations.ValidationResult.Success
            : new System.ComponentModel.DataAnnotations.ValidationResult("Invalid phone format.");
    }
}


public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, string> IsValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(CustomValidators.IsValidEmail)
            .WithMessage("'{PropertyName}' must be a valid email address.");
    }

    public static IRuleBuilderOptions<T, string> IsValidPhone<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(CustomValidators.IsValidPhone)
            .WithMessage("'{PropertyName}' must be a valid phone number.");
    }

    public static IRuleBuilderOptions<T, string> IsValidTinNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(CustomValidators.IsValidTinNumber)
            .WithMessage("'{PropertyName}' must be a valid TIN number.");
    }

    public static IRuleBuilderOptions<T, string> IsValidSku<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(CustomValidators.IsValidSku)
            .WithMessage("'{PropertyName}' must be a valid SKU format.");
    }

    public static IRuleBuilderOptions<T, string> IsValidTagNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(CustomValidators.IsValidTagNumber)
            .WithMessage("'{PropertyName}' must be a valid tag number format.");
    }

    public static IRuleBuilderOptions<T, string> IsValidGrnNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(CustomValidators.IsValidGrnNumber)
            .WithMessage("'{PropertyName}' must be a valid GRN number format.");
    }

    public static IRuleBuilderOptions<T, string> IsValidSrNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(CustomValidators.IsValidSrNumber)
            .WithMessage("'{PropertyName}' must be a valid SR number format.");
    }

    public static IRuleBuilderOptions<T, string> IsValidSivNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(CustomValidators.IsValidSivNumber)
            .WithMessage("'{PropertyName}' must be a valid SIV number format.");
    }

    public static IRuleBuilderOptions<T, DateTime> IsNotFutureDate<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
    {
        return ruleBuilder.Must(CustomValidators.IsNotFutureDate)
            .WithMessage("'{PropertyName}' cannot be in the future.");
    }

    public static IRuleBuilderOptions<T, DateTime> IsNotPastDate<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
    {
        return ruleBuilder.Must(CustomValidators.IsNotPastDate)
            .WithMessage("'{PropertyName}' cannot be in the past.");
    }

    public static IRuleBuilderOptions<T, int> IsValidQuantity<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder.Must(CustomValidators.IsValidQuantity)
            .WithMessage("'{PropertyName}' must be greater than 0.");
    }

    public static IRuleBuilderOptions<T, decimal> IsValidPrice<T>(this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder.Must(CustomValidators.IsValidPrice)
            .WithMessage("'{PropertyName}' must be greater than or equal to 0.");
    }

    public static IRuleBuilderOptions<T, string> IsValidUsername<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(CustomValidators.IsValidUsername)
            .WithMessage("'{PropertyName}' must be 3-20 characters and contain only letters, numbers, and underscore.");
    }

    public static IRuleBuilderOptions<T, string> IsStrongPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(CustomValidators.IsStrongPassword)
            .WithMessage("'{PropertyName}' must be at least 8 characters and contain uppercase, lowercase, number, and special character.");
    }

    public static IRuleBuilderOptions<T, string> IsValidUrl<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(CustomValidators.IsValidUrl)
            .WithMessage("'{PropertyName}' must be a valid URL.");
    }
}