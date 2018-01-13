using System;
using System.Collections.Generic;
using System.Text;

namespace HMO
{
    public interface IScheduler
    {
        IEnumerable<string> Schedule();
        int TotalTime { get; }
    }
}
