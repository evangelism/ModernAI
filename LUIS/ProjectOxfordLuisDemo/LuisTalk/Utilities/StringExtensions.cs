using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuisTalk.Utilities
{
    public static class StringExtensions
    {
        public static string StringJoin(this IEnumerable<string> stringList, string separator)
        {
            Check.Required<ArgumentNullException>(() => stringList != null);
            Check.Required<ArgumentNullException>(() => separator != null);

            return String.Join(separator, stringList);
        }
    }
}
