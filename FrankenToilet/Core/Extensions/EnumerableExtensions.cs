using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FrankenToilet.Core.Extensions;

[PublicAPI]
public static class EnumerableExtensions
{
    extension(IEnumerable source)
    {
        public TTo[] CastToArray<TTo>() => source.Cast<TTo>().ToArray();
    }
    extension<T>(IEnumerable<T> source) where T : class
    {
        public IEnumerable<T> WhereNotNull() => source.Where(static o => o != null);
    }
}