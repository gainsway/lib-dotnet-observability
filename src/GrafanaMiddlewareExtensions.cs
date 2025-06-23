using Microsoft.AspNetCore.Builder;

namespace Gainsway.Observability;

public static class GrafanaMiddlewareExtensions
{
    /// <summary>
    /// This middleware adds a "server-timing" header to the response,
    /// which is useful for monitoring and tracing in Grafana.
    /// It's manadatory to integrate frontend and backend tracing in Grafana.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseGrafanaMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<GrafanaMiddleware>();
}
