using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMaker.PatternProcessing
{
    public class TimerData
    {
        public TimerData(int tick, bool ready = false)
        {
            Tick = tick;
            Ready = ready;
        }

        public int Tick { get; private set; }

        public bool Ready { get; private set; }
    }
}
