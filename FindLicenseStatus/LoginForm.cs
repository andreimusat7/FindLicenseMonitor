
using FindLicenseStatus.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FindLicenseStatus
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            ReadFromResources();
        }

        private void ReadFromResources()
        {
            string currentResource = ConfigurationManager.AppSettings["Culture"];
            if (!string.IsNullOrEmpty(currentResource))
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(currentResource);
            label1.Text = Strings.LoginForm_username;
            label2.Text = Strings.LoginForm_password;
            button1.Text = Strings.LoginForm_submit;
            TextBoxPass.UseSystemPasswordChar = true;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var savedusername = "admin";
            var savedpassword = "718779752b851ac0dc6281a8c8d77e7e";
            var usernameEntered = TextBoxUser.Text;
            var passwordEntered = GetMd5Hash(TextBoxPass.Text);


            if (passwordEntered == savedpassword && savedusername == usernameEntered)
            {
                this.Hide();
                StatusForm form2 = new StatusForm();
                form2.ShowDialog();
                this.Close();   
            }
            else {
                labelPasswordMatch.Visible = true;
            }
        }
        
        public static string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
