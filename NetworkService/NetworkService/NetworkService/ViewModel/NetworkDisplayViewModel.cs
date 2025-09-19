using MVVM1;
using MVVMLight.Messaging;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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

        public MyICommand<object> TreeViewDropCommand { get; set; }

        public MyICommand<object> TreeViewDragOverCommand { get; set; }



        public MyICommand<object> DragOverCommand { get; set; }

        public MyICommand<object> DropCommand { get; set; }

        public MyICommand<object> MouseLeftButtonDownCommand { get; set; }
       
        public ObservableCollection<Server> CanvasSlots { get; set; }
        
        public NetworkDisplayViewModel()
        {
            
            dataBase = new ServersByType() { Servers = new ObservableCollection<Server>(), ServerType = new ServerType { Type = Type.DatabaseServer, UrlImage = @"..\..\Resources\Images\DatabaseServer.png" } };
            web = new ServersByType() { Servers = new ObservableCollection<Server>(), ServerType = new ServerType { Type = Type.WebServer, UrlImage = @"..\..\Resources\Images\WebServer.png" } };
            file = new ServersByType() { Servers = new ObservableCollection<Server>(), ServerType = new ServerType { Type = Type.FileServer, UrlImage = @"..\..\Resources\Images\FileServer.png" } };
            SviServeri = new ObservableCollection<ServersByType>();
            CanvasSlots = new ObservableCollection<Server>();
            LoadCanvas();
            Messenger.Default.Register<ObservableCollection<Server>>(this, LoadServers);

            MouseLeftButtonUpCommand = new MyICommand(OnMouseLeftButtonUp);
            SelectedItemChangedCommand = new MyICommand<object>(OnSelectedItemChanged);
            TreeViewDropCommand = new MyICommand<object>(OnTreeViewDropCommand);
            TreeViewDragOverCommand = new MyICommand<object>(OnTreeViewDragOverCommand);


            DragOverCommand = new MyICommand<object>(OnDragOverCommand);
            DropCommand = new MyICommand<object>(OnDropCommand);
            MouseLeftButtonDownCommand = new MyICommand<object>(MouseLeftButtonDownMove);

        }

        private void OnTreeViewDragOverCommand(object obj)
        {
            var nesto = obj;
        }

        private void OnTreeViewDropCommand(object obj)
        {
            if (DraggedItem.Type.Type.Equals(Type.DatabaseServer))
            {
                if (SviServeri[0].Servers.Contains(DraggedItem) == false)
                {
                    SviServeri[0].Servers.Add(DraggedItem);
                    
                    CanvasSlots.Remove(DraggedItem);
                }
            }
            else if (DraggedItem.Type.Type.Equals(Type.WebServer))
            {
                if (SviServeri[1].Servers.Contains(DraggedItem) == false)
                {
                    SviServeri[1].Servers.Add(DraggedItem);
                    CanvasSlots.Remove(DraggedItem);
                }
            }
            else if (DraggedItem.Type.Type.Equals(Type.FileServer))
            {
                if (SviServeri[2].Servers.Contains(DraggedItem) == false)
                {
                    SviServeri[2].Servers.Add(DraggedItem);
                    CanvasSlots.Remove(DraggedItem);
                }
            }

            ResetDragState();
        }

        private void MouseLeftButtonDownMove(object obj)
        {
            if (obj != null)
            {
                MouseButtonEventArgs e = (MouseButtonEventArgs)obj;

                Canvas canvas = (((obj as MouseEventArgs).Source as Image).Parent as StackPanel).Parent as Canvas;

                if (canvas is Canvas && canvas.Tag is Server server)
                {
                    DraggedItem = server;
                    DragDrop.DoDragDrop(canvas, server, DragDropEffects.Move);
                    ResetDataCanvas(canvas);
                    CanvasSlots.Remove(server);
                }

            }

        }

        private void ResetDataCanvas(Canvas canvas)
        {
            if (canvas != null) {

                canvas.DataContext = null;  
                canvas.Resources.Remove("taken");

            }
        }
        
        private Server nekiServer;

        public Server NekiServer
        {
            get { return nekiServer; }
            set
            {
                if (nekiServer != value)
                {
                    nekiServer = value;
                    OnPropertyChanged("NekiServer");
                }
            }
        }

        private void OnDropCommand(object obj)
        {
            Canvas canvas = (obj as DragEventArgs).Source as Canvas;

            if (DraggedItem != null)
            {
                if (canvas.Resources["taken"] == null)
                {
                    canvas.Resources.Add("taken", true);
                    RemoveItemFromCollection(DraggedItem);
                    NekiServer = DraggedItem;
                    //canvas.DataContext = NekiServer;
                   
                    CanvasSlots.Add(DraggedItem);

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
                    draggedItemIndex = FindIndexOfSelectedServer(DraggedItem);
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

        private void RemoveItemFromCollection(Server movedServer)
        {

            if (movedServer != null) {

                if (movedServer.Type.Type.Equals(Type.DatabaseServer))
                {

                    if (SviServeri[0].Servers.Contains(movedServer)) {

                    SviServeri[0].Servers.RemoveAt(draggedItemIndex);
                       
                    }



                }
                else if (movedServer.Type.Type.Equals(Type.WebServer))
                {
                    if (SviServeri[1].Servers.Contains(movedServer))
                    {


                    SviServeri[1].Servers.RemoveAt(draggedItemIndex);
                    }
                }
                else
                {
                    if (SviServeri[2].Servers.Contains(movedServer))
                    {

                    SviServeri[2].Servers.RemoveAt(draggedItemIndex);

                    }
                }
            
            }

        }

        private int FindIndexOfSelectedServer(Server movedServer)
        {
            int index = -1;

            if (movedServer != null)
            {

                if (movedServer.Type.Type.Equals(Type.DatabaseServer))
                {

                    index = SviServeri[0].Servers.IndexOf(movedServer);

                }
                else if (movedServer.Type.Type.Equals(Type.WebServer))
                {
                    index = SviServeri[1].Servers.IndexOf(movedServer);
                }
                else
                {
                    index = SviServeri[2].Servers.IndexOf(movedServer);
                }

            }

            return index;
        }

        private void LoadCanvas()
        {
            for (int i = 0; i < 12; i++)
            {
                CanvasSlots.Add(null); // prazni slotovi
            }
        }

        private void LoadServers(ObservableCollection<Server> collection)
        {
            SviServeri.Clear();
            dataBase.Servers.Clear();
            web.Servers.Clear();
            file.Servers.Clear();

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
