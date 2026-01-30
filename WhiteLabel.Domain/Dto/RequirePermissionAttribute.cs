using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using WhiteLabel.Domain.Entity.Enum;

namespace WhiteLabel.Domain.Dto
{
    /// <summary>
    /// Atributo para verificar permissões específicas
    /// </summary>
    public class RequirePermissionAttribute : TypeFilterAttribute
    {
        public RequirePermissionAttribute(params string[] permissions)
            : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { permissions };
        }

        private class PermissionFilter : IAuthorizationFilter
        {
            private readonly string[] _requiredPermissions;
            private readonly ILogger<PermissionFilter> _logger;

            public PermissionFilter(string[] requiredPermissions, ILogger<PermissionFilter> logger)
            {
                _requiredPermissions = requiredPermissions;
                _logger = logger;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                _logger.LogInformation("🔍 [PERMISSION CHECK] Starting authorization check");
                _logger.LogInformation("   Required permissions: {Permissions}", string.Join(", ", _requiredPermissions));

                // Verificar se usuário está autenticado
                var user = context.HttpContext.User;
                var isAuthenticated = user.Identity?.IsAuthenticated ?? false;

                _logger.LogInformation("   IsAuthenticated: {IsAuth}", isAuthenticated);

                if (!isAuthenticated)
                {
                    _logger.LogWarning("❌ User not authenticated");
                    context.Result = new UnauthorizedResult();
                    return;
                }

                // Pegar role do usuário
                var roleClaim = user.FindFirst(ClaimTypes.Role);

                _logger.LogInformation("   Role claim found: {HasRole}", roleClaim != null);

                if (roleClaim == null)
                {
                    _logger.LogWarning("❌ No role claim found in token");

                    // Log all claims for debugging
                    var allClaims = user.Claims.Select(c => $"{c.Type}={c.Value}");
                    _logger.LogWarning("   Available claims: {Claims}", string.Join(", ", allClaims));

                    context.Result = new ForbidResult();
                    return;
                }

                var userRole = roleClaim.Value;
                _logger.LogInformation("   User role: {Role}", userRole);

                // Verificar se tem alguma das permissões necessárias
                var rolePermissions = Permissions.GetRolePermissions(userRole);
                _logger.LogInformation("   Role permissions: {RolePerms}", string.Join(", ", rolePermissions));

                var hasPermission = Permissions.HasAnyPermission(userRole, _requiredPermissions);
                _logger.LogInformation("   Has required permission: {HasPerm}", hasPermission);

                if (!hasPermission)
                {
                    _logger.LogWarning("❌ User does not have required permissions");
                    _logger.LogWarning("   User has: {UserPerms}", string.Join(", ", rolePermissions));
                    _logger.LogWarning("   Needs one of: {NeededPerms}", string.Join(", ", _requiredPermissions));

                    context.Result = new ForbidResult();
                    return;
                }

                _logger.LogInformation("✅ Permission check passed");
            }
        }
    }

    /// <summary>
    /// Atributo para permitir apenas Administradores
    /// </summary>
    public class RequireAdminAttribute : RequirePermissionAttribute
    {
        public RequireAdminAttribute() : base(Permissions.ManageSettings)
        {
        }
    }
}