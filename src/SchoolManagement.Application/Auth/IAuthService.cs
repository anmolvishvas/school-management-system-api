using SchoolManagement.Application.Auth.Dtos;

namespace SchoolManagement.Application.Auth;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
}
