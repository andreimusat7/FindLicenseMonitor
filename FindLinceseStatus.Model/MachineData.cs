using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindLinceseStatus.Model
{
    public class MachineData
    {
        public string Machine { get; set; }
        public int NumberOfCarsTotal { get; set; }
        public int ScroolHitsTotal { get; set; }
        public int FlIdleNrTotal { get; set; }
        public int ZoomsTotal { get; set; }
        public int MagnifiersTotal { get; set; }
        public int NegativesTotal { get; set; }
        public int BrightnessTotal { get; set; }
        public int FindCarsTotal { get; set; }
        public int ReadCarsTotal { get; set; }
        public int MinusTime { get; set; }
    }
}
