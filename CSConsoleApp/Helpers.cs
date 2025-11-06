using System.Collections.Generic;

{
    public static class Helpers
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) where TKey : notnull
        {
            if (dict.TryGetValue(key, out var value)) return value;
            return default(TValue);
        }
    }
}