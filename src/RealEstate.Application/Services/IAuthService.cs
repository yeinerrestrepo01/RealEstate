namespace RealEstate.Application.Services
{
    public interface IAuthService
    {
        Task<AuthUser?> ValidateAsync(string username, string password, CancellationToken ct = default);
    }
    public record AuthUser(string Username, string Role);
}
