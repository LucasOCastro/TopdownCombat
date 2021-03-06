using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatGame
{
    public static class CollectionUtility
    {

        private static System.Exception firstFailError = new System.Exception("Failed to get first item in collection.");
        public static T FirstOrDefault<T>(this IList<T> list) => list.Count > 0 ? list[0] : default;
        public static T FirstOrThrow<T>(this IList<T> list) => list.Count > 0 ? list[0] : throw firstFailError;

        public static T FirstOrDefault<T>(this IEnumerable<T> col)
        {            
            foreach (T t in col) return t;
            return default;
        }
        public static T FirstOrThrow<T>(this IEnumerable<T> col)
        {
            foreach (T t in col) return t;
            throw firstFailError;
        }

        public static T2 GetOrDefault<T1, T2>(this Dictionary<T1, T2> dict, T1 key, T2 defaultValue = default) => dict.TryGetValue(key, out T2 val) ? val : defaultValue;

        public static T2 GetOrAdd<T1, T2>(this Dictionary<T1, T2> dict, T1 key, T2 defaultValue = default)
        {

            if (!dict.TryGetValue(key, out T2 val)){
                dict.Add(key, defaultValue);
                val = defaultValue;
            }
            return val;
        }

        public static T2 GetOrNew<T1, T2>(this Dictionary<T1, T2> dict, T1 key) where T2: new() => dict.TryGetValue(key, out T2 val) ? val : new T2();
        public static T2 GetOrAddNew<T1, T2>(this Dictionary<T1, T2> dict, T1 key) where T2: new()
        {
            if (!dict.TryGetValue(key, out T2 val)){
                val = new T2();
                dict.Add(key, val);
            }
            return val;
        }
        public static T2[] GetOrAddNew<T1, T2>(this Dictionary<T1, T2[]> dict, T1 key, int newSize)
        {
            if (!dict.TryGetValue(key, out T2[] val)){
                val = new T2[newSize];
                dict.Add(key, val);
            }
            return val;
        }

        public static string ToStringEnumerable<T>(this IEnumerable<T> col, string separator = " || ")
        {
            StringBuilder tx = new StringBuilder();
            foreach (T t in col)
            {
                tx.Append(t.ToString());
                tx.Append(separator);
            }
            return tx.ToString();
        }

        public static string ToStringEnumerable<T>(this IList<T> list, string separator = " || ")
        {
            StringBuilder tx = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                tx.Append(list[i].ToString());
                tx.Append(separator);
            }
            return tx.ToString();
        }

        public static T RandomElement<T>(this IList<T> list)
        {
            if (list.Count == 0) throw new System.Exception("Tried to get random element from empty collection.");
            return list[Random.RandiRange(0, list.Count)];
        }

        public static T RandomElementByWeight<T>(this IList<T> list, System.Func<T, float> weightSelector)
        {
            if (list.Count == 0) throw new System.Exception("Tried to get random element from empty collection.");

            float[] weightSums = new float[list.Count];
            float totalWeight = 0f;
            for (int i = 0; i < list.Count; i++)
            {
                weightSums[i] = totalWeight + weightSelector(list[i]);
            }

            float rand = Random.RandfRange(0f, totalWeight);
            for (int i = 0; i < list.Count; i++)
            {
                if (rand <= weightSums[i]){
                    return list[i];
                }
            }
            return default;
        }
    }
    
}