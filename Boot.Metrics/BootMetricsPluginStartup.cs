namespace Boot.Metrics;

using System.Net;
using Impostor.Api.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

public class BootMetricsPluginStartup : IPluginHttpStartup
{
    /// <inheritdoc/>
    public void ConfigureHost(IHostBuilder host)
    {
        // Implementation mandatory but not required
    }

    /// <inheritdoc/>
    public void ConfigureServices(IServiceCollection services)
    {

        var otel = services.AddOpenTelemetry();

        // Configure OpenTelemetry Resources with the application name
        otel.ConfigureResource(resource => resource
            .AddService(serviceName: "Impostor"));

        // Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
        otel.WithMetrics(metrics => metrics
            // Metrics provider from OpenTelemetry
            .AddAspNetCoreInstrumentation()
            // Metrics provides by ASP.NET Core in .NET 8
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddPrometheusExporter());

        // tracing omitted because that'd require adding spans in Impostor
    }

    public void ConfigureWebApplication(IApplicationBuilder builder)
    {
        builder.UseOpenTelemetryPrometheusScrapingEndpoint();
    }
}
