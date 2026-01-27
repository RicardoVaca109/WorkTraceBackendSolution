using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WorkTrace.Application.Repositories;

public class ClientEvaluationAuthorizeAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var token = context.HttpContext.Request.Headers["X-Client-Token"].FirstOrDefault();

        if (string.IsNullOrEmpty(token))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var repo = context.HttpContext.RequestServices
            .GetRequiredService<IClientEvaluationSessionRepository>();

        var session = await repo.GetByTokenAsync(token);

        if (session == null || !session.IsAuthenticated || session.IsUsed)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        context.HttpContext.Items["ClientSession"] = session;
        await next();
    }
}