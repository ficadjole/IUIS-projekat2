using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkService.Helpers;

namespace NetworkService.Model
{

    public class Server : ValidationBase
    {
        private int id;
        private string name;
        private string ipAddress;
        private ServerType type;
        private int value;

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                if(id != value)
                {
                    id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string IpAddress
        {
            get
            {
                return ipAddress;
            }
            set
            {
                if (ipAddress != value)
                {
                    ipAddress = value;
                    OnPropertyChanged("IpAddress");
                }
            }
        }

        public ServerType Type
        {
            get
            {
                return type;
            }
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged("Type");
                }
            }
        }

        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged("Value");
                }
            }
        }

        protected override void ValidateSelf()
        {
            if(string.IsNullOrWhiteSpace(string.IsNullOrWhiteSpace(Name) ? "" : Name.Trim()))
            {
                this.ValidationErrors["Name"] = "Name cannot be empty.";
            }

            if (string.IsNullOrWhiteSpace(string.IsNullOrWhiteSpace(IpAddress) ? "" : IpAddress.Trim()))
            {
                this.ValidationErrors["IpAddress"] = "IP Address cannot be empty.";
            }
            else
            {
                System.Net.IPAddress ip;
                if (!System.Net.IPAddress.TryParse(IpAddress, out ip))
                {
                    this.ValidationErrors["IpAddress"] = "Invalid IP Address format.";
                }
            }

            if (Value < 0)
            {
                this.ValidationErrors["Value"] = "Value must be non-negative.";
            }


        }
    }
}
