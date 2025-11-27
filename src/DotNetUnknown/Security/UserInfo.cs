namespace DotNetUnknown.Security;

public record UserInfo(string UserId, string Username, string Email, List<string> Roles, string Status = "Active");