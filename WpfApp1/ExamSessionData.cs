using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{



    public class ExamSession
    {
        public string id { get; set; }
        public string student { get; set; }
        public string exam { get; set; }
        public string lector { get; set; }
        public string starttime { get; set; }
        public string status { get; set; }


        public string StateAsColor
        {
            get
            {
                switch (status)
                {
                    case "Actief": return "Green";
                    case "Time-out": return "Orange";
                }
                return "White";
            }
        }

        public string OrderName
        {
            get
            {
                string wrongname = student;
                var split = student.Split(' ');
                var just = "";
                for (int i = 1; i < split.Length; i++)
                {
                    just += split[i] + " ";
                }
                just += split[0];
                return just;
            }
        }

        public string ExtraInfo
        {
            get
            {
                return $"[Lector:{lector}, Examen:{exam}, Starttijd:{starttime}]";
            }
        }

    }

}
