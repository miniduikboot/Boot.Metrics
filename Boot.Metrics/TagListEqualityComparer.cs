using System.Diagnostics;

namespace Boot.Metrics;

/// <summary>
/// Helper class to compare tag lists.
/// </summary>
/// <remarks>
/// The order of tags in the tag list is assumed to be equal in both lists.
///
/// Note that this is also necessary for OTel to perform optimally.
/// </remarks>
internal sealed class TagListEqualityComparer : IEqualityComparer<TagList>
{
    public bool Equals(TagList x, TagList y)
    {
        return x.Count == y.Count && x.SequenceEqual(y);
    }

    public int GetHashCode(TagList obj)
    {
        return obj.GetHashCode();
    }
}
