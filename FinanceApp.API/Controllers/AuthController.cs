using Microsoft.AspNetCore.Mvc;
using FinanceApp.API.Data;
using FinanceApp.API.DTOs.Auth;
using FinanceApp.API.Models;
using FinanceApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace FinanceApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private const string RefreshTokenCookieName = "refreshToken";
    private readonly FinanceDbContext _context;
    private readonly PasswordService _passwordService;
    private readonly JwtService _jwtService;
    private readonly EmailService _emailService;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        FinanceDbContext context,
        PasswordService passwordService,
        EmailService emailService,
        JwtService jwtService,
        IConfiguration config,
        ILogger<AuthController> logger)
    {
        _context = context;
        _passwordService = passwordService;
        _emailService = emailService;
        _jwtService = jwtService;
        _config = config;
        _logger = logger;
    }

    [Authorize]
    [HttpGet]
    public IActionResult GetUsers()
    {
        return Ok(_context.Users.ToList());
    }

    [Authorize]
    [HttpGet("me")]
    [EnableRateLimiting("fixed")]
    public IActionResult Me()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { message = "Token tidak valid." });
        }

        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user is null)
        {
            return NotFound(new { message = "User tidak ditemukan." });
        }

        return Ok(new
        {
            user.Id,
            user.Name,
            user.Email,
            user.CreatedAt,
            user.UpdatedAt
        });
    }

    [HttpPost("Login")]
    [EnableRateLimiting("fixed")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email == dto.Email);

        if(user == null)
        {
            _logger.LogWarning(
                "auth.login.failed user_not_found email={Email} ip={Ip} ua={UserAgent}",
                dto.Email,
                GetRemoteIp(),
                GetUserAgent());

            return Error(StatusCodes.Status401Unauthorized, "Email atau password salah.");
        }

        var isValid = _passwordService.VerifyPassword(user.PasswordHash, dto.Password);

        if(!isValid)
        {
            _logger.LogWarning(
                "auth.login.failed invalid_password userId={UserId} email={Email} ip={Ip} ua={UserAgent}",
                user.Id,
                user.Email,
                GetRemoteIp(),
                GetUserAgent());

            return Error(StatusCodes.Status401Unauthorized, "Email atau password salah.");
        }

        if (!user.IsEmailVerified)
        {
            _logger.LogWarning(
                "auth.login.failed email_not_verified userId={UserId} email={Email} ip={Ip} ua={UserAgent}",
                user.Id,
                user.Email,
                GetRemoteIp(),
                GetUserAgent());

            return Error(StatusCodes.Status403Forbidden, "Email belum diverifikasi. Silakan cek inbox email kamu.");
        }

        var accessToken = _jwtService.GenerateToken(user.Id, user.Email, user.TokenVersion);
        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenHash = _jwtService.HashToken(refreshToken);

        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
        };

        _context.RefreshTokens.Add(token);
        _context.SaveChanges();

        SetRefreshTokenCookie(refreshToken, token.ExpiresAt);

        _logger.LogInformation(
            "auth.login.success userId={UserId} email={Email} ip={Ip} ua={UserAgent}",
            user.Id,
            user.Email,
            GetRemoteIp(),
            GetUserAgent());

        return Success("Login berhasil.", new
        {
            accessToken
        });
    }

    [HttpPost("Register")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (_context.Users.Any(u => u.Email == dto.Email))
        {
            return Conflict(new { message = "Email already exists." });
        }

        var verifyCode = GenerateVerificationCode();
        var verifyToken = Guid.NewGuid().ToString("N");
        var verifyExpiry = DateTime.UtcNow.AddMinutes(30);

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = _passwordService.HashPassword(dto.Password),
            EmailVerificationCode = verifyCode,
            EmailVerificationToken = verifyToken,
            EmailVerificationExpiresAt = verifyExpiry,
            IsEmailVerified = false
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        var verifyPageBaseUrl = _config["App:VerifyEmailUrl"] ?? "http://localhost:3000/email-verify";
        var verifyPageUrl =
            $"{verifyPageBaseUrl}?email={WebUtility.UrlEncode(user.Email)}&token={WebUtility.UrlEncode(verifyToken)}";

        var subject = "Verifikasi Email FinanceApp";
        var body = BuildVerificationEmailHtml(user.Name, verifyCode, verifyPageUrl);

        try
        {
            await _emailService.SendEmailAsync(user.Email, subject, body, isHtml: true);

            _logger.LogInformation(
                "auth.register.verification_sent userId={UserId} email={Email} ip={Ip} ua={UserAgent}",
                user.Id,
                user.Email,
                GetRemoteIp(),
                GetUserAgent());
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "auth.register.verification_send_failed userId={UserId} email={Email} ip={Ip} ua={UserAgent}",
                user.Id,
                user.Email,
                GetRemoteIp(),
                GetUserAgent());

            return Error(StatusCodes.Status500InternalServerError, "Registrasi berhasil, tapi gagal kirim email verifikasi.");
        }

        return StatusCode(StatusCodes.Status201Created, new
        {
            success = true,
            message = "Registrasi berhasil. Cek email untuk verifikasi akun.",
            data = new
            {
                user.Id,
                user.Name,
                user.Email,
                user.CreatedAt,
                user.UpdatedAt
            }
        });
    }

    [HttpPost("verify-email")]
    [EnableRateLimiting("fixed")]
    public IActionResult VerifyEmail([FromBody] VerifyEmailDto dto)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

        if (user is null)
        {
            _logger.LogWarning(
                "auth.verify_email.failed user_not_found email={Email} ip={Ip} ua={UserAgent}",
                dto.Email,
                GetRemoteIp(),
                GetUserAgent());

            return Error(StatusCodes.Status404NotFound, "User tidak ditemukan.");
        }

        if (user.IsEmailVerified)
        {
            return Success("Email sudah terverifikasi.");
        }

        if (user.EmailVerificationExpiresAt is null || user.EmailVerificationExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning(
                "auth.verify_email.failed expired userId={UserId} email={Email} ip={Ip} ua={UserAgent}",
                user.Id,
                user.Email,
                GetRemoteIp(),
                GetUserAgent());

            return Error(StatusCodes.Status400BadRequest, "Kode verifikasi sudah kedaluwarsa.");
        }

        var codeProvided = !string.IsNullOrWhiteSpace(dto.Code);
        var tokenProvided = !string.IsNullOrWhiteSpace(dto.Token);

        if (!codeProvided && !tokenProvided)
        {
            return Error(StatusCodes.Status400BadRequest, "Code atau token verifikasi harus dikirim.");
        }

        var isCodeValid = codeProvided &&
            string.Equals(user.EmailVerificationCode, dto.Code, StringComparison.Ordinal);
        var isTokenValid = tokenProvided &&
            string.Equals(user.EmailVerificationToken, dto.Token, StringComparison.Ordinal);

        if (!isCodeValid && !isTokenValid)
        {
            _logger.LogWarning(
                "auth.verify_email.failed invalid_code_or_token userId={UserId} email={Email} ip={Ip} ua={UserAgent}",
                user.Id,
                user.Email,
                GetRemoteIp(),
                GetUserAgent());

            return Error(StatusCodes.Status400BadRequest, "Code atau token verifikasi tidak valid.");
        }

        user.IsEmailVerified = true;
        user.EmailVerifiedAt = DateTime.UtcNow;
        user.EmailVerificationCode = null;
        user.EmailVerificationToken = null;
        user.EmailVerificationExpiresAt = null;

        _context.SaveChanges();

        _logger.LogInformation(
            "auth.verify_email.success userId={UserId} email={Email} ip={Ip} ua={UserAgent}",
            user.Id,
            user.Email,
            GetRemoteIp(),
            GetUserAgent());

        return Success("Email berhasil diverifikasi.");
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("fixed")]
    public IActionResult Refresh()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieName];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            _logger.LogWarning(
                "auth.refresh.failed token_missing ip={Ip} ua={UserAgent}",
                GetRemoteIp(),
                GetUserAgent());

            return Error(StatusCodes.Status401Unauthorized, "Refresh token tidak ditemukan.");
        }

        var refreshTokenHash = _jwtService.HashToken(refreshToken);
        var token = _context.RefreshTokens.FirstOrDefault(t => t.Token == refreshTokenHash);

        if (token == null)
        {
            _logger.LogWarning(
                "auth.refresh.failed token_invalid ip={Ip} ua={UserAgent}",
                GetRemoteIp(),
                GetUserAgent());

            return Error(StatusCodes.Status401Unauthorized, "Refresh token tidak valid.");
        }

        if (token.Revoked || token.ExpiresAt < DateTime.UtcNow)
        {
            var compromisedUser = _context.Users.Find(token.UserId);
            if (compromisedUser is not null)
            {
                compromisedUser.TokenVersion += 1;

                var activeTokens = _context.RefreshTokens.Where(t => t.UserId == token.UserId && !t.Revoked);
                foreach (var activeToken in activeTokens)
                {
                    activeToken.Revoked = true;
                }

                _context.SaveChanges();

                _logger.LogWarning(
                    "auth.refresh.reuse_detected userId={UserId} revokedAll=true ip={Ip} ua={UserAgent}",
                    compromisedUser.Id,
                    GetRemoteIp(),
                    GetUserAgent());
            }

            return Error(StatusCodes.Status401Unauthorized, "Refresh token sudah tidak berlaku.");
        }

        token.Revoked = true;

        var newRefresh = _jwtService.GenerateRefreshToken();
        var newRefreshHash = _jwtService.HashToken(newRefresh);

        var refreshEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = token.UserId,
            DeviceId = token.DeviceId,
            Token = newRefreshHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshEntity);
        _context.SaveChanges();
    
        var user = _context.Users.Find(token.UserId);
        if (user is null)
        {
            _logger.LogWarning(
                "auth.refresh.failed user_not_found userId={UserId} ip={Ip} ua={UserAgent}",
                token.UserId,
                GetRemoteIp(),
                GetUserAgent());

            return Error(StatusCodes.Status401Unauthorized, "User tidak ditemukan.");
        }

        var accessToken = _jwtService.GenerateToken(user.Id, user.Email, user.TokenVersion);

        SetRefreshTokenCookie(newRefresh, refreshEntity.ExpiresAt);

        _logger.LogInformation(
            "auth.refresh.success userId={UserId} ip={Ip} ua={UserAgent}",
            user.Id,
            GetRemoteIp(),
            GetUserAgent());

        return Success("Token berhasil diperbarui.", new
        {
            accessToken
        });
    }

    [HttpPost("logout")]
    [EnableRateLimiting("fixed")]
    public IActionResult Logout()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieName];
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            Response.Cookies.Delete(RefreshTokenCookieName);

            _logger.LogInformation(
                "auth.logout.success no_token_cookie ip={Ip} ua={UserAgent}",
                GetRemoteIp(),
                GetUserAgent());

            return Success("Logout berhasil.");
        }

        var refreshTokenHash = _jwtService.HashToken(refreshToken);
        var token = _context.RefreshTokens.FirstOrDefault(t => t.Token == refreshTokenHash);

        if(token != null)
        {
            token.Revoked = true;

            var user = _context.Users.Find(token.UserId);
            if (user is not null)
            {
                user.TokenVersion += 1;
            }

            _context.SaveChanges();

            _logger.LogInformation(
                "auth.logout.success userId={UserId} tokenRevoked=true ip={Ip} ua={UserAgent}",
                token.UserId,
                GetRemoteIp(),
                GetUserAgent());
        }
        else
        {
            _logger.LogWarning(
                "auth.logout.token_not_found ip={Ip} ua={UserAgent}",
                GetRemoteIp(),
                GetUserAgent());
        }

        Response.Cookies.Delete(RefreshTokenCookieName);

        return Success("Logout berhasil.");
    }

    private void SetRefreshTokenCookie(string refreshToken, DateTime expiresAt)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = new DateTimeOffset(expiresAt)
        };

        Response.Cookies.Append(RefreshTokenCookieName, refreshToken, cookieOptions);
    }

    private IActionResult Success(string message, object? data = null)
    {
        return Ok(new
        {
            success = true,
            message,
            data
        });
    }

    private IActionResult Error(int statusCode, string message)
    {
        return StatusCode(statusCode, new
        {
            success = false,
            message
        });
    }

    private string GetRemoteIp()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private string GetUserAgent()
    {
        return Request.Headers.UserAgent.ToString();
    }

    private static string GenerateVerificationCode()
    {
        return RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
    }

    private static string BuildVerificationEmailHtml(string name, string code, string verifyPageUrl)
    {
        return $@"
<div style='font-family:Arial,sans-serif;line-height:1.5;color:#111'>
  <h2>Verifikasi Email Kamu</h2>
  <p>Halo {WebUtility.HtmlEncode(name)}, terima kasih sudah daftar di FinanceApp.</p>
  <p>Kode verifikasi kamu:</p>
  <p style='font-size:24px;font-weight:bold;letter-spacing:2px'>{WebUtility.HtmlEncode(code)}</p>
  <p>Atau klik tombol berikut untuk langsung menuju halaman verifikasi:</p>
  <p>
    <a href='{WebUtility.HtmlEncode(verifyPageUrl)}'
       style='display:inline-block;padding:10px 16px;background:#0b5fff;color:#fff;text-decoration:none;border-radius:6px'>
       Verifikasi Email
    </a>
  </p>
  <p>Kode ini berlaku 30 menit.</p>
</div>";
    }
}
