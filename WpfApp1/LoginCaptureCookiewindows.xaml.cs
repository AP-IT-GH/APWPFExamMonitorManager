using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using WpfApp1.Tools;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for LoginCaptureCookiewindows.xaml
    /// </summary>
    public partial class LoginCaptureCookiewindows : MetroWindow
    {
        public LoginCaptureCookiewindows()
        {
            InitializeComponent();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //var cookie= GetUriCookieContainer(new Uri("http://examonitoring.ap.be/#/dashboard"));
            //var cook= cookie.GetCookies(new Uri("http://examonitoring.ap.be/#/dashboard"));

            //Properties.Settings.Default.cookieCreateTime = DateTime.Now;
            //Properties.Settings.Default.cookieSessionValue = cook[0].Value;
            //Properties.Settings.Default.Save();

            //this.Close();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();

            //userlogin
            bool result = await RestClient.LoginAndGetSession(txbUser.Text, txbPass.Password, client);
            if (result == true)
            {
                MessageBox.Show("Gelukt");
                //close en return client?
            }

            if (chkSafePW.IsChecked == true)
            {
                Properties.Settings.Default.SafePW = true;
                Properties.Settings.Default.UserName = txbUser.Text;
                Properties.Settings.Default.Password = SecurePasswordVault.EncryptString(SecurePasswordVault.ToSecureString(txbPass.Password));
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.SafePW = false;
                Properties.Settings.Default.UserName = "";
                Properties.Settings.Default.Password = "";
                Properties.Settings.Default.Save();
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.SafePW == true)
            {
                chkSafePW.IsChecked = true;
                txbUser.Text = Properties.Settings.Default.UserName;
                txbPass.Password = SecurePasswordVault.ToInsecureString(SecurePasswordVault.DecryptString(Properties.Settings.Default.Password));

            }
        }
    }
}
