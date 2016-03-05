using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindLinceseStatus.Model
{
    public class Reports
    {
        //public int NumRecords { get; set; }

        public string StartTime { get; set; }

        //public string TimeStart { get; set; }

        //public string TimeEnd { get; set; }

        //public string User { get; set; }
        public double FindCars { get; set; }

        public double ReadCars { get; set; }

        public double SavedCars { get; set; }

        public double ScroolHits { get; set; }

        public double FlIdleNr { get; set; }

        public double Zooms { get; set; }

        public double Magnifiers { get; set; }

        public double Negatives { get; set; }

        public double Brightness { get; set; }

    }
}
