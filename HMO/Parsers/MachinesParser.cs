using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HMO.Parsers
{
    public static class MachinesParser
    {
        static string pattern = @"embedded_board\(\s'(\w+)'\).";

        public static IEnumerable<string> Parse(string input)
        {
            var rv = new HashSet<string>();

            foreach (Match match in Regex.Matches(input, pattern))
            {
                rv.Add(match.Groups[1].Value);
            }

            return rv;
        }
    }
}
