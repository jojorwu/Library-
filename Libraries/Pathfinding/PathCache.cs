using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Pathfinding;

public static class PathCache
{
    private static readonly ConcurrentDictionary<(long, int, int, int, int, bool, bool), PathResult> Cache = new();

    public static bool TryGetValue((long, int, int, int, int, bool, bool) key, [MaybeNullWhen(false)] out PathResult pathResult)
    {
        return Cache.TryGetValue(key, out pathResult);
    }

    public static void Set((long, int, int, int, int, bool, bool) key, PathResult pathResult)
    {
        Cache[key] = pathResult;
    }

    public static void Clear()
    {
        Cache.Clear();
    }
}
