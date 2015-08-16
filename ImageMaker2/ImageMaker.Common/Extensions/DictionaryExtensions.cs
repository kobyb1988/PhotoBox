using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Monads;
using System.Text;
using System.Threading.Tasks;

namespace ImageMaker.Common.Extensions
{
    public static class DictionaryExtensions
    {
        public static void Append<TKey, TValue>(this IDictionary<TKey, ObservableCollection<TValue>> source, TKey key, TValue value) 
        {
            if (source.ContainsKey(key))
            {
                source[key].Recover(() => new ObservableCollection<TValue>()).Add(value);
            }
            else
            {
                source.Add(key, new ObservableCollection<TValue> { value });
            }
        }

        public static void Clear<TKey, TValue>(this IDictionary<TKey, ObservableCollection<TValue>> source, TKey key)
        {
            if (!source.ContainsKey(key))
                return;

            var val = source[key];
            foreach (var item in val.ToList())
            {
                val.Remove(item);
            }

            source[key] = null;
            source.Keys.Remove(key);
        }

        public static void Delete<TKey, TValue>(this IDictionary<TKey, ObservableCollection<TValue>> source, TKey key, TValue value)
        {
            if (!source.ContainsKey(key))
                return;

            IList<TValue> val = source[key];
            if (val == null)
                return;

            val.Remove(value);
            if (val.Count == 0)
            {
                source.Clear(key);
            }
        }
    }
}
