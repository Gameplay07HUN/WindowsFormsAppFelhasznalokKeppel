using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsAppFelhasznalokKeppel
{
    internal class KepCuccos
    {
        int id;
        string nev;
        DateTime datum;
        Image kep;

        public int Id { get => id; set => id = value; }
        public string Nev { get => nev; set => nev = value; }
        public DateTime Datum { get => datum; set => datum = value; }

        public Image Kep { get => kep; set => kep = value; }

        public KepCuccos(int id, string nev, DateTime datum, Image kep)
        {
            Id = id;
            Nev = nev;
            Datum = datum;
            Kep = kep;
        }
        public override string ToString()
        {
            return id + " (" + nev + " )" + Datum+kep;
        }
    }
}
