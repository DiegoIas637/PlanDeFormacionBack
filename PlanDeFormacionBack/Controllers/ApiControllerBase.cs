using System.Security.Claims;
using Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace PlanDeFormacionBack.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected int GetCurrentUserId()
        {
            var subject = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(subject, out var userId))
            {
                throw new UnauthorizedException("No se pudo identificar al usuario autenticado.");
            }

            return userId;
        }
    }
}
