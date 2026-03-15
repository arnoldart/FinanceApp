using Microsoft.AspNetCore.Mvc;
using FinanceApp.API.Data;
using FinanceApp.API.DTOs.Auth;
using FinanceApp.API.Models;
using FinanceApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinanceApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private const string RefreshTokenCookieName = "refreshToken";
    private readonly FinanceDbContext _context;
    private readonly PasswordService _passwordService;
    private readonly JwtService _jwtService;

    public AuthController(FinanceDbContext context, PasswordService passwordService, JwtService jwtService)
    {
        _context = context;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    [Authorize]
    [HttpGet]
    public IActionResult GetUsers()
    {
        return Ok(_context.Users.ToList());
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { message = "Token tidak valid." });
        }

        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        // Console.WriteLine("INIIII LOGGGG=========================");
        // Console.WriteLine(userId);
        // Console.WriteLine("INIIII LOGGGG=========================");

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
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email == dto.Email);

        if(user == null)
        {
            return Unauthorized();
        }

        var isValid = _passwordService.VerifyPassword(user.PasswordHash, dto.Password);

        if(!isValid)
        {
            return Unauthorized();
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

        return Ok(new
        {
            accessToken
        });
    }

    [HttpPost("Register")]
    public IActionResult Register([FromBody] RegisterDto dto)
    {
        if (_context.Users.Any(u => u.Email == dto.Email))
        {
            return Conflict(new { message = "Email already exists." });
        }

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = _passwordService.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, new
        {
            user.Id,
            user.Name,
            user.Email,
            user.CreatedAt,
            user.UpdatedAt
        });
    }

    [Authorize]
    [HttpPost("refresh")]
    public IActionResult Refresh()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieName];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized(new { message = "Refresh token tidak ditemukan." });
        }

        var refreshTokenHash = _jwtService.HashToken(refreshToken);
        var token = _context.RefreshTokens.FirstOrDefault(t => t.Token == refreshTokenHash);

        if (token == null)
        {
            return Unauthorized(new { message = "Refresh token tidak valid." });
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
            }

            return Unauthorized(new { message = "Refresh token sudah tidak berlaku." });
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
            return Unauthorized(new { message = "User tidak ditemukan." });
        }

        var accessToken = _jwtService.GenerateToken(user.Id, user.Email, user.TokenVersion);

        SetRefreshTokenCookie(newRefresh, refreshEntity.ExpiresAt);

        return Ok(new
        {
            accessToken
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieName];
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            Response.Cookies.Delete(RefreshTokenCookieName);
            return Ok(new { message = "Logged out successfully." });
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
        }

        Response.Cookies.Delete(RefreshTokenCookieName);

        return Ok(new { message = "Logged out successfully." });
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
}
