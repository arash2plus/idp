using System.Collections.Generic;
using System.Linq;

namespace Gaia.IdP.Infrastructure.Helpers
{
    public static class StringExtensions
    {
        public static string GetString(this IEnumerable<string> query, string seperator = ", ")
        {
            if (query.Count() == 0) return null;

            return string.Join(seperator, query);
        }

        public static string GetString(this IQueryable<string> query, string seperator = ", ")
        {
            if (!query.Any()) return null;

            return string.Join(seperator, query.ToArray());
        }
    }
}