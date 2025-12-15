using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Pathfinding;

public static class PathCache
{
    private static readonly ConcurrentDictionary<(long, int, int, int, int, int), PathResult> Cache = new();

    public static bool TryGetValue((long, int, int, int, int, int) key, [MaybeNullWhen(false)] out PathResult pathResult)
    {
        return Cache.TryGetValue(key, out pathResult);
    }

    public static void Set((long, int, int, int, int, int) key, PathResult pathResult)
    {
        Cache[key] = pathResult;
    }

    public static void Clear()
    {
        Cache.Clear();
    }
}
