using FindLicenseStatus.Core;
using FindLicenseStatus.DAL;
using FindLicenseStatus.Resources;
using FindLinceseStatus.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FindLicenseStatus
{
    public partial class Report : Form
    {
        private string mname;
        Database db = new Database();

        private BindingSource _gridSource;

        public enum ViewType
        {
            Hour,
            Day,
            Week,
            Month
        }

        List<string> ReportTime = new List<string>();

        private BindingSource GridSource
        {
            get
            {
                if (_gridSource == null)
                    _gridSource = new BindingSource();
                return _gridSource;
            }
        }
        public Report(string machinename, string user )
        {
            InitializeComponent();
            mname = machinename;
           
            //comboBoxMachines.DataSource = db.GetMachines();
            List<string> allUsers = new List<string>();
            allUsers = db.GetUsers();
            allUsers.Insert(0,  Name = Strings.Report_allusers );
            comboBoxUsers.DataSource = allUsers;
            //comboBoxMachines.SelectedIndex = comboBoxMachines.FindString(machinename);
            comboBoxUsers.SelectedIndex = comboBoxUsers.FindString(user);
            dateTimePicker1.CustomFormat = "dd/MM/yyyyy hh:mm";
            dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;

            dateTimePicker2.CustomFormat = "dd/MM/yyyyy hh:mm";
            dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;

            dateTimePicker1.Value = DateTime.Now.AddDays(-7);

            string startdate = dateTimePicker1.Value.ToShortDateString();
            string enddate = dateTimePicker2.Value.ToShortDateString();
            GetFromResources();
            comboBox1.DataSource = ReportTime;
            comboBox1.DisplayMember = "name";
           
            int groupby = comboBox1.SelectedIndex;
            FillData(mname, user, startdate, enddate, groupby);
            
        }

        private void FillData(string machinename, string username, string startdate, string enddate, int groupby, bool clean = false)
        {

            SortableBindingList<Reports> reports = new SortableBindingList<Reports>();
            reports = db.ReadReportDatabase(machinename, username, startdate, enddate, groupby, clean);

            GridSource.DataSource = reports;
            // dataGridView1.Columns["EarType"].Visible = false; //Optionally hide a column
            reportGridView1.DataSource = reports;


            // Resize the master DataGridView columns to fit the newly loaded data.
            reportGridView1.AutoResizeColumns();

            // Configure the details DataGridView so that its columns automatically
            // adjust their widths when the data changes.
            reportGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            if (reports.Count > 0)
            {
                reportGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                reportGridView1.Columns[0].HeaderText = Strings.Report_starttime;
                reportGridView1.Columns[1].HeaderText = Strings.Report_findcars;
                reportGridView1.Columns[2].HeaderText = Strings.Report_readcars;
                reportGridView1.Columns[3].HeaderText = "Saved cars";
                reportGridView1.Columns[4].HeaderText = Strings.Report_scrollhits;
                reportGridView1.Columns[5].HeaderText = Strings.Report_idles;
                reportGridView1.Columns[6].HeaderText = Strings.Report_zooms;
                reportGridView1.Columns[7].HeaderText = Strings.Report_magnifiers;
                reportGridView1.Columns[8].HeaderText = Strings.Report_negatives;
                reportGridView1.Columns[9].HeaderText = Strings.Report_brightness;
            }

        }
        private void buttonFilter_Click(object sender, EventArgs e)
        {
            string startdate = dateTimePicker1.Value.ToString();
            string enddate = dateTimePicker2.Value.ToString();
            string machinename = string.Empty;
            string username = string.Empty;
            if (comboBoxUsers.SelectedIndex != 0)
                username = comboBoxUsers.SelectedValue.ToString();
            else
                username = string.Empty;
            int groupby = comboBox1.SelectedIndex;
            bool clean = CheckboxClean.Checked;
            FillData(machinename, username, startdate, enddate, groupby, clean);
        }
        private void GetFromResources() {
            label1.Text = Strings.Report_daterange;
            label3.Text = Strings.Report_users;
            CheckboxClean.Text = Strings.Report_cleanresults;
            label5.Text = Strings.Report_display;
            buttonFilter.Text = Strings.Report_submit;

            ReportTime.Add(Strings.Report_perhour);
            ReportTime.Add(Strings.Report_perday);
            ReportTime.Add(Strings.Report_perweek);
            ReportTime.Add(Strings.Report_permonth);
        }
    }
}
