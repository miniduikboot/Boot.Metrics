namespace Boot.Metrics;

using System.Diagnostics;
using System.Diagnostics.Metrics;
using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;

/// <summary>
/// Count the amount of games currently hosted by an Impostor server, implemented using an Async Instrument.
/// </summary>
public class GameMetrics : IEventListener
{
    private readonly Counter<int> _totalGameCounter;
    private readonly IGameManager _gameManager;
    private readonly Dictionary<TagList, int> _state = new(new TagListEqualityComparer());

    public GameMetrics(IMeterFactory meterFactory, IGameManager gameManager)
    {
        var meter = meterFactory.Create("Boot.Metrics.Game");
        meter.CreateObservableUpDownCounter("boot.metrics.games.open", CalculateGameCount, "{games}", "Amount of currently open games");
        _totalGameCounter = meter.CreateCounter<int>("boot.metrics.games.total", string.Empty, "Count started games");
        _gameManager = gameManager;
    }

    private enum MetricsProfile
    {
        StartedGames,
        OpenGames,
    }

    private IEnumerable<Measurement<int>> CalculateGameCount()
    {
        // Reset internal state
        foreach (var (key, _) in _state)
        {
            _state[key] = 0;
        }

        // Collect all games with the values we want to store them with
        foreach (var game in _gameManager.Games)
        {
            var tags = GetGameTags(game, MetricsProfile.OpenGames);
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
    public void OnGameStarted(IGameStartedEvent e)
    {
        _totalGameCounter.Add(1, GetGameTags(e.Game, MetricsProfile.StartedGames));
    }

    private static TagList GetGameTags(IGame game, MetricsProfile profile)
    {
        // Bucket playercount above 20 to reduce cardinality
        var playerCount = game.PlayerCount;
        if (playerCount > 20)
        {
            playerCount -= playerCount % 10;
        }

        var list = new TagList
        {
            { "game_mode", game.Options.GameMode.ToString() },
            { "map", game.Options.Map.ToString() },
            { "player_count", playerCount },
            { "public", game.IsPublic },
        };

        // Adding game state doesn't make sense for StartedGames, as they'll all be in the started state.
        if (profile == MetricsProfile.OpenGames)
        {
            list.Add("game_state", game.GameState);
        }

        return list;
    }
}
