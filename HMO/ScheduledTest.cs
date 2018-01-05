using System;
using System.Collections.Generic;
using System.Text;

namespace HMO
{
    public class ScheduledTest
    {
        public Test Test { get; set; }
        public int StartTime { get; set; }
        public int EndTime => StartTime + Test.Duration;
        public string Machine { get; set; }
    }
}
