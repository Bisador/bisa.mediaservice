using System.Security.Claims;

namespace MediaService.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("sub")?.Value;
    }
    
    public static Guid GetTenantId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirst("tenant_id")?.Value;

        if (!Guid.TryParse(value, out var tenantId))
            throw new UnauthorizedAccessException();

        return tenantId;
    } 
}