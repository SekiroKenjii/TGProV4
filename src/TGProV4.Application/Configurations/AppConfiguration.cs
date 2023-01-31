namespace TGProV4.Application.Configurations;

public class AppConfiguration
{
    public string? Secret { get; set; }
    public string? ApplicationUrl { get; set; }
    public int ApiMajorVersion { get; set; }
    public int ApiMinorVersion { get; set; }
    public string? ApiVersionGroupNameFormat { get; set; }
}
