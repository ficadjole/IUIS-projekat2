using MVVM1;
using MVVMLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public class HomeViewModel : BindableBase
    {

        public MyICommand<string> NavCommand { get; set; }

        public HomeViewModel homeViewModel;
        public NetworkDisplayViewModel networkDisplayViewModel;
        public NetworkEntitiesViewModel networkEntitiesViewModel;
        public MeasurementGraphViewModel measurementGraphViewModel;
        public BindableBase currentViewModel;
        public HomeViewModel(){

            NavCommand = new MyICommand<string>(OnNav);

            //homeViewModel = new HomeViewModel();
            networkDisplayViewModel = new NetworkDisplayViewModel();
            networkEntitiesViewModel = new NetworkEntitiesViewModel();
            measurementGraphViewModel = new MeasurementGraphViewModel();
            CurrentViewModel = this;
        }
        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set { SetProperty(ref currentViewModel, value); }
        }
        private void OnNav(string destView)
        {
            switch (destView)
            {
                case "entities":
                     CurrentViewModel = networkEntitiesViewModel;
                break;
                case "display":
                    CurrentViewModel = networkDisplayViewModel;
                break;
                case "graph":
                    CurrentViewModel = measurementGraphViewModel;
                break;
            }

            Messenger.Default.Send<BindableBase>(CurrentViewModel);
        }
    }
}
