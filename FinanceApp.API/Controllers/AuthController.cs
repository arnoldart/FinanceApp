using Microsoft.AspNetCore.Mvc;
using FinanceApp.API.Data;
using FinanceApp.API.DTOs.Auth;
using FinanceApp.API.Models;
using FinanceApp.API.Services;
using Microsoft.AspNetCore.Authorization;

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

        var accessToken = _jwtService.GenerateToken(user.Id, user.Email);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
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

    [HttpPost("refresh")]
    public IActionResult Refresh()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieName];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized(new { message = "Refresh token tidak ditemukan." });
        }

        var token = _context.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);

        if (token == null)
        {
            return Unauthorized(new { message = "Refresh token tidak valid." });
        }

        if (token.Revoked || token.ExpiresAt < DateTime.UtcNow)
        {
            return Unauthorized(new { message = "Refresh token sudah tidak berlaku." });
        }

        token.Revoked = true;

        var newRefresh = _jwtService.GenerateRefreshToken();

        var refreshEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = token.UserId,
            DeviceId = token.DeviceId,
            Token = newRefresh,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshEntity);
        _context.SaveChanges();
    
        var user = _context.Users.Find(token.UserId);
        var accessToken = _jwtService.GenerateToken(user.Id, user.Email);

        SetRefreshTokenCookie(newRefresh, refreshEntity.ExpiresAt);

        return Ok(new
        {
            accessToken
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieName];

        var token = _context.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);

        if(token != null)
        {
            _context.RefreshTokens.Remove(token);
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
