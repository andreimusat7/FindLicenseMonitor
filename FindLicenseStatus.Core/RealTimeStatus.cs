using FindLicenseStatus.DAL;
using FindLinceseStatus.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FindLicenseStatus.Core
{
    public class RealTimeStatus
    {
        public List<Status> RealTimeDatabase()
        {
            List<Status> st = new List<Status>();
            DataTable tb = new DataTable();
            Database db = new Database();
            tb = db.ReadDatabase();
            foreach (DataRow row in tb.Rows)
            {

                foreach (DataColumn column in tb.Columns)
                {
                    Status status = new Status();
                    status.MachineName = row["MachineName"].ToString();
                    status.MachineStatus = row["Status"].ToString();
                    status.StatusTime = (DateTime)row["StatusTime"];
                    status.UserName = row["UserName"].ToString();
                    status.VersionNumber = row["VersionNumber"].ToString();
                    st.Add(status);
//                    object item = row[column];
                    // read column and item
                }
            }
            return st;
        }


    }
}
