using System;
using System.Collections.Generic;
using System.Linq;

namespace AudioBoos.Server.Helpers;

public static class LinqExtensions {
    public static TResult MostCommon<TSource, TResult>(this IEnumerable<TSource> items,
        Func<TSource, TResult> predicate) {
        return items.Select(predicate)
            .GroupBy(x => x)
            .OrderByDescending(x => x.Count())
            .First().Key;
    }
}
