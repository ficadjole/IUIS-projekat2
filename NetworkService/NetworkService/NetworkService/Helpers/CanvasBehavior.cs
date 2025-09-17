using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace NetworkService.Helpers
{
    public class CanvasBehavior : Behavior<Canvas>
    {
        public static readonly DependencyProperty DropCommandProperty =
       DependencyProperty.Register(nameof(DropCommand), typeof(ICommand), typeof(CanvasBehavior));

        public ICommand DropCommand
        {
            get => (ICommand)GetValue(DropCommandProperty);
            set => SetValue(DropCommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AllowDrop = true;
            AssociatedObject.Drop += AssociatedObject_Drop;
        }

        private void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            if (DropCommand != null && e.Data.GetDataPresent(typeof(Server)))
            {
                var canvas = sender as Canvas;
                var point = e.GetPosition(canvas);
                DropCommand.Execute(point);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Drop -= AssociatedObject_Drop;
        }

    }
}
