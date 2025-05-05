using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace SelfServicePortal.Application.Common.Authorization;

public static class IncidentOperations
{
    public static OperationAuthorizationRequirement Read =
        new()
        { Name = nameof(Read) };
    public static OperationAuthorizationRequirement Update =
        new()
        { Name = nameof(Update) };
    public static OperationAuthorizationRequirement Assign =
        new()
        { Name = nameof(Assign) };
}
