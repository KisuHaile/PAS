using System.ComponentModel.DataAnnotations;
using PAS.API.Validators;

namespace PAS.API.Models.Requests;

public class LoginRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    [CustomValidation(typeof(CustomValidators), nameof(CustomValidators.ValidateUsernameAttribute))]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}