using MVVM1;
using MVVMLight.Messaging;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Type = NetworkService.Model.Type;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableBase
    {

        public ObservableCollection<ServersByType> SviServeri { get; set; }

        public ServersByType dataBase { get; set; }
        public ServersByType web { get; set; }
        public ServersByType file { get; set; }


        public NetworkDisplayViewModel()
        {
            
            dataBase = new ServersByType() { Servers = new ObservableCollection<Server>(), ServerType = new ServerType { Type = Type.DatabaseServer, UrlImage = @"\Resources\Images\DatabaseServer.png" } };
            web = new ServersByType() { Servers = new ObservableCollection<Server>(), ServerType = new ServerType { Type = Type.WebServer, UrlImage = @"\Resources\Images\WebServer.png" } };
            file = new ServersByType() { Servers = new ObservableCollection<Server>(), ServerType = new ServerType { Type = Type.FileServer, UrlImage = @"\Resources\Images\FileServer.png" } };
            SviServeri = new ObservableCollection<ServersByType>();
            Messenger.Default.Register<ObservableCollection<Server>>(this, LoadServers);
        }

        private void LoadServers(ObservableCollection<Server> collection)
        {


            for (int i = 0; i < collection.Count; i++) 
            {

                if (collection[i].Type.Type.Equals(Type.DatabaseServer))
                {
                    dataBase.Servers.Add(collection[i]);
                
                }else if(collection[i].Type.Type.Equals(Type.WebServer))
                {
                    web.Servers.Add(collection[i]);
                }
                else if (collection[i].Type.Type.Equals(Type.FileServer))
                {
                    file.Servers.Add(collection[i]);
                }


            }

            SviServeri.Add(dataBase);
            SviServeri.Add(web);
            SviServeri.Add(file);

        }
    }
}
