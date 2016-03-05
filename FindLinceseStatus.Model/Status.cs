using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindLinceseStatus.Model
{
    public class Status
    {
        public int Id { get; set; }
        public string MachineStatus { get; set; }
        public DateTime StatusTime { get; set; }
        public string MachineName { get; set; }
        public string UserName { get; set; }
        public string Location { get; set; }
        public string VersionNumber { get; set; }
    }
}
