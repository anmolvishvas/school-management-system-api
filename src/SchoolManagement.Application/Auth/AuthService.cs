using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolManagement.Application.Auth.Dtos;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly ITeacherRepository _teachers;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository users, ITeacherRepository teachers, IConfiguration configuration)
    {
        _users = users;
        _teachers = teachers;
        _configuration = configuration;
    }

    public async Task<string> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        if (await _users.UsernameExistsAsync(dto.Username, cancellationToken))
            throw new InvalidOperationException("Username is already taken.");

        var roleTrim = dto.Role.Trim();
        var normalizedRole = ApplicationRoles.All.FirstOrDefault(a =>
            string.Equals(a, roleTrim, StringComparison.OrdinalIgnoreCase));
        if (normalizedRole == null)
            throw new InvalidOperationException("Invalid role.");

        var user = new User
        {
            Username = dto.Username.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = normalizedRole
        };

        await _users.AddAsync(user, cancellationToken);

        // Auto-provision Teacher profile so client doesn't need a second API call.
        if (string.Equals(normalizedRole, ApplicationRoles.Teacher, StringComparison.OrdinalIgnoreCase))
        {
            var existingTeacher = await _teachers.GetByUserIdAsync(user.Id, cancellationToken);
            if (existingTeacher == null)
            {
                await _teachers.AddAsync(
                    new Teacher
                    {
                        UserId = user.Id,
                        FullName = user.Username,
                        Email = user.Username.Contains('@') ? user.Username : null,
                        IsActive = true
                    },
                    cancellationToken);
            }
        }

        return "User created";
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByUsernameAsync(dto.Username, cancellationToken);
        if (user == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        var jwtSection = _configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);
        var expires = DateTime.UtcNow.AddHours(8);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256));

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponseDto
        {
            Token = tokenString,
            Username = user.Username,
            Role = user.Role,
            ExpiresAtUtc = expires
        };
    }
}
