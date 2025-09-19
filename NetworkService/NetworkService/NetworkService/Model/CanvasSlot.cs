
using MVVM1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class CanvasSlot : BindableBase
    {
        public Server canvasServer { get; set; }
        public int row { get; set; }
        public int col { get; set; }

       public Server CanvasServer
        {
            get {  return canvasServer; }

            set
            {
                if (canvasServer != value)
                {
                    canvasServer = value;
                    OnPropertyChanged("CanvasServer");

                }
            }
        }

        public int Row
        {
            get { return  row; }
            set
            {
                if (row != value)
                {
                    row = value;
                    OnPropertyChanged("Row");
                }
            }
        }

        public int Col
        {
            get { return col; }
            set
            {
                if (col != value)
                {
                    col = value;
                    OnPropertyChanged("Col");
                }
            }
        }
    }
}
