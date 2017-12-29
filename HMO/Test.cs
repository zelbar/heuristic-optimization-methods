using System;
using System.Collections.Generic;
using System.Text;

namespace HMO
{
    public class Test
    {
        public string Name { get; set; }
        public int Duration { get; set; }
        public IEnumerable<string> MachinesItCanRunOn { get; set; }
        public IEnumerable<string> ResourcesRequired { get; set; }
    }
}
