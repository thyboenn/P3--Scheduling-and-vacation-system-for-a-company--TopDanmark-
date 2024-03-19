using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Backend.Api.Messages;
using Backend.Api.Models;
using Backend.Api.Options;
using Backend.Api.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Api.Services;

public class AuthService : IAuthService
{
  private readonly IAssociateRepository _associateRepository;
  private readonly IAssociateService _associateService;
  private readonly JwtSettings _jwtSettings;
  private readonly TokenValidationParameters _tokenValidationParameters;

  public AuthService(
    IAssociateRepository associateRepository,
    IAssociateService associateService,
    JwtSettings jwtSettings,
    TokenValidationParameters tokenValidationParameters
  )
  {
    _associateRepository = associateRepository;
    _associateService = associateService;
    _jwtSettings = jwtSettings;
    _tokenValidationParameters = tokenValidationParameters;
  }

  public (string fingerprint, AuthResponseDTO response)? Login(LoginDTO request)
  {
    var associate = _associateRepository.ReadByEmail(request.email);

    if (
      associate is null
      || associate.PasswordSalt is null
      || associate.PasswordHash is null
      || !VerifyPassword(
        request.password,
        associate.PasswordSalt,
        associate.PasswordHash
      )
    )
    {
      return null;
    }

    return GenerateAuthResponse(associate);
  }

  public (string fingerprint, AuthResponseDTO response)? Register(
    RegisterDTO request
  )
  {
    if (_associateRepository.ReadByEmail(request.email) == null)
      return null;

    var createdAssociateId = _associateService
      .CreateAssociate(new(request.email, request.name))
      .id;
    var associate = _associateRepository.Read(createdAssociateId)!;

    (associate.PasswordSalt, associate.PasswordHash) = CreatePasswordHash(
      request.password
    );
    associate = _associateRepository.Update(associate);

    return GenerateAuthResponse(associate);
  }

  public (string fingerprint, AuthResponseDTO response)? Refresh(
    RefreshDTO request,
    string fingerprint
  )
  {
    var principal = GetPrincipalFromToken(request.refreshToken);
    if (principal is null)
      return null;

    string? fingerprintHash = principal.Claims
      .FirstOrDefault(claim => claim.Type == CustomClaims.FingerprintHash)
      ?.Value;
    string? refreshUserId = principal.Claims
      .FirstOrDefault(claim => claim.Type == CustomClaims.RefreshUserId)
      ?.Value;

    if (
      string.IsNullOrEmpty(fingerprintHash) || string.IsNullOrEmpty(refreshUserId)
    )
      return null;

    if (
      string.IsNullOrEmpty(fingerprint)
      || HashFingerprint(fingerprint) != fingerprintHash
    )
      return null;

    var user = _associateRepository.Read(new Guid(refreshUserId));

    return GenerateAuthResponse(user);
  }

  private ClaimsPrincipal? GetPrincipalFromToken(string token)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    try
    {
      var principal = tokenHandler.ValidateToken(
        token,
        _tokenValidationParameters,
        out SecurityToken validatedToken
      );

      if (
        validatedToken is JwtSecurityToken jwtSecurityToken
        && string.Equals(
          jwtSecurityToken.Header.Alg,
          SecurityAlgorithms.HmacSha512,
          StringComparison.OrdinalIgnoreCase
        )
      )
      {
        return principal;
      }
    }
    catch { }

    return null;
  }

  private (string fingerprint, AuthResponseDTO response)? GenerateAuthResponse(
    Associate? associate
  )
  {
    if (associate is null)
      return null;

    var accessToken = GenerateAccessToken(associate);
    var fingerprint = GenerateFingerprint();
    var fingerprintHash = HashFingerprint(fingerprint);
    var refreshToken = GenerateRefreshToken(associate, fingerprintHash);

    return (fingerprint, new AuthResponseDTO(accessToken, refreshToken));
  }

  private string HashFingerprint(string fingerprint)
  {
    var fingerprintBytes = Encoding.UTF8.GetBytes(fingerprint);

    using var hasher = SHA512.Create();

    return Convert.ToBase64String(hasher.ComputeHash(fingerprintBytes));
  }

  private string GenerateFingerprint()
  {
    var charArray = "0123456789abcdef".ToCharArray();

    var result = new char[50];
    for (var i = 0; i < result.Length; i++)
    {
      result[i] = charArray[RandomNumberGenerator.GetInt32(charArray.Length)];
    }

    return new string(result);
  }

  private string GenerateRefreshToken(Associate associate, string fingerprintHash)
  {
    var claims = new Claim[]
    {
      new(CustomClaims.RefreshUserId, associate.Id.ToString()),
      new(CustomClaims.FingerprintHash, fingerprintHash)
    };

    var expires = DateTime.Now.AddMinutes(60);

    return GenerateJwt(claims, expires);
  }

  private string GenerateAccessToken(Associate associate)
  {
    var claims = new Claim[] { new(CustomClaims.UserId, associate.Id.ToString()), };

    var expires = DateTime.Now.AddMinutes(10);

    return GenerateJwt(claims, expires);
  }

  private string GenerateJwt(Claim[] claims, DateTime expires)
  {
    var secretKey = new SymmetricSecurityKey(
      Encoding.UTF8.GetBytes(_jwtSettings.Secret)
    );

    var credentials = new SigningCredentials(
      secretKey,
      SecurityAlgorithms.HmacSha512
    );

    var token = new JwtSecurityToken(
      issuer: _jwtSettings.Issuer,
      audience: _jwtSettings.Audience,
      claims: claims,
      expires: expires,
      signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  private (byte[] salt, byte[] hash) CreatePasswordHash(string password)
  {
    using var hmac = new HMACSHA512();
    return (hmac.Key, hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
  }

  private bool VerifyPassword(string password, byte[] knownSalt, byte[] correctHash)
  {
    using var hmac = new HMACSHA512(knownSalt);
    var hashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

    if (correctHash.Length != hashedPassword.Length)
      return false;

    for (int i = 0; i < hashedPassword.Length; i++)
    {
      if (hashedPassword[i] != correctHash[i])
      {
        return false;
      }
    }

    return true;
  }
}

public static class CustomClaims
{
  public static string UserId => "userId";
  public static string RefreshUserId => "refreshUserId";
  public static string FingerprintHash => "fingerprintHash";
}
