using SelfServicePortal.Core.DTOs;
using SelfServicePortal.Web.Models;

namespace SelfServicePortal.Web.Extensions;

public static class MappingExtensions
{
    public static IncidentFilterDto ToDto(this IncidentFilterViewModel model)
    {
        return new IncidentFilterDto
        {
            CallType = model.CallType,
            Module = model.Module,
            Priority = model.Priority,
            SupportStatus = model.SupportStatus,
            UserStatus = model.UserStatus,
            AssignedToId = model.AssignedToId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            PageNumber = model.PageNumber,
            PageSize = model.PageSize
        };
    }
}