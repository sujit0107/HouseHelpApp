using System.Security.Claims;

namespace HouseHelp.Api.Extensions;

public static class UserExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
        return value is not null && Guid.TryParse(value, out var id) ? id : throw new InvalidOperationException("Missing user id");
    }

    public static string[] GetPermissions(this ClaimsPrincipal user)
    {
        return user.FindAll("permission").Select(x => x.Value).ToArray();
    }
}
