using Microsoft.AspNetCore.Http;
using OpenTelemetry.Trace;

namespace Common.Middleware;

public static class TraceIdHeaderMiddleware
{
    public static async Task Handler(HttpContext context, Func<Task> next)
    {
        var traceId = Tracer.CurrentSpan.Context.TraceId.ToHexString();
        if (!string.IsNullOrWhiteSpace(traceId))
        {
            context.Response.Headers.Add("x-trace-id", traceId);
        }
        await next.Invoke();
    }
}