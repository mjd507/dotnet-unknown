using DotNetUnknown.Security;

namespace DotNetUnknown.Tests.Support;

public sealed class JwtTokenTestSupport(JwtTokenUtils jwtTokenUtils)
{
    private static readonly UserInfo Alice = new("user-id-123", "alice", "alice@example.com", ["User"]);

    private static readonly UserInfo Bob = new("user-id-456", "bob", "bob@example.com", ["User", "Admin"]);

    public string NormalUserToken => jwtTokenUtils.GenerateToken(Alice);
    public string AdminUserToken => jwtTokenUtils.GenerateToken(Bob);
}