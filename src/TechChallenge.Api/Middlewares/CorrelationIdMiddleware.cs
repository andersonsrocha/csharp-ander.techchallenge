namespace TechChallenge.Api.Middlewares;
using Serilog.Context;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.Headers.TryGetValue("X-Correlation-Id", out var correlationIds);
        var correlationId = correlationIds.FirstOrDefault() ?? Guid.NewGuid().ToString();

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            context.Items["CorrelationId"] = correlationId;
            await next(context);
        }
    }
}