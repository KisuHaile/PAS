using System.ComponentModel.DataAnnotations;
using PAS.API.Validators;

namespace PAS.API.Models.Requests;

public class RegisterRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    [CustomValidation(typeof(CustomValidators), nameof(CustomValidators.ValidateUsernameAttribute))]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(20)]
    [CustomValidation(typeof(CustomValidators), nameof(CustomValidators.ValidateAlphanumericAttribute))]
    public string? EmployeeCode { get; set; }

    [Required]
    [StringLength(50)]
    [CustomValidation(typeof(CustomValidators), nameof(CustomValidators.ValidateAlphaOnlyAttribute))]
    public string Department { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [CustomValidation(typeof(CustomValidators), nameof(CustomValidators.ValidateStrongPasswordAttribute))]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [StringLength(50)]
    public string? RoleName { get; set; }

    [EmailAddress]
    [CustomValidation(typeof(CustomValidators), nameof(CustomValidators.ValidateEmailAttribute))]
    public string? Email { get; set; }

    [CustomValidation(typeof(CustomValidators), nameof(CustomValidators.ValidatePhoneAttribute))]
    public string? PhoneNumber { get; set; }
}