using MVVM1;
using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public enum Type
    {
        WebServer,
        FileServer,
        DatabaseServer
    }
    public class ServerType : BindableBase
    {
        public Type type;
        public string urlImage;

        public Type Type
        {
            get { return type; }
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged("Type");
                }
            }
        }

        public string UrlImage
        {
            get { return urlImage; }
            set
            {
                if (urlImage != value)
                {
                    urlImage = value;
                    OnPropertyChanged("UrlImage");
                }
            }
        }

    }
}
