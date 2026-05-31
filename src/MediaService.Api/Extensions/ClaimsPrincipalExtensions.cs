using System.Security.Claims;

namespace MediaService.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("sub")?.Value;
    }
    
    public static Guid? GetTenantId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirst("tenant_id")?.Value;

        return Guid.TryParse(value, out var tenantId)
            ? tenantId
            : null;
    }
}