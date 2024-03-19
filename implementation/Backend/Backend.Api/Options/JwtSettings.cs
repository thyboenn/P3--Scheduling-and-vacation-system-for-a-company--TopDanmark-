namespace Backend.Api.Options;

public class JwtSettings
{
  public string Audience { get; set; } = string.Empty;
  public string Issuer { get; set; } = string.Empty;
  public string Secret { get; set; } = string.Empty;
}
