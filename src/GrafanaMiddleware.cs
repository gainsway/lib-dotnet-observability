using Microsoft.AspNetCore.Http;
using OpenTelemetry.Trace;

namespace Gainsway.Observability;

public class GrafanaMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var currentSpan = Tracer.CurrentSpan;

        context.Response.Headers.Append(
            "server-timing",
            $"traceparent;desc=\"00-{currentSpan.Context.TraceId}-{currentSpan.Context.SpanId}-01\""
        );
        await _next(context);
    }
}
