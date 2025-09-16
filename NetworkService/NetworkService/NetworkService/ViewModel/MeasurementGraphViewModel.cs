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
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NetworkService.ViewModel
{
    public class MeasurementGraphViewModel : BindableBase
    {
        public static ObservableCollection<Server> Servers { get; set; }

        public  ObservableCollection<TrougaoPodaci> podaci;

        public static Dictionary<int,ObservableCollection<Tuple<double, DateTime>>> dataFromLog;

        public Server selectedServer = null;

        public ObservableCollection<TrougaoPodaci> Podaci
        {
            get { return podaci; }
            set { SetProperty(ref podaci, value); }
        }

        public Server SelectedServer
        {
            get { return selectedServer; }
            set
            {
                SetProperty(ref selectedServer, value);
                if (selectedServer != null)
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

        //private bool isReading = false;
        //private void LoadLogData(string line)
        //{

        //    while (true)
        //    {
        //        if (line != null && isReading == false)
        //        {
        //            isReading = true;
        //            string[] parts = line.Split('|');
        //            int id = Int32.Parse(parts[0]);
        //            double value = Double.Parse(parts[1]);
        //            DateTime time = DateTime.Parse(parts[2].Trim());
        //            if (dataFromLog.ContainsKey(id))
        //            {
        //                dataFromLog[id].Add(new Tuple<double, DateTime>(value, time));
        //            }
        //            else
        //            {
        //                dataFromLog.Add(id, new ObservableCollection<Tuple<double, DateTime>>() { new Tuple<double, DateTime>(value, time) });
                        
        //            }

        //            if(SelectedServer != null && SelectedServer.Id == id)
        //            {
        //                CreateGraph();
        //            }
        //            isReading = false;
        //            break;
        //        }
        //    }

        //}

        private void LoadLogData(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return;

            string[] parts = line.Split('|');
            int id = int.Parse(parts[0]);
            double value = double.Parse(parts[1]);
            DateTime time = DateTime.Parse(parts[2].Trim());

            // Dodavanje u dataFromLog
            //Application.Current govori da se odnosi na trenutnu WPF aplikaciju
            //Dispathcer zna na kojem se thredu nesto izvrsava, i njega koristimo da bismo mogli koristiti
            //funkciju koja menja podatke na UI koji je zaseban thread sa podacim koje uzimamo iz druge niti tj u nasem slucaju one koja prima Messsenger
            //Invoke - je delegat i sluzi da sve sto se nalazi unutar {} da se izvrsi na UI niti, a ne van nje
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (dataFromLog.ContainsKey(id))
                {
                    dataFromLog[id].Add(new Tuple<double, DateTime>(value, time));
                }
                else
                {
                    dataFromLog.Add(id, new ObservableCollection<Tuple<double, DateTime>>() { new Tuple<double, DateTime>(value, time) });
                }
                // Ako je selektovan taj server, update graf
                if (SelectedServer != null && SelectedServer.Id == id)
                {
                    CreateGraph();
                }
            });
        }


        private void CreateGraph()
        {
            Podaci.Clear();
            if (dataFromLog.ContainsKey(SelectedServer.Id))
            {


                foreach (var item in dataFromLog[SelectedServer.Id].Reverse())
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

                    string vreme =  item.Item2.ToString("H:mm");

                    //isao sam logikom ako mi je visina canvasa 650, a maks vrednost 100, onda 1 jedinica vrednosti = 6.5 visine pa sam hteo da mnozim sa 6.5 ali mi je bilo previse pa sam smanjio na 6.3
                    Podaci.Add(new TrougaoPodaci() { Visina = visina*6.3, Boja = boja, Vreme = vreme});
                    
                }

            }


        }
    }
}
