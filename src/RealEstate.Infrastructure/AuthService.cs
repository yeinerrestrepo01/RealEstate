using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using RealEstate.Application.Services;

namespace RealEstate.Infrastructure
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config) => _config = config;

        private static byte[] B64(string v) => Convert.FromBase64String(v);

        public Task<AuthUser?> ValidateAsync(string username, string password, CancellationToken ct = default)
        {
            var users = _config.GetSection("Auth:Users").GetChildren();
            foreach (var u in users)
            {
                var uname = u["Username"];
                if (!string.Equals(uname, username, StringComparison.OrdinalIgnoreCase))
                    continue;

                var role = u["Role"] ?? "User";
                var salt = B64(u["Salt"]!);
                var hash = B64(u["Hash"]!);
                var iterations = int.TryParse(u["Iterations"], out var it) ? it : 120_000;
                var calc = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, hash.Length);
                if (CryptographicOperations.FixedTimeEquals(calc, hash))
                {
                    return Task.FromResult<AuthUser?>(new AuthUser(uname!, role));
                }
                return Task.FromResult<AuthUser?>(null);
            }
            return Task.FromResult<AuthUser?>(null);
        }
    }
}