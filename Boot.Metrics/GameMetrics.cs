namespace Boot.Metrics;

using System.Diagnostics;
using System.Diagnostics.Metrics;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;

public class GameMetrics
{
    private readonly ObservableUpDownCounter<int> _gameCount;
    private readonly IGameManager _gameManager;
    private readonly ILogger<GameMetrics> _logger;
    private readonly HashSet<TaggedGame> _priorReportedSet = new();

    public GameMetrics(IMeterFactory meterFactory, IGameManager gameManager, ILogger<GameMetrics> logger)
    {
        var meter = meterFactory.Create("Boot.Metrics.Game");
        _gameCount = meter.CreateObservableUpDownCounter("boot.metrics.game", CalculateGameCount, "{games}", "Amoung of currently open games");
        _gameManager = gameManager;
        _logger = logger;
        _logger.LogInformation("Started GameMetrics");
    }

    private IEnumerable<Measurement<int>> CalculateGameCount()
    {
        var dict = new Dictionary<TaggedGame, int>();
        _logger.LogInformation("Calculating GameMetrics");

        // Collect all games with the values we want to store them with
        foreach (var game in _gameManager.Games)
        {
            // Bucket playercount above 20 to reduce cardinality
            var playerCount = game.PlayerCount;
            if (playerCount > 20)
            {
                playerCount -= playerCount % 10;
            }

            var tags = new TaggedGame(
                game.Options.GameMode,
                game.Options.Map,
                playerCount,
                game.IsPublic);
            dict.TryGetValue(tags, out var count);
            dict[tags] = count + 1;
        }

        // Convert them into measurements
        var result = new List<Measurement<int>>();

        // Remove the categories that no longer contain games
        foreach (var key in _priorReportedSet)
        {
            if (!dict.ContainsKey(key))
            {
                _logger.LogInformation("{Key}: gone", key);
                var tags = new TagList
                {
                    { "GameMode", key.GameMode.ToString() },
                    { "Map", key.Map.ToString() },
                    { "PlayerCount", key.PlayerCount },
                    { "Public", key.PublicMode },
                };
                result.Add(new Measurement<int>(0, tags));
            }
        }

        // Add the new measurements
        foreach (var (key, val) in dict)
        {
            _logger.LogInformation("{Key}: {Value}", key, val);
            var tags = new TagList
            {
                { "GameMode", key.GameMode.ToString() },
                { "Map", key.Map.ToString() },
                { "PlayerCount", key.PlayerCount },
                { "Public", key.PublicMode },
            };
            result.Add(new Measurement<int>(val, tags));
            _priorReportedSet.Add(key);
        }

        return result;
    }

    private readonly record struct TaggedGame(
        GameModes GameMode,
        MapTypes Map,
        int PlayerCount,
        bool PublicMode);
}
