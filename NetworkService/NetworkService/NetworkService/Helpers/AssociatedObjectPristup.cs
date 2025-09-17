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
    public class AssociatedObjectPristup : TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(AssociatedObjectPristup), new PropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            var treeView = AssociatedObject as TreeView;

            if(treeView != null)
            {
                var selectedItem = treeView.SelectedItem;

                if(selectedItem != null)
                {
                    Command?.Execute(selectedItem);

                    DragDrop.DoDragDrop(treeView, selectedItem, DragDropEffects.Move | DragDropEffects.Copy);
                }

            }
        }
    }
}
