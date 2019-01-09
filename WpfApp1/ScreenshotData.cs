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
    }
    public class ScreenshotSessionData
    {
        public string directory { get; set; }
        public User user { get; set; }
        public Exam exam { get; set; }
        public string[][] thumbnails { get; set; }
        private List<Screenshot> screenshots = null;
        public  List<Screenshot> Shots
        {
            get
            {
                if(screenshots==null)
                {
                    screenshots = new List<Screenshot>();
                    var l  = thumbnails.SelectMany(T => T).ToList();
                    foreach (var sc in l)
                    {
                        screenshots.Add(new Screenshot()
                        {
                            Thumb = $"{directory}\\{sc}",
                            Full = $"{directory}\\{sc.Replace("_thumb",string.Empty)}"
                        });
                            
                    }

                  //screenshots =  ProcessImages(screenshots);
                }

                return screenshots;
            }
        }

        //private async List<Screenshot> ProcessImages(List<Screenshot> screenshots)
        //{
        //    //Enkel check op laatste 2:
        //    WebClient wc = new WebClient();
        //    await wc.DownloadFileTaskAsync(new Uri(screenshots.Last().Thumb), "1.png");
        //    await wc.DownloadFileTaskAsync(new Uri(screenshots[screenshots.Count-2].Thumb), "2.png");

        //    return screenshots;
        //}
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
