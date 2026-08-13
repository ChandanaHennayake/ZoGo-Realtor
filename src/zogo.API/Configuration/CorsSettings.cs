namespace zogo.API.Configuration;

public class CorsSettings
{
    public const string SectionName = "Cors";

    public string[] AllowedOrigins { get; set; } = [];
    public string[] AllowedMethods { get; set; } = ["GET", "POST", "PUT", "PATCH", "DELETE"];
    public string[] AllowedHeaders { get; set; } = ["*"];
    public bool AllowCredentials { get; set; } = true;
}
