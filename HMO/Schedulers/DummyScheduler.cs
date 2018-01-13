using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HMO.Schedulers
{
    /// <summary>
    /// Creates a dummy schedule where none of the tests is scheduled at the same time.
    /// Used to test if all inputs are parsed correctly as it should produce 
    /// a formally valid, feasable solution, despite its uselessness.
    /// </summary>
    public class DummyScheduler : IScheduler
    {
        private readonly Test[] _tests;
        private readonly string[] _machines;
        private readonly Dictionary<string, int> _resources;
        private List<ScheduledTest> _schedule;
        private int _totalTime;

        public DummyScheduler(Test[] tests, string[] machines, Dictionary<string, int> resources)
        {
            _tests = tests;
            _machines = machines;
            _resources = resources;

            _schedule = new List<ScheduledTest>();
        }

        public int MaxDuration => _tests.Sum(x => x.Duration);

        public int TotalTime => _totalTime;

        public IEnumerable<string> Schedule()
        {
            _totalTime = 0;

            foreach (var test in _tests)
            {
                var st = new ScheduledTest()
                {
                    Test = test,
                    StartTime = _totalTime,
                    Machine = test.MachinesItCanRunOn.Count() > 0 ?
                        test.MachinesItCanRunOn.First() : _machines.First()
                };
                _totalTime += test.Duration;
                _schedule.Add(st);
            }

            return _schedule
                .Select(x => "\'" + x.Test.Name + "\'," + x.StartTime + ",\'" + x.Machine + "\'.")
                .ToList();
        }
    }
}
