using System.Diagnostics;
using System.Diagnostics.Metrics;
using Impostor.Api.Net.Manager;

namespace Boot.Metrics;

public class ClientMetrics
{
    private readonly IClientManager _clientManager;

    private readonly Dictionary<TagList, int> _state = new(new TagListEqualityComparer());

    public ClientMetrics(IMeterFactory meterFactory, IClientManager clientManager)
    {
        var meter = meterFactory.Create("Boot.Metrics.Client");
        meter.CreateObservableUpDownCounter("boot.metrics.client", CalculateClientCount, "{clients}", "Amount of currently connected clients");
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
            var tags = new TagList
            {
                { "language", client.Language.ToString() },
                { "platform", client.PlatformSpecificData.Platform.ToString() },
                { "game_version", client.GameVersion },
                { "chat_mode", client.ChatMode },
            };

            _state.TryGetValue(tags, out var count);
            _state[tags] = count + 1;
        }

        // Yield all combinations we found
        foreach (var (key, val) in _state)
        {
            yield return new Measurement<int>(val, key);
        }
    }
}
