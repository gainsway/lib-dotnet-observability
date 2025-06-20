using Microsoft.AspNetCore.Builder;

namespace Gainsway.Observability;

public static class GrafanaMiddlewareExtensions
{
    public static IApplicationBuilder UseGrafanaMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<GrafanaMiddleware>();
}
