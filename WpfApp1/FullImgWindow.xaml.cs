using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for FullImgWindow.xaml
    /// </summary>
    public partial class FullImgWindow : MahApps.Metro.Controls.MetroWindow
    {
        private ScreenshotSessionData allScreens;
        private ExamSession currentSession;

        public int currentImage = -1;
        //public FullImgWindow()
        //{
        //    InitializeComponent();
        //}

        public FullImgWindow(ScreenshotSessionData allscreenin, ExamSession currentSessionin)
        {
            InitializeComponent();
            allScreens = allscreenin;
            currentSession = currentSessionin;
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void btnPreviousScreen_Click(object sender, RoutedEventArgs e)
        {
            currentImage--;
            srcImage.Source = new BitmapImage(new Uri(allScreens.Shots[currentImage].Full));
            ShowInfo();
            if (currentImage>0)
            {
              
                btnNextScreen.IsEnabled = true;
            }
            else
            {
                btnPreviousScreen.IsEnabled = false;
            }
        }

        private void btnNextScreen_Click(object sender, RoutedEventArgs e)
        {
            currentImage++;
            srcImage.Source = new BitmapImage(new Uri(allScreens.Shots[currentImage].Full));
            ShowInfo();
            if (currentImage < allScreens.Shots.Count-1 )
            {
               
                btnPreviousScreen.IsEnabled = true;
            }
            else
            {
                btnNextScreen.IsEnabled = false;
            }

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(currentImage==0)
            {
                btnPreviousScreen.IsEnabled = false;
            }
            if(currentImage== allScreens.Shots.Count-1)
            {
                btnNextScreen.IsEnabled = false;
            }

            ShowInfo();
        }
        
        private void ShowInfo()
        {
            txbInfo.Text = $"Gebruiker: {allScreens.user.lastname} {allScreens.user.firstname}. Screenshot genomen om: {allScreens.Shots[currentImage].TimeTaken}";
        }

        private void MetroWindow_KeyUp(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Left:
                    btnPreviousScreen_Click(this, null);
                    break;
                case Key.Right:
                    btnNextScreen_Click(this, null);
                    break;
                case Key.Escape:
                    this.Close();
                    break;
            }
        }
    }
}
