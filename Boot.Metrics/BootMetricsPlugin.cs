namespace Boot.Metrics;

using Impostor.Api.Plugins;

/// <summary>Main class of the metrics plugin.</summary>
[ImpostorPlugin("at.duikbo.metrics")]
public class BootMetricsPlugin : PluginBase
{
    // HACK: Register metrics here so DI activates them
    public BootMetricsPlugin(GameMetrics gameMetrics, ClientMetrics clientMetrics)
    {
        // Implementation not necessary
    }
}
