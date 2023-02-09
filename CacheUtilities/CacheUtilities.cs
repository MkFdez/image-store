using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace CacheUtilities
{
    public class ChacheUtilities
    {
        public static Func<string, string> getMail = x => new StreamReader(Path.Combine(HttpContext.Current.Server.MapPath("~/Content/MailBodies/"), x+".txt")).ReadToEnd();

        private static TimeSpan DEFAULT_SLIDING_EXP = TimeSpan.Zero;
        private static DateTime DEFAULT_ABSOLUTE_EXP = DateTime.Now.AddDays(30);
        public static T GetCached<T>(string key, Func<T,T> initializer, TimeSpan slidingExpiration, DateTime absoluteExpiration)
        {
            var httpContext = HttpContext.Current;

            if (httpContext != null)
            {
                key = string.Intern(key);
                lock (key) // locking on interned key
                {
                    var obj = httpContext.Cache[key];
                    if (obj == null)
                    {
                        obj = initializer((T)Convert.ChangeType(key, typeof(T)));
                        httpContext.Cache.Add(key, obj, null, absoluteExpiration, slidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
                    }
                    // taking care of value types
                    if (obj == null && (typeof(T)).IsValueType)
                    {
                        return default(T);
                    }
                    return (T)obj;
                }
            }
            else
            {
                return initializer((T)Convert.ChangeType(key, typeof(T))); // no available cache
            }
        }


        public static T GetCached<T>(string key, Func<T,T> initializer)
        {
            var httpContext = HttpContext.Current;
            TimeSpan slidingExpiration = DEFAULT_SLIDING_EXP;
            DateTime absoluteExpiration = DEFAULT_ABSOLUTE_EXP;
            if (httpContext != null)
            {
                key = string.Intern(key);
                lock (key) // locking on interned key
                {
                    var obj = httpContext.Cache[key];
                    if (obj == null)
                    {
                        obj = initializer((T)Convert.ChangeType(key, typeof(T)));
                        httpContext.Cache.Add(key, obj, null, absoluteExpiration, slidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
                    }
                    // taking care of value types
                    if (obj == null && (typeof(T)).IsValueType)
                    {
                        return default(T);
                    }
                    return (T)obj;
                }
            }
            else
            {
                return initializer((T)Convert.ChangeType(key, typeof(T))); // no available cache
            }
        }
    }
}