using GUI_Frontend_WPF;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfApp1.ImageLab;
using WpfApp1.Tools;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        private string currCookiesession = "";
        DispatcherTimer timerRefresh = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();

        }


        IEnumerable<ExamSession> allsessions;

        private async void RefreshDataFromServer()
        {
            //Todo: button bestaat niet meer. wordt nu gewoon bij onloaded gestart
            try
            {
                timerRefresh.Stop();
                DownloadActiveSessions();


                timerRefresh.Start();

            }
            catch (Exception exc)
            {
                await this.ShowMessageAsync("Error", exc.Message);
            }

        }




        private async void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var url = ((sender as Image).DataContext as Screenshot).Full;


                

                FullImgWindow wnd = new FullImgWindow(currentScreens,currentSession );
                wnd.currentImage = ((sender as Image).DataContext as Screenshot).ID;
                wnd.srcImage.Source = new BitmapImage(new Uri(url));
                wnd.ShowDialog();
            }
            catch (Exception ex)
            {

                await this.ShowMessageAsync("BAM.KAPOT. Niet goed.", "Error=" + ex.Message);
            }

        }

        private async void btnCloseSession_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                timerRefresh.Stop();

                if (chkAskConfirm.IsChecked == true)
                {
                    if (await this.ShowMessageAsync("Opgelet", "Zeker dat je deze sessie wenst af te sluiten?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                    {
                        lbSessions.SelectedIndex = -1;
                        lbScreens.ItemsSource = null;
                        await CloseSession(sender);

                    }
                }
                else
                {
                    lbSessions.SelectedIndex = -1;
                    lbScreens.ItemsSource = null;
                    await CloseSession(sender);
                }
            }
            catch (Exception ex)
            {

                await this.ShowMessageAsync("BAM.KAPOT. Niet goed.", "Error=" + ex.Message);
            }
            RefreshDataFromServer();
            timerRefresh.Start();
        }

        private async Task CloseSession(object sender)
        {
            try
            {
                ExamSession currses = (sender as Button).DataContext as ExamSession;

                WebClient wc = new WebClient();
                wc.Headers.Add(HttpRequestHeader.Cookie, $"ci_session={currCookiesession}");

                await RestClient.CloseSession(currses.id);
                ((sender as Button).Parent as StackPanel).IsEnabled = false;
                ((sender as Button).Parent as StackPanel).Background = new SolidColorBrush(Colors.DarkGray);
                lbSessions.SelectedIndex = -1;
            }
            catch (Exception ex)
            {

                await this.ShowMessageAsync("BAM.KAPOT. Niet goed.", "Sessie close failed. Error=" + ex.Message);
            }

            //

        }

        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await FilterItemSourceOffline();

        }

        private IEnumerable<ExamSession> FilterData()
        {
            var resul = allsessions as IEnumerable<ExamSession>;
            if (txbStudFulter.Text != "")
                resul = resul.Where(p => p.student.ToLower().Contains(txbStudFulter.Text.ToLower()));
            if (txbLectorFilter.Text != "")
                resul = resul.Where(p => p.lector.ToLower().Contains(txbLectorFilter.Text.ToLower()) || p.exam.ToLower().Contains(txbLectorFilter.Text.ToLower()));
            if (chkOnlyTimeOuts.IsChecked == true)
                resul = resul.Where(p => p.status == "Time-out");



            return resul.OrderBy(p => p.OrderName);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                txbStudFulter.TextChanged += TextBox_TextChanged;
                if (Properties.Settings.Default.SafePW == true)
                {
                    string user = Properties.Settings.Default.UserName;
                    string pass = SecurePasswordVault.ToInsecureString(SecurePasswordVault.DecryptString(Properties.Settings.Default.Password));
                    var gotit = await RestClient.LoginAndGetSession(user, pass);
                    if(gotit!=true) //Login vereist
                    {
                        LoadLoginAndQuitIfNeeded();
                    }

                }
                else //Login vereist
                {
                    LoadLoginAndQuitIfNeeded();
                }



                timerRefresh.Interval = new TimeSpan(0, 0, 15);
                timerRefresh.Tick += (p, ex) => { DownloadActiveSessions(); };
                timerRefresh.IsEnabled = true;
                RefreshDataFromServer();
            }
            catch (Exception ex)
            {

                await this.ShowMessageAsync("BAM.KAPOT. Niet goed.", " Error=" + ex.Message);
            }
        }

        private async void DownloadActiveSessions()
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add(HttpRequestHeader.Cookie, $"ci_session={currCookiesession}");
                var res = await RestClient.GetActiveSessions();
                allsessions = JsonConvert.DeserializeObject<List<ExamSession>>(res);
                var filterd = FilterData();
                lbSessions.ItemsSource = filterd;
                Title = $"SESSIONS={filterd.Count().ToString()} \t  TIMEMOUT={filterd.Where(p => p.status == "Time-out").Count()}   \t\tLaatste update was om={DateTime.Now.ToString("HH:mm:ss")}";

            }
            catch (Exception ex)
            {
              //  await this.ShowMessageAsync("Error", ex.Message);

            }
        }



        private void btnDebug_Click(object sender, RoutedEventArgs e)
        {
            LoginCaptureCookiewindows wnd = new LoginCaptureCookiewindows();
            wnd.ShowDialog();
        }

        private void btnMissing_Click(object sender, RoutedEventArgs e)
        {
            ImporteerHelperWindow wnd = new ImporteerHelperWindow(lbSessions.ItemsSource as IEnumerable<ExamSession>);
            wnd.ShowDialog();
        }

        private ScreenshotSessionData currentScreens = null;
        private ExamSession currentSession = null;
        private async void lbSessions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbSessions.SelectedItem != null)
            {
                try
                {
                    currentSession = null;
                    currentScreens = null;
                    lbScreens.ItemsSource = null;
                    currentSession = lbSessions.SelectedItem as ExamSession;
                    //TODO:txbCurrName.Text = "Student:" + currses.student + " [Status:" + currses.status + "]";
                    WebClient wc = new WebClient();
                    //
                    wc.Headers.Add(HttpRequestHeader.Cookie, $"ci_session={currCookiesession}");
                    var res = await wc.DownloadStringTaskAsync(new Uri($"http://examonitoring.ap.be/api/sessions/getScreenshotList/{currentSession.id}"));

                    currentScreens = JsonConvert.DeserializeObject<ScreenshotSessionData>(res);
                   
                    lbScreens.ItemsSource = currentScreens.Shots;
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("BAM.KAPOT. Niet goed.", "Error=" + ex.Message);
                }
            }
        }

        private async void txbLectorFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            await FilterItemSourceOffline();
        }

        private async void btnInfo_Click(object sender, RoutedEventArgs e)
        {

            string url = "https://github.com/AP-Elektronica-ICT/APWPFExamMonitorManager";
            string version = "debug/onbekend";
            try
            {
                version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch
            {
                //ifgnore
            }

            string fulltekst = $"Versie:{version}.\r\nDit programma is geschreven door Tim Dams, AP Hogeschool Antwerpen (2019).\r\n Contacteer me bij vragen of opmerkingen. \r\n\r\nDe volledige broncode van dit programma kan op github bekeken worden: {url}";
            await this.ShowMessageAsync("Over dit programma", fulltekst);
        }

        private async void chkOnlyTimeOuts_Checked(object sender, RoutedEventArgs e)
        {
            await FilterItemSourceOffline();
        }

        private async Task FilterItemSourceOffline()
        {
            timerRefresh.Stop();
            try
            {
                lbSessions.ItemsSource = FilterData();
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("BAM.KAPOT. Niet goed.", "Error=" + ex.Message);
            }
            timerRefresh.Start();
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RefreshDataFromServer();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SafePW = false;
            Properties.Settings.Default.Save();
            timerRefresh.Stop();
            LoadLoginAndQuitIfNeeded();
        }

        private void LoadLoginAndQuitIfNeeded()
        {
            LoginCaptureCookiewindows wnd = new LoginCaptureCookiewindows();
            this.Visibility = Visibility.Hidden;
            if (wnd.ShowDialog() == false)
                System.Windows.Application.Current.Shutdown();
            this.Visibility = Visibility.Visible;
        }
    }
}
