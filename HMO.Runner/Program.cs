using System;
using System.IO;
using System.Linq;
using HMO.Parsers;
using HMO.Schedulers;
using System.Collections.Generic;
using System.Diagnostics;

namespace HMO.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1 && args.Length > 2)
            {
                Console.Error.WriteLine("Supply program arguments: path-to-input-file optional-time-constraint-to-run-in-minutes");
                return;
            }

            var input = File.ReadAllText(args[0]);

            Console.WriteLine("Read {0} lines from file {1}", input.Count(), args[0]);

            var tests = TestsParser.Parse(input);
            var machines = MachinesParser.Parse(input);
            var resources = ResourcesParser.Parse(input);

            IScheduler scheduler = new GreedyScheduler1(tests.ToArray(), machines.ToArray(), resources);

            int? timeToRunInMins = null;

            if (args.Length == 2)
                timeToRunInMins = int.Parse(args[1]);

            int shortestTime = int.MaxValue;
            var sw = new Stopwatch();
            int i = 1;
            sw.Start();

            for (; timeToRunInMins != null && sw.Elapsed.Minutes < timeToRunInMins || timeToRunInMins == null; ++i)
            {
                var result = scheduler.Schedule();
                var time = scheduler.TotalTime;

                if (time < shortestTime)
                {
                    Console.WriteLine("#" + i + " " + sw.Elapsed + ": Found solution with time {0}", time);
                    shortestTime = time;

                    var outputFileName = "solutions/res-" + ((args.Length == 2) ? args[1] : "ne") + "m-" + args[0].Split("/")[1];
                    File.WriteAllLines(outputFileName, result);
                    Console.WriteLine("{0} lines written to file {1}", result.Count(), outputFileName);
                }


            }

            sw.Stop();
            Console.WriteLine("Total iterations made: {0}", i);
        }
    }
}
