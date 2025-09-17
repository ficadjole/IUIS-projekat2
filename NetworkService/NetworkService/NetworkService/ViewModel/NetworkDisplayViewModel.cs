using MVVM1;
using MVVMLight.Messaging;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Type = NetworkService.Model.Type;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableBase
    {

        public ObservableCollection<ServersByType> SviServeri { get; set; }

        public ServersByType dataBase { get; set; }
        public ServersByType web { get; set; }
        public ServersByType file { get; set; }

        private bool isDragging = false;
        private Server draggedItem = null;
        private int draggedItemIndex = -1;

        public Server DraggedItem
        {
            get { return draggedItem; }
            set
            {
                if (draggedItem != value)
                {
                    draggedItem = value;
                    OnPropertyChanged("DraggedItem");
                }
            }
        }

        public MyICommand MouseLeftButtonUpCommand { get; set; }
        public MyICommand<object> SelectedItemChangedCommand { get; set; }
        public MyICommand<Point> CanvasDropCommand { get; set; }

        private ObservableCollection<Server> CanvasServers { get; set; }
        public NetworkDisplayViewModel()
        {
            
            dataBase = new ServersByType() { Servers = new ObservableCollection<Server>(), ServerType = new ServerType { Type = Type.DatabaseServer, UrlImage = @"\Resources\Images\DatabaseServer.png" } };
            web = new ServersByType() { Servers = new ObservableCollection<Server>(), ServerType = new ServerType { Type = Type.WebServer, UrlImage = @"\Resources\Images\WebServer.png" } };
            file = new ServersByType() { Servers = new ObservableCollection<Server>(), ServerType = new ServerType { Type = Type.FileServer, UrlImage = @"\Resources\Images\FileServer.png" } };
            SviServeri = new ObservableCollection<ServersByType>();
            CanvasServers = new ObservableCollection<Server>();
            Messenger.Default.Register<ObservableCollection<Server>>(this, LoadServers);
        
            MouseLeftButtonUpCommand = new MyICommand(OnMouseLeftButtonUp);
            SelectedItemChangedCommand = new MyICommand<object>(OnSelectedItemChanged);
            CanvasDropCommand = new MyICommand<Point>(OnCanvasDrop);

        }


        private void OnCanvasDrop(Point point)
        {
            if (DraggedItem != null)
            {
                // Dodaj item u Canvas kolekciju
                CanvasServers.Add(DraggedItem);

                // Ukloni item iz TreeView kolekcije
                foreach (var sbt in SviServeri)
                {
                    if (sbt.Servers.Contains(DraggedItem))
                    {
                        sbt.Servers.Remove(DraggedItem);
                        break;
                    }
                }

                DraggedItem = null; // reset
            }
        }

        private void OnSelectedItemChanged(object e)
        {
            var selectedItem = e as Server;
            if (!isDragging && selectedItem.GetType() != typeof(Server))
            {
                DraggedItem = selectedItem;
                isDragging = true;
                DraggedItem = selectedItem;
                draggedItemIndex = SviServeri.SelectMany(sbt => sbt.Servers).ToList().IndexOf(DraggedItem);
                    
            }
        }

        private void OnMouseLeftButtonUp()
        {
            ResetDragState();
        }

        private void ResetDragState()
        {
            isDragging = false;
            draggedItem = null;
            draggedItemIndex = -1;
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
