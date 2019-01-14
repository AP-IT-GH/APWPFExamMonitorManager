using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Screenshot
    {
        public string Thumb { get; set; }
        public string Full { get; set; }
        public int ID { get; set; }
        public DateTime TimeTaken { get; set; }

        public string ShortTime
        {
            get
            {
                return TimeTaken.ToString("HH:mm");
            }
        }

    }
    public class ScreenshotSessionData
    {
        public string directory { get; set; }
        public User user { get; set; }
        public Exam exam { get; set; }
        public string[][] thumbnails { get; set; }
        private List<Screenshot> screenshots = null;
        public List<Screenshot> Shots
        {
            get
            {
                if (screenshots == null)
                {
                    screenshots = new List<Screenshot>();
                    var l = thumbnails.SelectMany(T => T).ToList();
                    int counter = 0;
                    foreach (var sc in l)
                    {
                        string filename = sc.Replace("_thumb", string.Empty);
                        screenshots.Add(new Screenshot()
                        {
                            Thumb = $"{directory}\\{sc}",
                            Full = $"{directory}\\{filename}",
                            ID = counter,
                            TimeTaken = FromUnixTime(Convert.ToDouble(filename.Split('.')[0]))
                        });
                        counter++;

                    }

                    //screenshots =  ProcessImages(screenshots);
                }

                return screenshots;
            }
        }

        public static DateTime FromUnixTime(double unixTime)
        {
            return epoch.AddSeconds(unixTime).AddHours(1); //+1 hour want iets werkt in zomeruur precies
        }
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }

    public class User
    {
        public string id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string role { get; set; }
    }

    public class Exam
    {
        public string id { get; set; }
        public string name { get; set; }
        public string userid { get; set; }
        public string accessid { get; set; }

    }


}
