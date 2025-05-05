using SelfServicePortal.Web.Models;
using System.Diagnostics;

public class ErrorHandlingMiddleware(
    RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);

            if (context.Response.StatusCode == StatusCodes.Status404NotFound && !context.Response.HasStarted)
            {
                context.Request.Path = "/Home/NotFound";
                await next(context);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred.");

            context.Request.Path = "/Home/Error";
            var errorModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? context.TraceIdentifier,
                ErrorMessage = ex.Message,
                StackTrace = env.IsDevelopment() ? ex.StackTrace : null
            };

            context.Items["ErrorModel"] = errorModel;
            await next(context);
        }
    }
}