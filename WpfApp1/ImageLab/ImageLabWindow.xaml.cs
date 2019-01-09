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
using System.Windows.Shapes;

namespace WpfApp1.ImageLab
{
    /// <summary>
    /// Interaction logic for ImageLabWindow.xaml
    /// </summary>
    public partial class ImageLabWindow : Window
    {
        public ImageLabWindow()
        {
            InitializeComponent();
        }

        public List<Screenshot> Screens { get; internal set; }

        private async void btnMagic_Click(object sender, RoutedEventArgs e)
        {
            if (Screens != null)
            {
                for (int i = 0; i < 5 && i < Screens.Count; i++)
                {
                    WebClient wc = new WebClient();

                    await wc.DownloadFileTaskAsync(new Uri(Screens[i].Full), "img" + i + ".png");
                }
            }


        }

    }
}
