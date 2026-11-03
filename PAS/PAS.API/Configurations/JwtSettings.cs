namespace PAS.API.Configurations;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 480;
    public int RefreshTokenExpirationDays { get; set; } = 7;

    // compatibility aliases
    public string Key { get => Secret; set => Secret = value; }
    public int ExpiryInHours
    {
        get => AccessTokenExpirationMinutes / 60;
        set => AccessTokenExpirationMinutes = value * 60;
    }
}