using MVVM1;
using MVVMLight.Messaging;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NetworkService.ViewModel
{
    public class MeasurementGraphViewModel : BindableBase
    {
        public static ObservableCollection<Server> Servers { get; set; }

        public static ObservableCollection<TrougaoPodaci> Podaci { get; set; }

        public static Dictionary<int,ObservableCollection<Tuple<double, DateTime>>> dataFromLog;

        public Server selectedServer = null;

        public Server SelectedServer
        {
            get { return selectedServer; }
            set
            {
                SetProperty(ref selectedServer, value);

                if(selectedServer != null)
                {
                    CreateGraph();
                }
            }
        }

        public MeasurementGraphViewModel()
        {
            dataFromLog = new Dictionary<int, ObservableCollection<Tuple<double, DateTime>>>();
            Messenger.Default.Register<ObservableCollection<Server>>(this, LoadServers);
            Messenger.Default.Register<string>(this, LoadLogData);
            Podaci = new ObservableCollection<TrougaoPodaci>();

        }



        private void LoadServers(ObservableCollection<Server> AllServers) { 
        
            Servers = new ObservableCollection<Server>(AllServers);

        }

        private bool isReading = false;
        private void LoadLogData(string line)
        {

            while (true)
            {
                if (line != null && isReading == false)
                {
                    isReading = true;
                    string[] parts = line.Split('|');
                    int id = Int32.Parse(parts[0]);
                    double value = Double.Parse(parts[1]);
                    DateTime time = DateTime.Parse(parts[2].Trim());
                    if (dataFromLog.ContainsKey(id))
                    {
                        dataFromLog[id].Add(new Tuple<double, DateTime>(value, time));
                    }
                    else
                    {
                        dataFromLog.Add(id, new ObservableCollection<Tuple<double, DateTime>>() { new Tuple<double, DateTime>(value, time) });
                    }
                    isReading = false;
                    break;
                }
            }

        }


        private void CreateGraph()
        {
            Podaci.Clear();
            if (dataFromLog.ContainsKey(SelectedServer.Id))
            {

                foreach(var item in dataFromLog[SelectedServer.Id])
                {
                    if(Podaci.Count() == 5)
                    {
                        break;
                    }

                    double visina = item.Item1;
                    Brush boja;
                    if (visina < 45 || visina > 75)
                    {
                        boja = Brushes.Red;
                    }
                    else
                    {
                        boja = Brushes.Blue;

                    }

                    //isao sam logikom ako mi je visina canvasa 650, a maks vrednost 100, onda 1 jedinica vrednosti = 6.5 visine pa sam hteo da mnozim sa 6.5 ali mi je bilo previse pa sam smanjio na 6.3
                    Podaci.Add(new TrougaoPodaci() { Visina = visina*6.3, Boja = boja});
                }

            }


        }

    }
}
