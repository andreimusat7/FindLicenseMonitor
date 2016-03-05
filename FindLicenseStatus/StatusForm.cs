using FindLicenseStatus.Core;
using FindLicenseStatus.DAL;
using FindLicenseStatus.Resources;
using FindLinceseStatus.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace FindLicenseStatus
{
    public partial class StatusForm : Form
    {

        private System.Windows.Forms.ToolTip ToolTip1;
        System.Drawing.Color ColorOff;
        System.Drawing.Color ColorInactive;
        System.Drawing.Color ColorRead;
        System.Drawing.Color ColorFind;
        System.Drawing.Color ColorOtherProg ;
        System.Drawing.Color ColorClosed;
    
        Database db = new Database();
        List<MachineData> mdlist = new List<MachineData>();
        public StatusForm()
        {
            InitializeComponent();
            Load += new EventHandler(StatusForm_Load);

            ToolTip1 = new System.Windows.Forms.ToolTip();
            
            ToolTip1.InitialDelay = 5;
            ToolTip1.ReshowDelay = 5;
            ReadFromResources();
            GetMaxDateFound();
            SetTheColorsFromXML();
            

        }

        private void SetTheColorsFromXML()
        {
            var maction = string.Empty;
            var mcolor = string.Empty;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                string XMLPath = ConfigurationManager.AppSettings["XMLPath"];
                if (!string.IsNullOrEmpty(XMLPath))
                    xmlDoc.Load(XMLPath);
                else
                    return;
                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/controls/colors/setcolor");

                // Read particular values form each node
                foreach (XmlNode node in nodeList)
                {
                    maction = node.Attributes["Action"].Value;
                    mcolor = node.Attributes["Color"].Value;

                    //string switchcolor = ConfigurationManager.AppSettings[mcolor];
                    switch (maction)
                    {
                        case ("OFF"):
                            ColorOff = System.Drawing.ColorTranslator.FromHtml(mcolor);
                            break;
                        case ("INACTIVE"):
                            ColorInactive = System.Drawing.ColorTranslator.FromHtml(mcolor);
                            break;
                        case ("READ"):
                            ColorRead = System.Drawing.ColorTranslator.FromHtml(mcolor);
                            break;
                        case ("FIND"):
                            ColorFind = System.Drawing.ColorTranslator.FromHtml(mcolor);
                            break;
                        case ("OTHERPROG"):
                            ColorOtherProg = System.Drawing.ColorTranslator.FromHtml(mcolor);
                            break;
                        case ("CLOSED"):
                            ColorClosed = System.Drawing.ColorTranslator.FromHtml(mcolor);
                            break;
                    }

                }
            }
            catch (Exception Ex) { Console.WriteLine(Ex.ToString()); }
        }

        private void GetMaxDateFound()
        {
            string readtime = db.GetMaxDateFound("READ");
            string findtime = db.GetMaxDateFound("FIND");
            if (readtime != "")
                labelReadTime.Text = readtime;
            if (findtime != "")
                labelFindTime.Text = findtime;
        }

        private void ReadFromResources()
        {
            string currentResource = ConfigurationManager.AppSettings["Culture"];
            if (!string.IsNullOrEmpty(currentResource))
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(currentResource);
            groupBoxOff2.Text = Strings.StatusForm_off;
            groupBoxRead2.Text = Strings.StatusForm_read;
            groupBoxFind2.Text = Strings.StatusForm_find;
            groupBoxOtherProg2.Text = Strings.StatusForm_otherprog;
            groupBoxVepClosed2.Text = Strings.StatusForm_vepclosed;
            groupBoxInactive2.Text = Strings.StatusForm_inactive;

            groupBoxOff.Text = Strings.StatusForm_off;
            groupBoxRead.Text = Strings.StatusForm_read;
            groupBoxFind.Text = Strings.StatusForm_find;
            groupBoxOtherProg.Text = Strings.StatusForm_otherprog;
            groupBoxVepClosed.Text = Strings.StatusForm_vepclosed;
            groupBoxInactive.Text = Strings.StatusForm_inactive;
            label17.Text = Strings.StatusForm_PT_Total;
            label19.Text = Strings.StatusForm_PT;
            label18.Text = Strings.StatusForm_Nachshonim;
            labelTitle.Text = Strings.StatusForm_Nachshonim_total;
        }

        private void StatusForm_Load(object sender, EventArgs e)
        {
            UpdateStatus();
            System.Windows.Forms.Timer CheckIfClosedTimer = new System.Windows.Forms.Timer();
            CheckIfClosedTimer.Interval = 100000;
            CheckIfClosedTimer.Tick += new EventHandler(CheckIfClosedTimer_Tick);
            CheckIfClosedTimer.Start();

            //System.Windows.Forms Timer
            System.Windows.Forms.Timer MyTimer = new System.Windows.Forms.Timer();
            MyTimer.Interval = 5000; 
            MyTimer.Tick += new EventHandler(StatusForm_Tick);
            MyTimer.Start();

            //System.Windows.Forms.Timer InactiveColorChange = new System.Windows.Forms.Timer();
            //InactiveColorChange.Interval = 1000;
            //InactiveColorChange.Tick += new EventHandler(InactiveColorChange_Tick);
            //InactiveColorChange.Start();

            // Create a timer that calls a procedure every 2 seconds.
            // Note: There is no Start method; the timer starts running as soon as 
             //the instance is created.
            StateObjClass StateObj = new StateObjClass();
            StateObj.TimerCanceled = false;
            StateObj.SomeValue = 1;
            System.Threading.TimerCallback TimerDelegate = new System.Threading.TimerCallback(TimerTask);
            System.Threading.Timer TimerItem = new System.Threading.Timer(TimerDelegate, StateObj, 1000, 500);

            // Save a reference for Dispose.
            StateObj.TimerReference = TimerItem;  
         

            ReadLocationXML();

            mdlist.Clear();
            mdlist = db.ReadFromDatabase();

            
            //ToolTip1.Popup += new PopupEventHandler(ToolTip1_Popup);
            foreach (Control c in this.Controls)
            {
                if (c is GroupBox && c.Text != Strings.StatusForm_read && c.Text != Strings.StatusForm_find && c.Text != Strings.StatusForm_otherprog && c.Text != Strings.StatusForm_inactive && c.Text != Strings.StatusForm_vepclosed && c.Text != Strings.StatusForm_off)
                {
                    foreach (Control z in c.Controls)
                    {
                        if (z.TabIndex == 4)
                        {
                            //ToolTip1.SetToolTip(z, "Loading data...");
                            z.MouseEnter -= new EventHandler(this.panel1_MouseHover);
                            z.MouseEnter += new EventHandler(this.panel1_MouseHover);
                            z.Click -= new EventHandler(control_click);
                            z.Click += new EventHandler(control_click);
                        }
                    }
                }
                if (c is GroupBox && (c.Text == Strings.StatusForm_read || c.Text == Strings.StatusForm_find || c.Text == Strings.StatusForm_otherprog || c.Text == Strings.StatusForm_inactive || c.Text == Strings.StatusForm_vepclosed || c.Text == Strings.StatusForm_off))
                {
                    foreach (Control z in c.Controls)
                    {
                        if (z.TabIndex == 4)
                        {
                            z.Click -= new EventHandler(control_click2);
                            z.Click += new EventHandler(control_click2);
                        }
                    }
                }
            }
           
        }

        private void TimerTask(object state)
        {
            foreach (Control c in this.Controls)
            {
                if (c is GroupBox)
                {
                    //Iterate through groupbox controls
                    foreach (Control z in c.Controls)
                    {
                        if (z is Label)
                        {
                            if (z.TabIndex == 1)
                            {
                                if (z.Text == "INACTIVE")
                                {
                                    if (z.BackColor == ColorInactive)
                                        z.BackColor = Color.Black;
                                    else
                                        z.BackColor = ColorInactive;

                                }
                            }
                        }
                    }
                }
            }
        }

        private void panel1_MouseHover(object sender, EventArgs e)
        {
            string machinename = ((CtrlTransparent)sender).Parent.Text;

          
            StringBuilder messageBoxCS = new StringBuilder();
            ToolTip1.OwnerDraw = true;
            ToolTip1.Draw += toolTip1_Draw;
            for (var i = 0; i <= mdlist.Count - 1; i++)
            {
                if (mdlist[i].Machine == machinename)
                {
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_machine, mdlist[i].Machine);
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1} {2}", Strings.StatusForm_calculationtime, mdlist[i].MinusTime , "H");
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_totalcarssaved, mdlist[i].NumberOfCarsTotal);
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_readstotal, mdlist[i].ReadCarsTotal);
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_findstotal, mdlist[i].FindCarsTotal);
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_scroolhitstotal, mdlist[i].ScroolHitsTotal);
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_brightnesstotal, mdlist[i].BrightnessTotal);
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_magnifierstotal, mdlist[i].MagnifiersTotal);
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_negativestotal, mdlist[i].NegativesTotal);
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_zoomstotal, mdlist[i].ZoomsTotal);
                    messageBoxCS.AppendLine();
                    ToolTip1.SetToolTip(((CtrlTransparent)sender), messageBoxCS.ToString());
                }
            }
            if (messageBoxCS.Length == 0)
            {
                messageBoxCS.AppendFormat("{0}: {1}", "No data", "There is no data");
                messageBoxCS.AppendLine();
                
                ToolTip1.SetToolTip(((CtrlTransparent)sender), messageBoxCS.ToString());
            }

        }

        private void toolTip1_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.Graphics.FillRectangle(SystemBrushes.Info, e.Bounds);
            e.DrawBorder();
            e.DrawText(TextFormatFlags.RightToLeft | TextFormatFlags.Right);
        }

        private void InactiveColorChange_Tick(object sender, EventArgs e)
        {
            foreach (Control c in this.Controls)
            {
                if (c is GroupBox)
                {
                    Color initColor = c.BackColor;
                    //Iterate through groupbox controls
                    foreach (Control z in c.Controls)
                    {
                        if (z is Label)
                        {
                            
                            if (z.TabIndex == 1)
                            {
                                if (z.Text == "INACTIVE")
                                {
                                    if (z.BackColor == ColorInactive)
                                        z.BackColor = Color.Black;
                                    else
                                        z.BackColor = ColorInactive;

                                }
                            }
                            else {
                                z.BackColor = initColor;
                            }
                        }
                    }
                }
            }
        }

        private void control_click2(object sender, EventArgs e)
        {
            string machinename = string.Empty;
            string user = string.Empty;
            Report form2 = new Report(machinename, user);
            //Update form information
            form2.Show();
        }

        private void control_click(object sender, EventArgs e)
        {
            string machinename = ((CtrlTransparent)sender).Parent.Text;
           // MessageBox.Show(machinename, "Popup Event");
            //this.Hide();
            string user = string.Empty;
            foreach (Control c in this.Controls)
            {
                if (c is GroupBox)
                {
                    if (c.Text == machinename) // Iterate through databse returned list
                    {
                        //Iterate through groupbox controls
                        foreach (Control z in c.Controls)
                        {
                            if (z is Label)
                            {
                                if (z.TabIndex == 2)
                                {
                                    user = z.Text;
                                }
                            }
                        }
                    }
                }
            }
            
            Report form2 = new Report(machinename, user);
            //Update form information
            form2.Show();
        }

        private void ToolTip1_Popup(Object sender, PopupEventArgs e)
        {
            string machinename = e.AssociatedControl.Parent.Text;

            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            StringBuilder messageBoxCS = new StringBuilder();
            foreach (var item in mdlist)
            {
                if (item.Machine == machinename)
                {
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_machine, item.Machine);
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_calculationtime, item.MinusTime + "H");
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_totalcarssaved, item.NumberOfCarsTotal);
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_readstotal, item.ReadCarsTotal);
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_findstotal, item.FindCarsTotal);
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_scroolhitstotal, item.ScroolHitsTotal);
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1}",Strings.StatusForm_brightnesstotal, item.BrightnessTotal);
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_magnifierstotal, item.MagnifiersTotal);
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_negativestotal, item.NegativesTotal);
                    messageBoxCS.AppendLine();
                    messageBoxCS.AppendFormat("{0}: {1}", Strings.StatusForm_zoomstotal, item.ZoomsTotal);
                    messageBoxCS.AppendLine();
                    ToolTip1.SetToolTip(e.AssociatedControl, messageBoxCS.ToString());
                }
            }
            if (messageBoxCS.Length == 0)
            {
                messageBoxCS.AppendFormat("{0}: {1}", "No data", "There is no data");
                messageBoxCS.AppendLine();
                ToolTip1.SetToolTip(e.AssociatedControl, messageBoxCS.ToString());
            }
        }

        private void CheckIfClosedTimer_Tick(object sender, EventArgs e)
        {
            CheckIfClosedMachine();//Check if the machine is in CLOSED VEP State
            ReadFLLog();// Recheck every machine action totals
        }

        private void ReadFLLog()
        {
            mdlist.Clear();
            mdlist = db.ReadFromDatabase();
        }

        private void CheckIfClosedMachine()
        {
            DataTable dt = new DataTable();
            Database db = new Database();

            dt = db.ReadDatabase();
            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"].ToString() == "VEP CLOSED" )
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(row["ipaddress"].ToString()))
                        {
                            Ping myPing = new Ping();
                            PingReply reply = myPing.Send(row["ipaddress"].ToString(), 1000);
                            if (reply.Status.ToString() == "TimedOut")
                            {
                                try
                                {
                                    using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["FindLicenseRelease"].ConnectionString))
                                    using (SqlCommand sqlCmd = new SqlCommand())
                                    using (SqlDataAdapter sqlAdap = new SqlDataAdapter(sqlCmd))
                                    {
                                        if (sqlConn.State == ConnectionState.Closed)
                                        {
                                            sqlConn.Open();
                                        }
                                        sqlCmd.CommandText = "UPDATE MachineStatus SET Status='OFF' WHERE MachineName = '" + row["MachineName"].ToString() + "'";
                                        sqlCmd.Connection = sqlConn;
                                        sqlCmd.CommandType = CommandType.Text;
                                        sqlCmd.ExecuteNonQuery();
                                        sqlConn.Close();

                                    }
                                }
                                catch (Exception Ex) {
                                    Console.WriteLine(Ex.ToString());
                                }
                               
                            }
                        }
                    }
                    catch(Exception Ex)
                    {
                        Console.WriteLine("ERROR: You have Some TIMEOUT issue " + Ex.ToString());
                    }
                }
            }
        }

        private void StatusForm_Tick(object sender, EventArgs e)
        {
            UpdateStatus();
            GetMaxDateFound();
        }

        private void UpdateStatus() {
            string[] AdminMachines = ConfigurationManager.AppSettings["AdminMachines"].Split(';');
            DataTable dt = new DataTable();
            Database db = new Database();
            List<Status> st = new List<Status>();
            st.Clear();
            dt = db.ReadDatabase();

            foreach (DataRow row in dt.Rows)
            {
                //If machine is in Admin list don't count them or read them
                var isAdmin = false;
                for (var i = 0; i <= AdminMachines.Length - 1; i++)
                {
                    if (AdminMachines[i] == row["MachineName"].ToString())
                    {
                        isAdmin = true;
                    }
                }
                if (isAdmin == false)
                {
                    Status status = new Status();
                    status.MachineName = row["MachineName"].ToString();
                    status.MachineStatus = row["Status"].ToString();
                    //Console.WriteLine(status.MachineName + "   " + status.MachineStatus);
                    status.StatusTime = (DateTime)row["StatusTime"];
                    status.UserName = row["UserName"].ToString();
                    status.Location = row["Location"].ToString();
                    status.VersionNumber = row["VersionNumber"].ToString();
                    st.Add(status);
                }
            }

            CalculateTotals(st);
            //Itterate through all form controls
            foreach (Control c in this.Controls)
            {
                if (c is GroupBox)
                {
                    foreach (var item in st)
                    {
                        if (c.Text == item.MachineName) // Iterate through databse returned list
                        {
                            //Iterate through groupbox controls
                            foreach (Control z in c.Controls)
                            {
                                if (z is PictureBox)
                                    z.Dispose();
                                if (z is Label)
                                {
                                    if (z.TabIndex == 1)
                                    {
                                        //z.Text = item.MachineStatus;
                                        if (item.MachineStatus == "READ")
                                        {
                                            c.BackColor = ColorRead;
                                            c.ForeColor = Color.White;
                                            z.BackColor = ColorRead;
                                            z.Text = Strings.StatusForm_read;
                                        }
                                        if (item.MachineStatus == "INACTIVE")
                                        {
                                            c.BackColor = ColorInactive;
                                            c.ForeColor = Color.White;
                                            z.BackColor = ColorInactive;
                                            z.Text = Strings.StatusForm_inactive;
                                            //var pb = new PictureBox();
                                            //pb.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Images\\blink.gif");
                                            
                                            //pb.Dock = DockStyle.Fill;
                                            //pb.Size = new Size(20, 20);
                                            //pb.Location = new Point(68, 32);
                                            //c.Controls.Add(pb);
                                        }
                                        if (item.MachineStatus == "FIND")
                                        {
                                            c.BackColor = ColorFind;
                                            c.ForeColor = Color.White;
                                            z.BackColor = ColorFind;
                                            z.Text = Strings.StatusForm_find;
                                        }
                                        if (item.MachineStatus == "VEP CLOSED")
                                        {
                                            c.BackColor = ColorClosed;
                                            c.ForeColor = Color.White;
                                            z.BackColor = ColorClosed;
                                            z.Text = Strings.StatusForm_vepclosed;

                                        }
                                        if (item.MachineStatus == "OFF")
                                        {
                                            c.BackColor = ColorOff;
                                            c.ForeColor = Color.White;
                                            z.BackColor = ColorOff;
                                            z.Text = Strings.StatusForm_off;
                                        }
                                        if (item.MachineStatus == "OTHER PROG")
                                        {
                                            c.BackColor = ColorOtherProg;
                                            c.ForeColor = Color.White;
                                            z.BackColor = ColorOtherProg;
                                            z.Text = Strings.StatusForm_otherprog;
                                        }
                                    }
                                    if (z.TabIndex == 2)
                                        z.Text = item.UserName;
                                    if (z.TabIndex == 3)
                                        z.Text = "(V." + item.VersionNumber +")";
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CalculateTotals(List<Status> st)
        {
            labelVepClosed.Text = st.Where(x => x.MachineStatus == "VEP CLOSED" && x.Location == "Nachshonim").Count().ToString();
            labelInactive.Text = st.Where(x => x.MachineStatus == "INACTIVE" && x.Location == "Nachshonim").Count().ToString();
            labelOff.Text = st.Where(x => x.MachineStatus == "OFF" && x.Location == "Nachshonim").Count().ToString();
            labelRead.Text = st.Where(x => x.MachineStatus == "READ" && x.Location == "Nachshonim").Count().ToString();
            labelFind.Text = st.Where(x => x.MachineStatus == "FIND" && x.Location == "Nachshonim").Count().ToString();
            labelOtherProg.Text = st.Where(x => x.MachineStatus == "OTHER PROG" && x.Location == "Nachshonim").Count().ToString();

            labelVepClosed2.Text = st.Where(x => x.MachineStatus == "VEP CLOSED" && x.Location == "Petach Tikva").Count().ToString();
            labelInactive2.Text = st.Where(x => x.MachineStatus == "INACTIVE" && x.Location == "Petach Tikva").Count().ToString();
            labelOff2.Text = st.Where(x => x.MachineStatus == "OFF" && x.Location == "Petach Tikva").Count().ToString();
            labelRead2.Text = st.Where(x => x.MachineStatus == "READ" && x.Location == "Petach Tikva").Count().ToString();
            labelFind2.Text = st.Where(x => x.MachineStatus == "FIND" && x.Location == "Petach Tikva").Count().ToString();
            labelOtherProg2.Text = st.Where(x => x.MachineStatus == "OTHER PROG" && x.Location == "Petach Tikva").Count().ToString();
        }


        private void ReadLocationXML()
        {
            var mname = string.Empty;
            var mlocation = string.Empty;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                string XMLPath = ConfigurationManager.AppSettings["XMLPath"];
                if (!string.IsNullOrEmpty(XMLPath))
                    xmlDoc.Load(XMLPath);
                else
                    return;
                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/controls/machine");

                // Read particular values form each node
                foreach (XmlNode node in nodeList)
                {
                    mname = node.Attributes["Name"].Value;
                    mlocation = node.Attributes["Location"].Value;
                    string[] location = ConfigurationManager.AppSettings[mlocation].Split(',');
                    int x = Convert.ToInt32(location[0]);
                    int y = Convert.ToInt32(location[1]);
                    foreach (Control c in this.Controls)
                    {
                        if (c is GroupBox && c.Text == mname)
                        {
                             c.Location = new Point(x, y);
                        }
                    }
                }
            }
            catch(Exception ex){}
        }
        
    }

}
