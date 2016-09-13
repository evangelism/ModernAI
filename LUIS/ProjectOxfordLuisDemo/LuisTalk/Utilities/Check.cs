using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LuisTalk.Utilities
{
    public static class Check
    {
        public static void Required<T>(Func<bool> predicate, string format, params object[] args)
            where T : Exception
        {
            if (!predicate.Invoke())
            {
                var message = String.Format(format, args);
                var constructor = typeof(T).GetConstructor(new[] { typeof(string) });
                if (constructor != null)
                    throw constructor.Invoke(new[] { message }) as Exception;

                throw new ArgumentOutOfRangeException("T should be an Exception deriviate with new(string) constructor");
            }
        }

        public static void Required<T>(Expression<Func<bool>> predicate)
            where T : Exception
        {
            Required<T>(predicate.Compile(), predicate.Body.Reduce().ToString());
        }
    }
}
