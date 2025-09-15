using MVVM1;
using System;
using MVVMLight.Messaging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using NetworkService.Model;
using System.Data.SqlTypes;
using System.IO;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private int count; // Inicijalna vrednost broja objekata u sistemu
                                // ######### ZAMENITI stvarnim brojem elemenata
                                //           zavisno od broja entiteta u list

        public MyICommand HomeCommand { get; set; }
        public HomeViewModel homeViewModel;
        public BindableBase currentViewModel;
        
        public MainWindowViewModel()
        {
            
            createListener(); //Povezivanje sa serverskom aplikacijom
            Messenger.Default.Register<int>(this, SetCount);//ovo sam satvio ovde gore da bi se prvo uradila registracija pa tek onda da se posalje poruka iz NetworkEntitiesViewModel
            HomeCommand = new MyICommand (OnHomeCommand);

            homeViewModel = new HomeViewModel();
            SetCurrentView(homeViewModel);
            Messenger.Default.Register<BindableBase>(this, SetCurrentView);
           
        }

        private void SetCurrentView(BindableBase viewModel)
        {
             this.CurrentViewModel = viewModel;
        }
        private void SetCount(int cnt)
        {
            this.count = cnt;
        }

        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set { SetProperty(ref currentViewModel, value); }
        }

        private void OnHomeCommand()
        {
            SetCurrentView(homeViewModel);
        }

        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25675);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            Console.WriteLine("Message received: {0}", count.ToString());
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji

                            string[] parts = incomming.Split(':');
                            string name = parts[0];
                            int value = Int32.Parse(parts[1]); // Vrednost koju nam je poslao server
                            int id = Int32.Parse(name.Split('_')[1]); // Izdvajanje ID-a iz imena entiteta

                            string filePath = @"C:\Users\Filip\Desktop\IUIS-projekat2\NetworkService\NetworkService\NetworkService\Resources\Files\log.txt";

                            // Upis u log fajl
                            string line = id + "|" + value + "|" + DateTime.Now.ToString("HH:mm");

                            using (StreamWriter write = new StreamWriter(filePath, true))
                            {
                                write.WriteLine(line);
                            }

                            // Slanje poruke odgovarajućem ViewModel-u da ažurira stanje entiteta
                            Messenger.Default.Send<Tuple<int, int>>(new Tuple<int, int>(id, value));
                            Messenger.Default.Send<string>(line); // Slanje linije za log fajl
                        }
                    }, null);
                }
            });
            
            listeningThread.IsBackground = true;
            listeningThread.Start();
        }
    }
}
