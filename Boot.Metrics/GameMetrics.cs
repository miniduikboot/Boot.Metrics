namespace Boot.Metrics;

using System.Diagnostics;
using System.Diagnostics.Metrics;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;

/// <summary>
/// Count the amount of games currently hosted by an Impostor server, implemented using an Async Instrument.
/// </summary>
public class GameMetrics
{
    private readonly IGameManager _gameManager;
    private readonly Dictionary<TagList, int> _state = new(new TagListEqualityComparer());

    public GameMetrics(IMeterFactory meterFactory, IGameManager gameManager)
    {
        var meter = meterFactory.Create("Boot.Metrics.Game");
        meter.CreateObservableUpDownCounter("boot.metrics.game", CalculateGameCount, "{games}", "Amount of currently open games");
        _gameManager = gameManager;
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
            // Bucket playercount above 20 to reduce cardinality
            var playerCount = game.PlayerCount;
            if (playerCount > 20)
            {
                playerCount -= playerCount % 10;
            }

            var tags = new TagList
            {
                { "game_mode", game.Options.GameMode.ToString() },
                { "map", game.Options.Map.ToString() },
                { "player_count", playerCount },
                { "public", game.IsPublic },
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

    private readonly record struct TaggedGame(
        GameModes GameMode,
        MapTypes Map,
        int PlayerCount,
        bool PublicMode);
}
