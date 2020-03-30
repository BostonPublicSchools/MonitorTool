using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Wolfpack.Core.Interfaces;
using Castle.Core.Internal;

namespace Wolfpack.Core
{
    public static class Extensions
    {
        public static string TrimEnd(this string s, string remove)
        {
            return !s.EndsWith(remove, StringComparison.InvariantCultureIgnoreCase) 
                ? s 
                : s.Substring(0, s.Length - remove.Length);
        }

        public static IEnumerable<string> Lines(this StreamReader source)
        {
            string line;

            if (source == null)
                throw new ArgumentNullException("source");

            while ((line = source.ReadLine()) != null)
            {
                yield return line;
            }
        }

        /// <summary>
        /// Provides an inline conditional append format method. The supplied text is
        /// only appended if the <paramref name="test"/> is true.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="test"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static StringBuilder AppendIf(this StringBuilder sb, bool test, string format, params object[] args)
        {
            return !test ? sb : sb.AppendFormat(format, args);
        }

        /// <summary>
        /// Provides an inline conditional append format method. The supplied text is
        /// only appended if the <paramref name="test"/> is true.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="test"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static StringBuilder AppendLineIf(this StringBuilder sb, bool test, string format, params object[] args)
        {
            return !test ? sb : sb.AppendLine(string.Format(format, args));
        }

        public static bool InitialiseIfEnabled(this IPlugin plugin)
        {
            if (plugin is ICanBeSwitchedOff)
            {
                if (!((ICanBeSwitchedOff)plugin).Enabled)
                    return false;
            }

            Logger.Debug("Initialising component...'{0}'", plugin.GetType().Name);
            plugin.Initialise();
            return true;
        }

        public static bool ContainsAll(this List<string> collection, params string[] terms)
        {
            if (collection == null)
                return false;
            if (terms == null || terms.Length == 0)
                return true;

            return terms.All(t => collection.Contains(t, StringComparer.OrdinalIgnoreCase));
        }

        public static bool ContainsAny(this List<string> collection, params string[] terms)
        {
            if (collection == null)
                return false;
            if (terms == null || terms.Length == 0)
                return true;

            return terms.Any(t => collection.Contains(t, StringComparer.OrdinalIgnoreCase));
        }

        public static void AddIfMissing(this List<string> collection, params string[] tags)
        {
            if (collection == null)
                throw new NullReferenceException("string collection is null, unable to Add()");
            if (tags == null || tags.Length == 0)
                return;

            collection.AddRange(tags.Except(collection).ToArray());
        }

        public static void AddIfMissing(this Dictionary<string, string> dictionary, params KeyValuePair<string, string>[] entries)
        {
            if (dictionary == null)
                throw new NullReferenceException("dictionary is null, unable to Add()");
            if (entries == null || entries.Length == 0)
                return;

            entries.ForEach(e =>
                                {
                                    if (!dictionary.Contains(e))
                                        dictionary.Add(e.Key, e.Value);
                                });
        }

        public static void AddIfMissing(this Dictionary<string, string> dictionary, params Tuple<string, string>[] entries)
        {
            if (dictionary == null)
                throw new NullReferenceException("dictionary is null, unable to Add()");
            if (entries == null || entries.Length == 0)
                return;

            entries.ForEach(e =>
                                {
                                    if (!dictionary.ContainsKey(e.Item1))
                                        dictionary.Add(e.Item1, e.Item2);
                                });
        }
    }
}