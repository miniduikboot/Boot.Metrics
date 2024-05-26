using System.Diagnostics;
using System.Diagnostics.Metrics;
using Impostor.Api.Events;
using Impostor.Api.Events.Client;
using Impostor.Api.Net;
using Impostor.Api.Net.Manager;

namespace Boot.Metrics;

public class ClientMetrics : IEventListener
{
    private readonly Counter<int> _totalClientCounter;
    private readonly IClientManager _clientManager;
    private readonly Dictionary<TagList, int> _state = new(new TagListEqualityComparer());

    public ClientMetrics(IMeterFactory meterFactory, IClientManager clientManager)
    {
        var meter = meterFactory.Create("Boot.Metrics.Client");
        meter.CreateObservableUpDownCounter("boot.metrics.clients.connected", CalculateClientCount, string.Empty, "Amount of currently connected clients");
        _totalClientCounter = meter.CreateCounter<int>("boot.metrics.clients.total", string.Empty, "Amount of clients that have connected to this server");
        _clientManager = clientManager;
    }

    private IEnumerable<Measurement<int>> CalculateClientCount()
    {
        // Reset internal state
        foreach (var (key, _) in _state)
        {
            _state[key] = 0;
        }

        // Iterate over all clients
        foreach (var client in _clientManager.Clients)
        {
            var tags = GetClientTags(client);

            _state.TryGetValue(tags, out var count);
            _state[tags] = count + 1;
        }

        // Yield all combinations we found
        foreach (var (key, val) in _state)
        {
            yield return new Measurement<int>(val, key);
        }
    }

    [EventListener]
    public void OnClientConnected(IClientConnectedEvent e)
    {
        _totalClientCounter.Add(1, GetClientTags(e.Client));
    }

    private TagList GetClientTags(IClient client)
    {
        return new TagList
        {
            { "language", client.Language.ToString() },
            { "platform", client.PlatformSpecificData.Platform.ToString() },
            { "game_version", client.GameVersion },
            { "chat_mode", client.ChatMode },
            { "is_connected", client.Connection?.IsConnected },
        };
    }
}
