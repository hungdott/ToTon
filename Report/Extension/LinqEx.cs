using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Extension
{
    public static class LinqEx
    {
        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, int, bool> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }
        public static IEnumerable<TSource> DistinctBy<TSource,TKey>(this IEnumerable<TSource> source,Func<TSource, TKey> property)
        {
            return source.GroupBy(property).Select(x => x.First());
        }

        //public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
        //{
        //    return source.Reverse();
        //}


    }
}