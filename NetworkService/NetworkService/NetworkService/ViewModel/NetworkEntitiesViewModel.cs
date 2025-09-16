using MVVM1;
using MVVMLight.Messaging;
using NetworkService.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Type = NetworkService.Model.Type;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel : BindableBase
    {

        public static ObservableCollection<Server> Servers { get; set; }
        public static ObservableCollection<ServerType> EntityTypes { get; set; }

        private static ObservableCollection<Server> allServers;

        public Server newServer = new Server();

        public Server NewServer
        {
            get { return newServer; }
            set { SetProperty(ref newServer, value); }
        }

        private Server selectedEntity;

        public Server SelectedEntity
        {
            get { return selectedEntity; }
            set 
            { 
               if (selectedEntity != value)
                {
                    selectedEntity = value;
                    OnPropertyChanged("SelectedEntity");
                    DeleteEntitityCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private ServerType selectedTypeFilter;

        public ServerType SelectedTypeFilter
        {
            get { return selectedTypeFilter; }
            set
            {
                if (selectedTypeFilter != value)
                {
                    selectedTypeFilter = value;
                    OnPropertyChanged("SelectedTypeFilter");
                }
            }
        }

        private int idFilter;

        public int IdFilter
        {
            get { return idFilter; }
            set
            {
                if (idFilter != value)
                {
                    idFilter = value;
                    OnPropertyChanged("IdFilter");
                }
            }
        }

        private bool lowerFilter;
        public bool LowerFilter
        {
            get { return lowerFilter; }
            set
            {
                if (lowerFilter != value)
                {
                    lowerFilter = value;
                    OnPropertyChanged("LowerFilter");
                }
            }
        }

        private bool higherFilter;
        public bool HigherFilter
        {
            get { return higherFilter; }
            set
            {
                if (higherFilter != value)
                {
                    higherFilter = value;
                    OnPropertyChanged("HigherFilter");
                }
            }
        }

        private bool equalFilter;

        public bool EqualFilter
        {
            get { return equalFilter; }
            set
            {
                if (equalFilter != value)
                {
                    equalFilter = value;
                    OnPropertyChanged("EqualFilter");
                }
            }
        }

        public MyICommand AddEntity { get; set; }
        public MyICommand DeleteEntitityCommand { get; set; }

        public MyICommand SearchCommand { get; set; }

        public MyICommand ResetFilterCommand { get; set; }
        public NetworkEntitiesViewModel()
        {   
            LoadData();
            Messenger.Default.Send<int>(Servers.Count());

            AddEntity = new MyICommand(OnAdd);
            DeleteEntitityCommand = new MyICommand(OnDelete, CanDelete);
            SearchCommand = new MyICommand(OnSearch);
            ResetFilterCommand = new MyICommand(OnResetFilter);
            allServers = new ObservableCollection<Server>(Servers);

            Messenger.Default.Register<Tuple<int, int>>(this,updateValue);

            Messenger.Default.Send<ObservableCollection<Server>>(Servers);
        }

        private void updateValue(Tuple<int, int> item)
        {
            foreach (var server in Servers)
            {
                if (server.Id == item.Item1)
                {
                    server.Value = item.Item2;
                }
            }
        }
        private void OnResetFilter()
        {
            foreach (Server server in allServers)
            {
                if (!Servers.Contains(server))
                {
                    Servers.Add(server);
                }
            }

            SelectedTypeFilter = null;
            IdFilter = 0;
            LowerFilter = false;
            HigherFilter = false;
            EqualFilter = false;
        }

        private void OnSearch()
        {
            // Kreni uvek od svih servera
            var filteredList = allServers.ToList();

            // Filtriraj po tipu ako je izabran
            if (SelectedTypeFilter != null)
            {
                filteredList = filteredList.Where(s => s.Type.Type == SelectedTypeFilter.Type).ToList();
            }

            // Filtriraj po ID-u ako je unet
            if (IdFilter != 0)
            {
                if (LowerFilter)
                    filteredList = filteredList.Where(s => s.Id < IdFilter).ToList();
                else if (HigherFilter)
                    filteredList = filteredList.Where(s => s.Id > IdFilter).ToList();
                else if (EqualFilter)
                    filteredList = filteredList.Where(s => s.Id == IdFilter).ToList();
            }

            // Očisti trenutne servere i dodaj filtrirane
            Servers.Clear();
            foreach (var server in filteredList)
            {
                Servers.Add(server);
            }
        }

        private bool CanDelete()
        {
            return SelectedEntity != null;
        }

        private void OnDelete()
        {
            Servers.Remove(SelectedEntity);
        }

        private void OnAdd()
        {
            NewServer.Validate();

            if (NewServer.IsValid)
            {
                Server server = new Server
                {
                    Id = Servers.Count + 1,
                    Name = NewServer.Name,
                    IpAddress = NewServer.IpAddress,
                    Type = NewServer.Type,
                    Value = NewServer.Value
                };

                Servers.Add(server);

                // Reset the NewServer properties
                NewServer.Name = string.Empty;
                NewServer.IpAddress = string.Empty;
                NewServer.Type = null; // or any default value
                NewServer.Value = 0;
                Messenger.Default.Send<int>(Servers.Count());
                Messenger.Default.Send<ObservableCollection<Server>>(Servers);
            }

        }

        private void LoadData()
        {
            EntityTypes = new ObservableCollection<ServerType>();

            EntityTypes.Add(new ServerType { Type = Type.FileServer, UrlImage = @"\Resources\Images\FileServer.png" });
            EntityTypes.Add(new ServerType { Type = Type.DatabaseServer, UrlImage = @"\Resources\Images\DatabaseServer.png" });
            EntityTypes.Add(new ServerType { Type = Type.WebServer, UrlImage = @"\Resources\Images\WebServer.png" });


            Servers = new ObservableCollection<Server>();

            Servers.Add(new Server
            {
                Id = 0,
                Name = "Entity_0",
                IpAddress = "192.168.0.10",
                Type = new ServerType { Type = Type.FileServer, UrlImage = @"\Resources\Images\FileServer.png" },
                Value = 20
            });

            Servers.Add(new Server
            {
                Id = 1,
                Name = "Entity_1",
                IpAddress = "192.168.0.11",
                Type = new ServerType { Type = Type.DatabaseServer, UrlImage = @"\Resources\Images\DatabaseServer.png" },
                Value = 35
            });

            Servers.Add(new Server
            {
                Id = 2,
                Name = "Entity_2",
                IpAddress = "192.168.0.12",
                Type = new ServerType { Type = Type.WebServer, UrlImage = @"\Resources\Images\WebServer.png" },
                Value = 50
            });

            Servers.Add(new Server
            {
                Id = 3,
                Name = "Entity_3",
                IpAddress = "192.168.0.13",
                Type = new ServerType { Type = Type.FileServer, UrlImage = @"\Resources\Images\FileServer.png" },
                Value = 15
            });

            Servers.Add(new Server
            {
                Id = 4,
                Name = "Entity_4",
                IpAddress = "192.168.0.14",
                Type = new ServerType { Type = Type.DatabaseServer, UrlImage = @"\Resources\Images\DatabaseServer.png" },
                Value = 60
            });

            Servers.Add(new Server
            {
                Id = 5,
                Name = "Entity_5",
                IpAddress = "192.168.0.15",
                Type = new ServerType { Type = Type.WebServer, UrlImage = @"\Resources\Images\WebServer.png" },
                Value = 42
            });

            Servers.Add(new Server
            {
                Id = 6,
                Name = "Entity_6",
                IpAddress = "192.168.0.16",
                Type = new ServerType { Type = Type.FileServer, UrlImage = @"\Resources\Images\FileServer.png" },
                Value = 10
            });

            Servers.Add(new Server
            {
                Id = 7,
                Name = "Entity_7",
                IpAddress = "192.168.0.17",
                Type = new ServerType { Type = Type.DatabaseServer, UrlImage = @"\Resources\Images\DatabaseServer.png" },
                Value = 77
            });

            Servers.Add(new Server
            {
                Id = 8,
                Name = "Entity_8",
                IpAddress = "192.168.0.18",
                Type = new ServerType { Type = Type.WebServer, UrlImage = @"\Resources\Images\WebServer.png" },
                Value = 31
            });

            Servers.Add(new Server
            {
                Id = 9,
                Name = "Entity_9",
                IpAddress = "192.168.0.19",
                Type = new ServerType { Type = Type.FileServer, UrlImage = @"\Resources\Images\FileServer.png" },
                Value = 28
            });

            Servers.Add(new Server
            {
                Id = 10,
                Name = "Entity_10",
                IpAddress = "192.168.0.20",
                Type = new ServerType { Type = Type.DatabaseServer, UrlImage = @"\Resources\Images\DatabaseServer.png" },
                Value = 55
            });

            Servers.Add(new Server
            {
                Id = 12,
                Name = "Entity_11",
                IpAddress = "192.168.0.21",
                Type = new ServerType { Type = Type.WebServer, UrlImage = @"\Resources\Images\WebServer.png" },
                Value = 18
            });

            Servers.Add(new Server
            {
                Id = 12,
                Name = "Entity_12",
                IpAddress = "192.168.0.22",
                Type = new ServerType { Type = Type.FileServer, UrlImage = @"\Resources\Images\FileServer.png" },
                Value = 63
            });

            Servers.Add(new Server
            {
                Id = 13,
                Name = "Entity_13",
                IpAddress = "192.168.0.23",
                Type = new ServerType { Type = Type.DatabaseServer, UrlImage = @"\Resources\Images\DatabaseServer.png" },
                Value = 47
            });

            Servers.Add(new Server
            {
                Id = 14,
                Name = "Entity_14",
                IpAddress = "192.168.0.24",
                Type = new ServerType { Type = Type.WebServer, UrlImage = @"\Resources\Images\WebServer.png" },
                Value = 39
            });


        }


    }
}
