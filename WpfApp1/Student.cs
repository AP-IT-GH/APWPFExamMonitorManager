using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Student
    {
       
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }

        public override string ToString()
        {
            return $"{Achternaam} {Voornaam}";
        }
        public bool IsMissing { get; set; } = true;
        public string MissingColor
        {
          get
            {
                if (IsMissing) return "Red";
                return "LightGreen";
            }
        }

  
        public string OrderName
        {
            get
            {
                return this.ToString();
            }
        }
    }
}
