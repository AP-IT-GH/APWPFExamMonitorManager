using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

using Microsoft.Win32;
using WpfApp1;

namespace GUI_Frontend_WPF
{
    /// <summary>
    /// Interaction logic for ImporteerHelperWindow.xaml
    /// </summary>
    public partial class ImporteerHelperWindow
    {
        private IEnumerable<ExamSession> sessions;

        public List<Student> NamenLijst { get; set; }

        public ImporteerHelperWindow()
        {
            InitializeComponent();
            NamenLijst = new List<Student>();
        }

        public ImporteerHelperWindow(IEnumerable<ExamSession> itemsSource) : this()
        {
            this.sessions = itemsSource;
        }

        private void BtnBameFlexCvsImport_OnClick(object sender, RoutedEventArgs e)
        {
            //TODO: settings vragen:
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "CSV file|*.csv";
            if (dlg.ShowDialog() == true)
            {
                NamenLijst = GetCVSObjects(dlg.FileName,true, ';');
                GeneratePreview();
            }

        }
        private void GeneratePreview()
        {
            foreach (var naam in NamenLijst)
            {
                if (sessions?.FirstOrDefault(p => p.OrderName == naam.OrderName) != null)
                    naam.IsMissing = false;
            }

            lbPreview.ItemsSource = NamenLijst.OrderByDescending(p => p.IsMissing);
        }

        private void BtnPlainTekstImport_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "TXT file|*.txt";
            if (dlg.ShowDialog() == true)
            {
                NamenLijst = GetPlainTxtObjects(dlg.FileName);
                GeneratePreview();
            }

        }

        private List<Student> GetPlainTxtObjects(string filename)
        {
            var datalines = new List<Student>();
            foreach (var line in File.ReadAllLines(filename, Encoding.Unicode))
            {

                if (line != "")
                {
                    datalines.Add(new Student()
                    {

                        Achternaam = line.Trim()
                    });
                }
            }
            return datalines;
        }

        private void BtnOk_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }


        private Encoding DiscoverEncoding(string filePath)
        {
            System.Text.Encoding enc = null;
            System.IO.FileStream file = new System.IO.FileStream(filePath,
                FileMode.Open, FileAccess.Read, FileShare.Read);
            if (file.CanSeek)
            {
                byte[] bom = new byte[4]; // Get the byte-order mark, if there is one 
                file.Read(bom, 0, 4);
                if ((bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) || // utf-8 
                    (bom[0] == 0xff && bom[1] == 0xfe) || // ucs-2le, ucs-4le, and ucs-16le 
                    (bom[0] == 0xfe && bom[1] == 0xff) || // utf-16 and ucs-2 
                    (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)) // ucs-4 
                {
                    enc = System.Text.Encoding.Unicode;
                }
                else
                {
                    enc = System.Text.Encoding.ASCII;
                }

                // Now reposition the file cursor back to the start of the file 
                file.Seek(0, System.IO.SeekOrigin.Begin);
            }
            else
            {
                // The file cannot be randomly accessed, so you need to decide what to set the default to 
                // based on the data provided. If you're expecting data from a lot of older applications, 
                // default your encoding to Encoding.ASCII. If you're expecting data from a lot of newer 
                // applications, default your encoding to Encoding.Unicode. Also, since binary files are 
                // single byte-based, so you will want to use Encoding.ASCII, even though you'll probably 
                // never need to use the encoding then since the Encoding classes are really meant to get 
                // strings from the byte array that is the file. 

                enc = System.Text.Encoding.UTF8;
            }
            return enc;
        }
        private List<Student> GetCVSObjects(string filename, bool hasheader = false, char delimiter = ';')
        {

            int count = 0;
            var datalines = new List<Student>();
            foreach (var line in File.ReadAllLines(filename))
            {
                if (!hasheader || (hasheader && count >0))
                {
                    var split = line.Split(delimiter);
                    Student s = new Student();
                    s.Voornaam = split[0].Replace("\"", string.Empty);
                    s.Achternaam = split[1].Replace("\"", string.Empty);

                    datalines.Add(s);
                }
                count++;
            }
            return datalines;
        }

    }
}
