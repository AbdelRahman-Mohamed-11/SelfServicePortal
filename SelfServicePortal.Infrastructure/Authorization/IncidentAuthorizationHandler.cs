using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using SelfServicePortal.Core.Entities.Identity.Enums;
using SelfServicePortal.Core.Entities;
using System.Security.Claims;
using SelfServicePortal.Application.Common.Authorization;
using Microsoft.Extensions.Logging;

namespace SelfServicePortal.Infrastructure.Authorization;

public class IncidentAuthorizationHandler
    : AuthorizationHandler<OperationAuthorizationRequirement, Incident>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Incident incident)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Task.CompletedTask;

        if (context.User.IsInRole(Role.Admin.ToString()))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (context.User.IsInRole(Role.ERP.ToString()))
        {
            if (requirement == IncidentOperations.Read ||
                requirement == IncidentOperations.Update)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }

        if (incident.LoggedById.ToString() == userIdClaim)
        {
            if (requirement == IncidentOperations.Read ||
                requirement == IncidentOperations.Update)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}