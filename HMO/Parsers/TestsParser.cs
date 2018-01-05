using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace HMO.Parsers
{
    public static class TestsParser
    {
        static string pattern = @"test\(\s'(\w+)', (\d+), \[(.*)\], \[(.*)\]\).";

        public static IEnumerable<Test> Parse(string input)
        {
            var rv = new List<Test>();

            foreach(Match match in Regex.Matches(input, pattern))
            {
                try
                {
                    var name = match.Groups[1].Value;
                    var duration = int.Parse(match.Groups[2].Value);
                    var machines = match.Groups[3].Value
                        .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Replace("\'", ""));
                    var resources = match.Groups[4].Value
                        .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Replace("\'", ""));

                    rv.Add(new Test()
                    {
                        Name = name,
                        Duration = duration,
                        MachinesItCanRunOn = machines,
                        ResourcesRequired = resources
                    });
                }
                catch (FormatException fmtex)
                {
                    Console.Error.WriteLine("Invalid duration format: " + fmtex);
                }
                catch(Exception ex)
                {
                    Console.Error.WriteLine("Parsing exception: " + ex);
                }
            }

            return rv;
        }
    }
}
