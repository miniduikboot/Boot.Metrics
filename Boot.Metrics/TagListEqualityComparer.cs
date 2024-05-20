using System.Diagnostics;

namespace Boot.Metrics;

/// <summary>
/// Helper class to compare tag lists.
/// </summary>
internal sealed class TagListEqualityComparer : IEqualityComparer<TagList>
{
    public bool Equals(TagList x, TagList y)
    {
        return x.Count == y.Count && x.SequenceEqual(y);
    }

    public int GetHashCode(TagList obj)
    {
        var hashCode = default(HashCode);

        foreach (var (key, value) in obj)
        {
            hashCode.Add(key);
            hashCode.Add(value);
        }

        return hashCode.ToHashCode();
    }
}
