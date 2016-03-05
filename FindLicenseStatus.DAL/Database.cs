using FindLicenseStatus.Core;
using FindLinceseStatus.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindLicenseStatus.DAL
{
    public class Database
    {

        public DataTable ReadDatabase()
        {
            try
            {
                DataTable dataTable = new DataTable("MachineStatus");
                using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["FindLicenseRelease"].ConnectionString))
                using (SqlCommand sqlCmd = new SqlCommand())
                using (SqlDataAdapter sqlAdap = new SqlDataAdapter(sqlCmd))
                {
                    if (sqlConn.State == ConnectionState.Closed)
                    {
                        sqlConn.Open();
                    }
                    sqlCmd.CommandText = "Select * From MachineStatus";
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlAdap.Fill(dataTable);
                    sqlConn.Close();
                    return dataTable;
                }
            }
            catch (Exception Ex)
            {
                //Logger log = new Logger();
                //log.logInfo(Ex.ToString());
                //MessageBox.Show(Ex.ToString());
                return null;
            }

        }

        public SortableBindingList<Reports> ReadReportDatabase(string machinename, string username, string startdate, string enddate, int groupby, bool clean)
        {

#if DEBUG
            string myConnectionString2 = ConfigurationManager.ConnectionStrings["FindLicenseConnectionString"].ConnectionString;
#else
            string myConnectionString2 = ConfigurationManager.ConnectionStrings["FindLicenseRelease"].ConnectionString;
#endif

            SortableBindingList<Reports> ListReports = new SortableBindingList<Reports>();
            SqlConnection conn2 = new SqlConnection(myConnectionString2);
            SqlCommand cmd2 = new SqlCommand();
            cmd2.Connection = conn2;
            string checkUser = "";
            string joinType = " LEFT JOIN ";
            try
            {
                conn2.Open();

                if (!string.IsNullOrEmpty(username))
                {
                    checkUser = "LoggedUser='" + username + "' AND ";
                }
                if( clean == true)
                    joinType = " INNER JOIN ";
                if (groupby == 3) // MONTH
                    cmd2.CommandText =
                                       " WITH Dates_CTE AS (SELECT  DATEADD(Month, DATEDIFF(Month, 0,CONVERT(DATETIME,'" + startdate + "',103)), 0) AS Dates UNION ALL " +
                                       " SELECT Dateadd(MM, 1, Dates)  FROM   Dates_CTE  WHERE  Dates < CONVERT(DATETIME,'" + enddate + "',103))  SELECT * FROM Dates_CTE AS T1 " +
                                       joinType + " (SELECT min(Id) as Rid, sum(NumberOfCars) as NumberOfCars_total, DATEADD(Month, DATEDIFF(Month, 0, min(TimeStampStart)), 0) as TimeStampStartMin, max(TimeStampEnd) as TimeStampEndMax, Count(*) as numrec, " +
                                       " sum(ScrollHits) as ScrollHits_total, " +
                                       " sum(FlIdleNr) as FlIdleNr_total, " +
                                       " sum(Zooms) as Zooms_total, " +
                                       " sum(Magnifiers) as Magnifiers_total, " +
                                       " sum(Negatives) as Negatives_total, " +
                                       " sum(Brightness) as Brightness_total, " +
                                       " sum(FindCars) as FindCars_total, " +
                                       " sum(ReadCars) as ReadCars_total " +
                                       " FROM FLActionsLog WHERE " + checkUser +
                                       " CONVERT(DATETIME,'" + startdate + "',103) <= TimeStampStart AND " +
                                       " CONVERT(DATETIME,'" + enddate + "',103) >= TimeStampEnd  " +
                                       " GROUP BY DATEPART(MONTH, TimeStampStart))  AS T2 ON T1.Dates = T2.TimeStampStartMin order by T1.Dates desc OPTION (MAXRECURSION 0)  ";

                if (groupby == 2) // WEEK
                    cmd2.CommandText =
                                   " WITH Dates_CTE AS (SELECT  DATEADD(Week, DATEDIFF(Week, 0,CONVERT(DATETIME,'" + startdate + "',103)), 0) AS Dates UNION ALL " +
                                   " SELECT Dateadd(WW, 1, Dates)  FROM   Dates_CTE  WHERE  Dates < CONVERT(DATETIME,'" + enddate + "',103))  SELECT * FROM Dates_CTE AS T1 " +
                                    joinType + " (SELECT min(Id) as Rid, sum(NumberOfCars) as NumberOfCars_total, DATEADD(Week, DATEDIFF(Week, 0, min(TimeStampStart)), 0) as TimeStampStartMin, max(TimeStampEnd) as TimeStampEndMax, Count(*) as numrec, " +
                                   " sum(ScrollHits) as ScrollHits_total, " +
                                   " sum(FlIdleNr) as FlIdleNr_total, " +
                                   " sum(Zooms) as Zooms_total, " +
                                   " sum(Magnifiers) as Magnifiers_total, " +
                                   " sum(Negatives) as Negatives_total, " +
                                   " sum(Brightness) as Brightness_total, " +
                                   " sum(FindCars) as FindCars_total, " +
                                   " sum(ReadCars) as ReadCars_total " +
                                   " FROM FLActionsLog WHERE " + checkUser +
                                   " CONVERT(DATETIME,'" + startdate + "',103) <= TimeStampStart AND " +
                                   " CONVERT(DATETIME,'" + enddate + "',103) >= TimeStampEnd  " +
                                   " GROUP BY DATEPART(WEEK, TimeStampStart)) AS T2 ON T1.Dates = T2.TimeStampStartMin  order by T1.Dates desc OPTION (MAXRECURSION 0) ";

                if (groupby == 1) //DAY
                    cmd2.CommandText = "  WITH Dates_CTE AS (SELECT  DATEADD(Day, DATEDIFF(Day, 0,CONVERT(DATETIME,'" + startdate + "',103)), 0) AS Dates UNION ALL " +
                                       " SELECT Dateadd(DD, 1, Dates) " +
                                       " FROM   Dates_CTE " +
                                       " WHERE  Dates < CONVERT(DATETIME,'" + enddate + "',103)) " +
                                       " SELECT * FROM Dates_CTE AS T1 " +
                                        joinType +  " (SELECT min(Id) as Rid, sum(NumberOfCars) as NumberOfCars_total, DATEADD(Day, DATEDIFF(Day, 0, min(TimeStampStart)), 0) AS TimeStampStartMin, max(TimeStampEnd) as TimeStampEndMax, Count(*) as numrec, " +
                                       " sum(ScrollHits) as ScrollHits_total, " +
                                       " sum(FlIdleNr) as FlIdleNr_total, " +
                                       " sum(Zooms) as Zooms_total, " +
                                       " sum(Magnifiers) as Magnifiers_total, " +
                                       " sum(Negatives) as Negatives_total, " +
                                       " sum(Brightness) as Brightness_total, " +
                                       " sum(FindCars) as FindCars_total, " +
                                       " sum(ReadCars) as ReadCars_total " +
                                       " FROM FLActionsLog WHERE " + checkUser +
                                       " CONVERT(DATETIME,'" + startdate + "',103) <= TimeStampStart AND " +
                                       " CONVERT(DATETIME,'" + enddate + "',103) >= TimeStampEnd  " +
                                       " GROUP BY DATEPART(DAY, TimeStampStart)) AS T2 ON T1.Dates = T2.TimeStampStartMin  order by T1.Dates desc OPTION (MAXRECURSION 0) ";

                if (groupby == 0) //HOUR

                    cmd2.CommandText = " WITH Dates_CTE AS (SELECT  DATEADD(Hour, DATEDIFF(Hour, 0,CONVERT(DATETIME,'" + startdate + "',103)), 0) AS Dates UNION ALL " +
                                       " SELECT Dateadd(HH, 1, Dates) " +
                                       " FROM   Dates_CTE " +
                                       " WHERE  Dates < CONVERT(DATETIME,'" + enddate + "',103)) " +                 
                                       " SELECT * FROM Dates_CTE AS T1 " +
                                       joinType + " ( SELECT min(Id) as Rid, sum(NumberOfCars) as NumberOfCars_total, DATEADD(Hour, DATEDIFF(Hour, 0, min(TimeStampStart)), 0) AS TimeStampStartMin, max(TimeStampEnd) as TimeStampEndMax, Count(*) as numrec, " +
                                       " sum(ScrollHits) as ScrollHits_total, " +
                                       " sum(FlIdleNr) as FlIdleNr_total, " +
                                       " sum(Zooms) as Zooms_total, " +
                                       " sum(Magnifiers) as Magnifiers_total, " +
                                       " sum(Negatives) as Negatives_total, " +
                                       " sum(Brightness) as Brightness_total, " +
                                       " sum(FindCars) as FindCars_total, " +
                                       " sum(ReadCars) as ReadCars_total " +
                                       " FROM FLActionsLog WHERE " + checkUser +
                                    
                                       " CONVERT(DATETIME,'" + startdate + "',103) <= TimeStampStart AND " +
                                       " CONVERT(DATETIME,'" + enddate + "',103) >= TimeStampEnd  " +
                                       " GROUP BY DATEPART(HOUR, TimeStampStart), DATEPART(DAY, TimeStampStart)) AS T2 ON T1.Dates = T2.TimeStampStartMin  order by T1.Dates desc  OPTION (MAXRECURSION 0) ";

                cmd2.ExecuteNonQuery();
                DateTime StartDate = DateTime.Parse(startdate);
                DateTime EndDate = DateTime.Parse(enddate);
                double days = (EndDate - StartDate).TotalDays;
                double weeks = (EndDate - StartDate).TotalDays/7;
             
                using (SqlDataReader rdr = cmd2.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Reports report = new Reports();

                        //if (!rdr.IsDBNull(rdr.GetOrdinal("myGroup")))
                        //{
                        //    int rec = rdr.GetInt32(rdr.GetOrdinal("myGroup"));
                        //    report.ReportId = rec;
                        //}

                        //if (!rdr.IsDBNull(rdr.GetOrdinal("LoggedUser")))
                        //{
                        //    report.User=  rdr.GetString(rdr.GetOrdinal("LoggedUser")).ToString();
                        //}

                        if (!rdr.IsDBNull(rdr.GetOrdinal("dates")))
                        {
                            report.StartTime = rdr.GetDateTime(rdr.GetOrdinal("dates")).ToString();
                        }

                        //if (!rdr.IsDBNull(rdr.GetOrdinal("TimeStampStartMin")))
                        //{
                        //    report.TimeStart = rdr.GetDateTime(rdr.GetOrdinal("TimeStampStartMin")).ToString();
                        //}
                        //if (!rdr.IsDBNull(rdr.GetOrdinal("TimeStampEndMax")))
                        //{
                        //    report.TimeEnd = rdr.GetDateTime(rdr.GetOrdinal("TimeStampEndMax")).ToString();
                        //}

                        if (!rdr.IsDBNull(rdr.GetOrdinal("NumberOfCars_total")))
                        {
                            int NumberOfCarsTotal = rdr.GetInt32(rdr.GetOrdinal("NumberOfCars_total"));
                            report.SavedCars = NumberOfCarsTotal;
                        }

                        if (!rdr.IsDBNull(rdr.GetOrdinal("ScrollHits_total")))
                        {
                            int ScroolHitsTotal = rdr.GetInt32(rdr.GetOrdinal("ScrollHits_total"));
                            report.ScroolHits = ScroolHitsTotal;
                        }

                        if (!rdr.IsDBNull(rdr.GetOrdinal("FlIdleNr_total")))
                        {
                            report.FlIdleNr = rdr.GetInt32(rdr.GetOrdinal("FlIdleNr_total"));
                        }

                        if (!rdr.IsDBNull(rdr.GetOrdinal("Zooms_total")))
                        {
                            report.Zooms = rdr.GetInt32(rdr.GetOrdinal("Zooms_total"));
                        }

                        if (!rdr.IsDBNull(rdr.GetOrdinal("Magnifiers_total")))
                        {
                            report.Magnifiers = rdr.GetInt32(rdr.GetOrdinal("Magnifiers_total"));
                        }
                        if (!rdr.IsDBNull(rdr.GetOrdinal("Negatives_total")))
                        {
                            report.Negatives = rdr.GetInt32(rdr.GetOrdinal("Negatives_total"));
                        }
                        if (!rdr.IsDBNull(rdr.GetOrdinal("Brightness_total")))
                        {
                            report.Brightness = rdr.GetInt32(rdr.GetOrdinal("Brightness_total"));
                        }

                        if (!rdr.IsDBNull(rdr.GetOrdinal("FindCars_total")))
                        {
                            report.FindCars = rdr.GetInt32(rdr.GetOrdinal("FindCars_total"));
                        }
                        if (!rdr.IsDBNull(rdr.GetOrdinal("ReadCars_total")))
                        {
                            report.ReadCars = rdr.GetInt32(rdr.GetOrdinal("ReadCars_total"));
                        }

                        ListReports.Add(report);
                    }
                }
                return ListReports;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            finally
            {
                conn2.Close();
                if (conn2 != null) conn2.Dispose();
                if (cmd2 != null) cmd2.Dispose();
            }
            
        }

        public List<MachineData> ReadFromDatabase()
        {
            int minustime = -1;
            List<MachineData> mdlist = new List<MachineData>();
            #region ReadFromDatabase
#if DEBUG
            string myConnectionString2 = ConfigurationManager.ConnectionStrings["FindLicenseConnectionString"].ConnectionString;
#else
            string myConnectionString2 = ConfigurationManager.ConnectionStrings["FindLicenseRelease"].ConnectionString;
#endif
            string result = string.Empty;
            SqlConnection conn2 = new SqlConnection(myConnectionString2);
            SqlCommand cmd2 = new SqlCommand();
            cmd2.Connection = conn2;
            try
            {
                conn2.Open();
                cmd2.CommandText = "SELECT Machine, sum(distinct NumberOfCars ) as NumberOfCars_total , " +
                                   "sum(distinct ScrollHits) as ScrollHits_total,  " +
                                   "sum(distinct FlIdleNr) as FlIdleNr_total,  " +
                                   "sum(distinct FlIdleLongNr) as FlIdleLongNr_total,  " +
                                   "sum( distinct Zooms) as Zooms_total,  " +
                                   "sum(distinct Magnifiers) as Magnifiers_total,  " +
                                   "sum(distinct Negatives) as Negatives_total,  " +
                                   "sum(distinct Brightness) as Brightness_total,  " +
                                   "sum(distinct FindCars) as FindCars_total,  " +
                                   "sum(distinct ReadCars) as ReadCars_total  " +
                                   "FROM FLActionsLog WHERE TimeStampStart >= " + "CONVERT(DATETIME,'" + DateTime.Now.AddHours(minustime).ToString("dd/MM/yyyy HH:mm:ss") + "',103) group by Machine";

                cmd2.ExecuteNonQuery();

                using (SqlDataReader rdr = cmd2.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        MachineData md = new MachineData();
                        md.MinusTime = minustime;
                        if (!rdr.IsDBNull(rdr.GetOrdinal("NumberOfCars_total")))
                        {
                            md.Machine = rdr.GetString(rdr.GetOrdinal("Machine")).ToString();
                        }
                        if (!rdr.IsDBNull(rdr.GetOrdinal("NumberOfCars_total")))
                        {
                            md.NumberOfCarsTotal = rdr.GetInt32(rdr.GetOrdinal("NumberOfCars_total"));
                        }

                        if (!rdr.IsDBNull(rdr.GetOrdinal("ScrollHits_total")))
                        {
                            md.ScroolHitsTotal = rdr.GetInt32(rdr.GetOrdinal("ScrollHits_total"));
                        }

                        if (!rdr.IsDBNull(rdr.GetOrdinal("FlIdleNr_total")))
                        {
                            md.FlIdleNrTotal = rdr.GetInt32(rdr.GetOrdinal("FlIdleNr_total"));
                        }

                        if (!rdr.IsDBNull(rdr.GetOrdinal("Zooms_total")))
                        {
                            md.ZoomsTotal = rdr.GetInt32(rdr.GetOrdinal("Zooms_total"));
                        }

                        if (!rdr.IsDBNull(rdr.GetOrdinal("Magnifiers_total")))
                        {
                            md.MagnifiersTotal = rdr.GetInt32(rdr.GetOrdinal("Magnifiers_total"));
                        }
                        if (!rdr.IsDBNull(rdr.GetOrdinal("Negatives_total")))
                        {
                            md.NegativesTotal = rdr.GetInt32(rdr.GetOrdinal("Negatives_total"));
                        }
                        if (!rdr.IsDBNull(rdr.GetOrdinal("Brightness_total")))
                        {
                            md.BrightnessTotal = rdr.GetInt32(rdr.GetOrdinal("Brightness_total"));
                        }

                        if (!rdr.IsDBNull(rdr.GetOrdinal("FindCars_total")))
                        {
                            md.FindCarsTotal = rdr.GetInt32(rdr.GetOrdinal("FindCars_total"));
                        }
                        if (!rdr.IsDBNull(rdr.GetOrdinal("ReadCars_total")))
                        {
                            md.ReadCarsTotal = rdr.GetInt32(rdr.GetOrdinal("ReadCars_total"));
                        }

                        mdlist.Add(md);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {

                conn2.Close();
                if (conn2 != null) conn2.Dispose();
                if (cmd2 != null) cmd2.Dispose();

            }
            return mdlist;
            #endregion
        }

        public List<string> GetMachines()
        {
            List<string> machineList = new List<string>();
            #region ReadFromDatabase
#if DEBUG
            string myConnectionString2 = ConfigurationManager.ConnectionStrings["FindLicenseConnectionString"].ConnectionString;
#else
            string myConnectionString2 = ConfigurationManager.ConnectionStrings["FindLicenseRelease"].ConnectionString;
#endif
            string result = string.Empty;
            SqlConnection conn2 = new SqlConnection(myConnectionString2);
            SqlCommand cmd2 = new SqlCommand();
            cmd2.Connection = conn2;
            try
            {
                conn2.Open();
                cmd2.CommandText = "SELECT distinct Machine FROM FLActionsLog";
                cmd2.ExecuteNonQuery();

                using (SqlDataReader rdr = cmd2.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var machine = string.Empty;
                        if (!rdr.IsDBNull(rdr.GetOrdinal("Machine")))
                        {
                            machine = rdr.GetString(rdr.GetOrdinal("Machine")).ToString();
                        }

                        machineList.Add(machine);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                conn2.Close();
                if (conn2 != null) conn2.Dispose();
                if (cmd2 != null) cmd2.Dispose();
            }
            return machineList;
            #endregion
        }

        public List<string> GetUsers()
        {
            List<string> userList = new List<string>();
            #region ReadFromDatabase
#if DEBUG
            string myConnectionString2 = ConfigurationManager.ConnectionStrings["FindLicenseConnectionString"].ConnectionString;
#else
            string myConnectionString2 = ConfigurationManager.ConnectionStrings["FindLicenseRelease"].ConnectionString;
#endif
            string result = string.Empty;
            SqlConnection conn2 = new SqlConnection(myConnectionString2);
            SqlCommand cmd2 = new SqlCommand();
            cmd2.Connection = conn2;
            try
            {
                conn2.Open();
                cmd2.CommandText = "SELECT distinct LoggedUser FROM FLActionsLog";
                cmd2.ExecuteNonQuery();

                using (SqlDataReader rdr = cmd2.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var usr = string.Empty;
                        if (!rdr.IsDBNull(rdr.GetOrdinal("LoggedUser")))
                        {
                            usr = rdr.GetString(rdr.GetOrdinal("LoggedUser")).ToString();
                        }

                        userList.Add(usr);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                conn2.Close();
                if (conn2 != null) conn2.Dispose();
                if (cmd2 != null) cmd2.Dispose();
            }
            return userList;
            #endregion
        }

        public string GetMaxDateFound(string Status) {
              #region ReadFromDatabase
#if DEBUG
            string myConnectionString2 = ConfigurationManager.ConnectionStrings["FindLicenseConnectionString"].ConnectionString;
#else
            string myConnectionString2 = ConfigurationManager.ConnectionStrings["FindLicenseRelease"].ConnectionString;
#endif
            string result = string.Empty;
            SqlConnection conn2 = new SqlConnection(myConnectionString2);
            SqlCommand cmd2 = new SqlCommand();
            cmd2.Connection = conn2;
            try
            {
                conn2.Open();
                if(Status == "FIND")
                    cmd2.CommandText = " SELECT max(VEPDate) as maxFoundTime FROM MachineStatus WHERE Status = 'FIND' ";
                else if(Status == "READ")
                    cmd2.CommandText = "SELECT max(VEPDateRead) as maxFoundTime FROM MachineStatus WHERE Status = 'READ' ";

                cmd2.ExecuteNonQuery();

                using (SqlDataReader rdr = cmd2.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (!rdr.IsDBNull(rdr.GetOrdinal("maxFoundTime")))
                        {
                            result = rdr.GetDateTime(rdr.GetOrdinal("maxFoundTime")).ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                conn2.Close();
                if (conn2 != null) conn2.Dispose();
                if (cmd2 != null) cmd2.Dispose();
            }
            if (result == SqlDateTime.MinValue.ToString())
                result = "";
            return result;
            #endregion

        }


    }
}
