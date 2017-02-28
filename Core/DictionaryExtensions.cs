using System.Collections.Generic;

namespace Paccia
{
    static class DictionaryExtensions
    {
        internal static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            TValue value;

            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }

        internal static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) => 
            dictionary.GetValueOrDefault(key, default(TValue));

        internal static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> values)
        {
            foreach (var pair in values)
                dictionary.Add(pair);
        }
    }
}