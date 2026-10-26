namespace PAS.API.Configurations;


public class SwaggerSettings
{
  
    public string Version { get; set; } = "v1";

    public string Title { get; set; } = "ECX Property Automation System API";

    public string Description { get; set; }

    public string ContactName { get; set; } = "ECX Property Division";

    public string ContactEmail { get; set; } = "property@ecx.com.et";

    public string ContactUrl { get; set; } = "https://www.ecx.com.et";

    public string LicenseName { get; set; } = "Proprietary";

    public string LicenseUrl { get; set; }

  
    public string TermsOfServiceUrl { get; set; }

  
    public bool IncludeXmlComments { get; set; } = true;

    public string XmlCommentsPath { get; set; }

   

    public bool EnableSwaggerUI { get; set; } = true;


    public string RoutePrefix { get; set; } = "swagger";
    public List<SwaggerServer> Servers { get; set; } = new();

    public SwaggerSecurity Security { get; set; } = new();
}
public class SwaggerServer
{
    public string Url { get; set; }
    public string Description { get; set; }
}

public class SwaggerSecurity
{
    public string Name { get; set; } = "Bearer";
    public string Scheme { get; set; } = "bearer";
    public string BearerFormat { get; set; } = "JWT";
    public string Description { get; set; } = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token";
}