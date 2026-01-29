using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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

            public PermissionFilter(string[] requiredPermissions)
            {
                _requiredPermissions = requiredPermissions;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                // Verificar se usuário está autenticado
                var user = context.HttpContext.User;
                if (!user.Identity?.IsAuthenticated ?? true)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                // Pegar role do usuário
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                if (roleClaim == null)
                {
                    context.Result = new ForbidResult();
                    return;
                }

                var userRole = roleClaim.Value;

                // Verificar se tem alguma das permissões necessárias
                var hasPermission = Permissions.HasAnyPermission(userRole, _requiredPermissions);

                if (!hasPermission)
                {
                    context.Result = new ForbidResult();
                    return;
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
}