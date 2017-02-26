using System;
using System.Collections.Generic;

namespace Paccia
{
    static class EnumerableExtensions
    {
        internal static EnumerableStream<T> ToStream<T>(this IEnumerable<T> source, Func<T, byte[]> map) =>
            new EnumerableStream<T>(source, map);
    }
}