using System;
using System.Collections.Generic;
using System.Linq;

namespace Paccia
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetEnumerableMembers<T>() => Enum.GetValues(typeof(T)).Cast<T>();
    }
}