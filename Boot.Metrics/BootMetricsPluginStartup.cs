namespace Boot.Metrics;

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
        // No implementation needed
    }

    /// <inheritdoc/>
    public void ConfigureServices(IServiceCollection services)
    {
        // Register metrics
        services.AddSingleton<ClientMetrics>();
        services.AddSingleton<GameMetrics>();

        // Set up OpenTelemetry
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
            .AddMeter("System.Net.Http")
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()

            // Add our own metrics
            .AddMeter("Boot.Metrics.*")

            // Export to Prometheus
            .AddPrometheusExporter());

        // tracing omitted because that'd require adding spans in Impostor
    }

    /// <inheritdoc/>
    public void ConfigureWebApplication(IApplicationBuilder builder)
    {
        builder.UseOpenTelemetryPrometheusScrapingEndpoint();
    }
}
