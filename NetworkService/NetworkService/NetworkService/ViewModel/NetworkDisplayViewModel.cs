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
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        
        public MyICommand<object> DragOverCommand { get; set; }

        public MyICommand<object> DropCommand { get; set; }

        private ObservableCollection<Server> CanvasServers { get; set; }
        public NetworkDisplayViewModel()
        {
            
            dataBase = new ServersByType() { Servers = new ObservableCollection<Server>(), ServerType = new ServerType { Type = Type.DatabaseServer, UrlImage = @"..\..\Resources\Images\DatabaseServer.png" } };
            web = new ServersByType() { Servers = new ObservableCollection<Server>(), ServerType = new ServerType { Type = Type.WebServer, UrlImage = @"..\..\Resources\Images\WebServer.png" } };
            file = new ServersByType() { Servers = new ObservableCollection<Server>(), ServerType = new ServerType { Type = Type.FileServer, UrlImage = @"..\..\Resources\Images\FileServer.png" } };
            SviServeri = new ObservableCollection<ServersByType>();
            CanvasServers = new ObservableCollection<Server>();
            Messenger.Default.Register<ObservableCollection<Server>>(this, LoadServers);
        
            MouseLeftButtonUpCommand = new MyICommand(OnMouseLeftButtonUp);
            SelectedItemChangedCommand = new MyICommand<object>(OnSelectedItemChanged);
            DragOverCommand = new MyICommand<object>(OnDragOverCommand);
            DropCommand = new MyICommand<object>(OnDropCommand);

        }

        private void OnDropCommand(object obj)
        {
            Canvas canvas = (obj as DragEventArgs).Source as Canvas;
            TextBlock textBlock = (TextBlock)(canvas.Children[0]);

            if(DraggedItem != null)
            {
                if (canvas.Resources["taken"] == null)
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(DraggedItem.Type.UrlImage, UriKind.Relative);
                    image.EndInit();
                    canvas.Background = new ImageBrush(image);
                    textBlock.Text = DraggedItem.Name +"-"+ DraggedItem.Value;
                    canvas.Resources.Add("token", true);
                    CanvasServers.Add(DraggedItem);
                    
                    //Dodati brisanje iz liste

                }
                ResetDragState();
            }

            (obj as DragEventArgs).Handled = true;
        }

        private void OnDragOverCommand(object obj)
        {
            Canvas canvas = (obj as DragEventArgs).Source as Canvas;

            if(canvas != null)
            {
                if (canvas.Resources["taken"] != null)
                {
                    (obj as DragEventArgs).Effects = DragDropEffects.None;
                }
                else
                {
                    (obj as DragEventArgs).Effects = DragDropEffects.Move;
                }

                (obj as DragEventArgs).Handled = true;
            }


        }

        private void OnSelectedItemChanged(object e)
        {
            var selectedItem = (e as RoutedPropertyChangedEventArgs<object>).NewValue as Server;
            var treeView = (e as RoutedPropertyChangedEventArgs<object>).Source as TreeView;
            if (selectedItem != null) {
                if (!isDragging && selectedItem.GetType() == typeof(Server))
                {
                    isDragging = true;
                    DraggedItem = selectedItem;
                    draggedItemIndex = SviServeri.SelectMany(sbt => sbt.Servers).ToList().IndexOf(DraggedItem);
                    DragDrop.DoDragDrop(treeView, DraggedItem, DragDropEffects.Move | DragDropEffects.Copy);

                }
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
