using MVVM1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class ServersByType : BindableBase
    {
        private ServerType serverType;
        private ObservableCollection<Server> servers;

        public ServerType ServerType
        {
            get { return serverType; }
            set
            {
                if (serverType != value)
                {
                    serverType = value;
                    OnPropertyChanged("ServerType");
                }
            }
        }

        public ObservableCollection<Server> Servers
        {
            get { return servers; }
            set
            {
                if (servers != value)
                {
                    servers = value;
                    OnPropertyChanged("Servers");
                }
            }
        }
    }
}
