using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Unseal.Constants;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

namespace Unseal.Middlewares;

public class CustomTenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ICurrentTenant _currentTenant;
    
    public CustomTenantMiddleware(
        RequestDelegate next,
        ICurrentTenant currentTenant)
    {
        _next = next;
        _currentTenant = currentTenant;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers[AuthConstants.Authorization].ToString();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith(AuthConstants.Bearer))
        {
            try
            {
                var token = authHeader.Substring($"{AuthConstants.Bearer} ".Length).Trim();
                token = token.Trim((char)65279);
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(token))
                {
                    var jwtToken = handler.ReadJwtToken(token);
                    var tenantIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == AbpClaimTypes.TenantId)?.Value;

                    if (!string.IsNullOrEmpty(tenantIdClaim) && Guid.TryParse(tenantIdClaim, out var tenantId))
                    {
                        using (_currentTenant.Change(tenantId))
                        {
                            await _next(context);
                            return;
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        await _next(context);
    }
}