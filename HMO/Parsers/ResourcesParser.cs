using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HMO.Parsers
{
    public static class ResourcesParser
    {
        static string pattern = @"esource\(\s'(\w+)',\s(\d+)\).";

        public static Dictionary<string, int> Parse(string input)
        {
            var rv = new Dictionary<string, int>();

            foreach (Match match in Regex.Matches(input, pattern))
            {
                rv[match.Groups[1].ToString()] = int
                    .Parse(match.Groups[2].Value);
            }

            return rv;
        }
    }
}
