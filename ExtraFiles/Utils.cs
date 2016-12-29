using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyBot
{
    public static class Utils
    {
        public static string NextTo(this string[] str, string pat)
        {
            for (int i = 0; i < str.Length - 1; i++)
            {
                if (str[i] == pat) return str[i + 1];
            }
            return "";
        }

    }
}