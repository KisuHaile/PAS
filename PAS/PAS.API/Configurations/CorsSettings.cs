namespace PAS.API.Configurations;
public class CorsSettings
{
    public string PolicyName { get; set; } = "AllowSpecificOrigin";
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
    public string[] AllowedMethods { get; set; } = new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS" };
    public string[] AllowedHeaders { get; set; } = new[] { "Authorization", "Content-Type", "Accept", "Origin", "X-Requested-With" };
    public string[] ExposedHeaders { get; set; } = Array.Empty<string>();
    public bool AllowCredentials { get; set; } = true;

    public int PreflightMaxAgeSeconds { get; set; } = 86400; // 24 hours

    public Dictionary<string, CorsPolicyConfig> AdditionalPolicies { get; set; } = new();
}

public class CorsPolicyConfig
{
    public string[] AllowedOrigins { get; set; }
    public string[] AllowedMethods { get; set; }
    public string[] AllowedHeaders { get; set; }
    public bool AllowCredentials { get; set; }
}