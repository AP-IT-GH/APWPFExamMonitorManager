using GUI_Frontend_WPF;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfApp1.ImageLab;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

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

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
           
                try
                {
                    RefreshData();
                    
                        timerRefresh.IsEnabled = true;
                        timerRefresh.Start();

                }
                catch (Exception exc)
                {
                await this.ShowMessageAsync("Error",exc.Message);
                }
            
        }

        private async void ButtonViewScreenShots_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExamSession currses = (sender as Button).DataContext as ExamSession;
                txbCurrName.Text = currses.student + " " + currses.status;
                WebClient wc = new WebClient();
                //
                wc.Headers.Add(HttpRequestHeader.Cookie, $"ci_session={currCookiesession}");
                var res = await wc.DownloadStringTaskAsync(new Uri($"http://examonitoring.ap.be/api/sessions/getScreenshotList/{currses.id}"));

                var screendat = JsonConvert.DeserializeObject<ScreenshotSessionData>(res);

                lbScreens.ItemsSource = screendat.Shots;
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("BAM.KAPOT. Niet goed.", "Error=" + ex.Message);
            }

        }


        private async void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var url = ((sender as Image).DataContext as Screenshot).Full;
                imfull.Source = new BitmapImage(new Uri(url));
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
                if (await this.ShowMessageAsync("Opgelet","Zeker dat je deze sessie wenst af te sluiten?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    //h

                    ExamSession currses = (sender as Button).DataContext as ExamSession;

                    WebClient wc = new WebClient();
                    //
                    wc.Headers.Add(HttpRequestHeader.Cookie, $"ci_session={currCookiesession}");
                    var res = await wc.DownloadStringTaskAsync(new Uri($"http://examonitoring.ap.be/api/sessions/finishSession/{currses.id}"));

                }
                
            }
            catch (Exception ex)
            {

                await this.ShowMessageAsync("BAM.KAPOT. Niet goed.", "Error=" + ex.Message);
            }
            timerRefresh.Start();
        }

        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            timerRefresh.Stop();
            try
            {
                if (txbStudFulter.Text != "")
                    lbSessions.ItemsSource = FilterData().Where(p => p.student.ToLower().Contains(txbStudFulter.Text.ToLower()));
                else
                {
                    IEnumerable<ExamSession> f = FilterData();
                    lbSessions.ItemsSource = f;
                }
            }
            catch (Exception ex)
            {

                await this.ShowMessageAsync("BAM.KAPOT. Niet goed.","Error=" + ex.Message);
            }
            timerRefresh.Start();

        }

        private IEnumerable<ExamSession> FilterData()
        {
            if (txbLectorFilter.Text != "")
                return allsessions.OrderBy(p => p.OrderName).Where(p => p.lector.ToLower().Contains(txbLectorFilter.Text.ToLower()) || p.exam.ToLower().Contains(txbLectorFilter.Text.ToLower()));

            else
                return allsessions.OrderBy(p => p.OrderName);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                txbStudFulter.TextChanged += TextBox_TextChanged;

                var time = DateTime.Now.Subtract(Properties.Settings.Default.cookieCreateTime);
                if (time.Days == 0 && time.Hours == 0 && time.Minutes < 55)
                {
                   // MessageBox.Show("We can use the cookie again");
                    currCookiesession = Properties.Settings.Default.cookieSessionValue;
                }
                else
                {
                    LoginCaptureCookiewindows wnd = new LoginCaptureCookiewindows();
                    this.Visibility = Visibility.Hidden;
                    wnd.ShowDialog();
                    this.Visibility = Visibility.Visible;
                    currCookiesession = Properties.Settings.Default.cookieSessionValue;
                }

                timerRefresh.IsEnabled = false;
                timerRefresh.Interval = new TimeSpan(0, 0, 30);
                timerRefresh.Tick += (p, ex) => { RefreshData(); };
                DownloadButton_Click(this, null);
            }
            catch (Exception ex)
            {

                await this.ShowMessageAsync("BAM.KAPOT. Niet goed."," Error=" + ex.Message);
            }
        }

        private async void RefreshData()
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add(HttpRequestHeader.Cookie, $"ci_session={currCookiesession}");
                var res = await wc.DownloadStringTaskAsync(new Uri("http://examonitoring.ap.be/api/sessions/getActiveSessions"));
                allsessions = JsonConvert.DeserializeObject<List<ExamSession>>(res);
                var filterd = FilterData();
                lbSessions.ItemsSource = filterd;
                Title = $"SESSIONS={filterd.Count().ToString()} \t  TIMEMOUT={filterd.Where(p => p.status == "Time-out").Count()}   \t\tTimeRefresh={DateTime.Now.TimeOfDay}";
                CanvasAlert(filterd);
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("Error",ex.Message);

            }
        }

        private void CanvasAlert(IEnumerable<ExamSession> filterd)
        {
            //if (filterd.Count(p => p.status == "Time-out") > 0)
            //    cnvTimeMout.Visibility = Visibility.Visible;
            //else cnvTimeMout.Visibility = Visibility.Collapsed;
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

        private void imfull_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Window winimg = new Window();
            Image g = new Image();
            g.Source = imfull.Source;
            winimg.Content = g;
            winimg.WindowState = WindowState.Maximized;
            winimg.WindowStyle = WindowStyle.SingleBorderWindow;

            winimg.ShowDialog();
        }



        private async void MagicButton_Click(object sender, RoutedEventArgs e)
        {
            var screens = (lbScreens.ItemsSource as List<Screenshot>);
            ImageLabWindow wc = new ImageLabWindow();
            wc.Screens = screens;
            wc.ShowDialog();


        }


    }
}
