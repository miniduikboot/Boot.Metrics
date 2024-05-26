namespace Boot.Metrics;

using Impostor.Api.Events.Managers;
using Impostor.Api.Plugins;

/// <summary>Main class of the metrics plugin.</summary>
[ImpostorPlugin("at.duikbo.metrics")]
public class BootMetricsPlugin : PluginBase
{
    public BootMetricsPlugin(IEventManager eventManager, GameMetrics gameMetrics, ClientMetrics clientMetrics)
    {
        eventManager.RegisterListener(clientMetrics);
        eventManager.RegisterListener(gameMetrics);
    }
}
