using MVVM1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NetworkService.Model
{
    public class TrougaoPodaci : BindableBase
    {
        private double visina;
        private Brush boja;

        public double Visina
        {
            get { return visina; }
            set { SetProperty(ref visina, value); }
        }

        public Brush Boja
        {
            get { return boja; }
            set { SetProperty(ref boja, value); }
        }
    }
}
