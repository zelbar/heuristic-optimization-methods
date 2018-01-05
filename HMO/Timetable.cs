using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HMO
{
    public class Timetable
    {
        private readonly Test[] _tests;
        private readonly string[] _machines;
        private readonly Dictionary<string, int> _resources;
        private List<ScheduledTest> _schedule;

        public Timetable(Test[] tests, string[] machines, Dictionary<string, int> resources)
        {
            _tests = tests;
            _machines = machines;
            _resources = resources;

            _schedule = new List<ScheduledTest>();
        }

        public IEnumerable<string> Calculate()
        {

            int globalTime = 0;

            foreach (var test in _tests)
            {
                var st = new ScheduledTest()
                {
                    Test = test,
                    StartTime = globalTime,
                    Machine = test.MachinesItCanRunOn.Count() > 0 ?
                        test.MachinesItCanRunOn.First() : _machines.First()
                };
                globalTime += test.Duration;
                _schedule.Add(st);
            }

            return _schedule
                .Select(x => "\'" + x.Test.Name + "\'," + x.StartTime + ",\'" + x.Machine + "\'.")
                .ToList();
        }

        public int MaxDuration => _tests.Sum(x => x.Duration);
        
    }
}
