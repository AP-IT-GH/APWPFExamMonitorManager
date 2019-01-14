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
        public ScreenshotSessionData AllScreens { get; set; }
        public int currentImage = -1;
        public FullImgWindow()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void btnPreviousScreen_Click(object sender, RoutedEventArgs e)
        {
            currentImage--;
            srcImage.Source = new BitmapImage(new Uri(AllScreens.Shots[currentImage].Full));
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
            srcImage.Source = new BitmapImage(new Uri(AllScreens.Shots[currentImage].Full));
            if (currentImage < AllScreens.Shots.Count-1 )
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
            if(currentImage== AllScreens.Shots.Count-1)
            {
                btnNextScreen.IsEnabled = false;
            }
        }
    }
}
