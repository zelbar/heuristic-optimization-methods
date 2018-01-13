using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HMO.Schedulers
{
    public class GreedyScheduler1 : IScheduler
    {
        private readonly Dictionary<string, Test> _tests;
        private readonly HashSet<string> _machines;
        private readonly Dictionary<string, int> _resources;
        private readonly Random _random;
        private List<Test> _unscheduled;
        private Dictionary<string, string[]> _scheduleTable;
        private Dictionary<string, int[]> _resourcesTable;
        private int _totalTime;
        private int _maxDuration;

        public int MaxDuration => _maxDuration;

        public int TotalTime => _totalTime + 1;

        public GreedyScheduler1(Test[] tests, string[] machines, Dictionary<string, int> resources)
        {
            _random = new Random();
            _tests = new Dictionary<string, Test>();

            foreach (var test in tests)
            {
                _tests[test.Name] = test;
            }

            _machines = new HashSet<string>(machines);
            _resources = resources;

            _maxDuration = _tests.Values.Sum(x => x.Duration);
            _scheduleTable = new Dictionary<string, string[]>(_machines.Count);
            _resourcesTable = new Dictionary<string, int[]>(_resources.Count);
        }

        private void prepareDataStructures()
        {
            _scheduleTable.Clear();

            foreach (string m in _machines)
            {
                _scheduleTable[m] = new string[MaxDuration];
            }

            _resourcesTable.Clear();

            foreach (var r in _resources)
            {
                _resourcesTable[r.Key] = Enumerable
                    .Repeat(r.Value, MaxDuration).ToArray();
            }

            _unscheduled = _tests.Values.OrderBy(x => _random.Next()).ToList();

        }

        public IEnumerable<string> Schedule()
        {
            prepareDataStructures();
            int testsScheduled = 0;

            // Schedule tests that can run on only one machine
            Test test;
            while ((test = _unscheduled.Where(x => x.MachinesItCanRunOn.Count() == 1).FirstOrDefault()) != null)
            {
                scheduleTestOnRandomAvailableMachine(test);
                ++testsScheduled;
            }

            // Schedule all other tests
            while (_unscheduled.Count() > 0)
            {
                test = _unscheduled.First();
                scheduleTestOnRandomAvailableMachine(test);
                ++testsScheduled;
            }

            // Console.WriteLine("Total {0} tests scheduled", testsScheduled);

            // Calculate total time of schedule
            _totalTime = 0;

            foreach (var m in _machines)
            {
                for (int t = _totalTime; t < _scheduleTable[m].Length; ++t)
                {
                    if (_scheduleTable[m][t] != null && t > _totalTime)
                        _totalTime = t;
                }
            }

            return serializeTimeTable();
        }

        private IEnumerable<string> serializeTimeTable()
        {
            var rv = new List<Tuple<Test, int, string>>();

            foreach (var mt in _scheduleTable)
            {
                for (int i = 0; i < mt.Value.Length; ++i)
                {
                    var testName = _scheduleTable[mt.Key][i];

                    if (testName == null)
                        continue;

                    var test = _tests[testName];
                    rv.Add(new Tuple<Test, int, string>(test, i, mt.Key));

                    while (_scheduleTable[mt.Key][i] == testName)
                        ++i;
                    --i;
                }
            }

            if (rv.Count != _tests.Count)
            {
                var diffInSchedule = _tests.Keys.ToList().Except<string>(_scheduleTable.SelectMany(x => x.Value));
                var diffInOutput = _tests.Keys.ToList().Except<string>(rv.Select(x => x.Item1.Name));

                foreach (var x in diffInSchedule)
                {
                    Console.WriteLine("No test " + x + " in schedule");
                }
                foreach (var x in diffInOutput)
                {
                    Console.WriteLine("No test " + x + " in output");
                }

                throw new Exception("Some tests are lost :/");
            }

            return rv.Select(x => "\'" + x.Item1.Name + "\'," + x.Item2 + ",\'" + x.Item3 + "\'.").ToList();
        }

        private void scheduleTestOnRandomAvailableMachine(Test test)
        {
            bool success = false;
            var machines = test.MachinesItCanRunOn.Count() == 0 
                ? _machines : test.MachinesItCanRunOn;
            
            foreach (var m in machines.OrderBy(x => _random.Next()))
            {
                int startIdx = findFirstFeasableTimeIdx(test, m);

                if (startIdx != -1)
                {
                    for (int t = startIdx; t < startIdx + test.Duration; ++t)
                    {
                        _scheduleTable[m][t] = test.Name;

                        foreach (var r in test.ResourcesRequired)
                        {
                            if (_resourcesTable[r][t] <= 0)
                                throw new Exception("FAIL");

                            _resourcesTable[r][t] -= 1;
                        }
                    }

                    success = true;
                    _unscheduled.Remove(test);
                    break;
                }
                else
                {
                    throw new Exception("IDK");
                }
            }

            if (!success) throw new Exception("Impossible");
        }

        private int findFirstFeasableTimeIdx(Test test, string machine)
        {
            for (int t = 0; t < MaxDuration - test.Duration - 1; ++t)
            {
                if (timeIdxFeasableForTestAndMachine(test, machine, t))
                    return t;
            }

            return -1;
        }

        private bool timeIdxFeasableForTestAndMachine(Test test, string m, int t)
        {
            for (int t2 = t; t2 < t + test.Duration; ++t2)
            {
                if (_scheduleTable[m][t2] != null)
                {
                    return false;
                }

                foreach (var r in test.ResourcesRequired)
                {
                    if (_resourcesTable[r][t2] <= 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
