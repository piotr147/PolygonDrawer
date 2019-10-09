using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Configuration;

namespace PolygonDrawer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region window parameters
        private int _height;
        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                RaisePropertyChanged("Height");
            }
        }
        private int _width;
        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                RaisePropertyChanged("Width");
            }
        }
        #endregion
        private WriteableBitmap _bitmap;

        public WriteableBitmap Bitmap
        {
            get
            {
                return _bitmap;
            }
            set
            {
                _bitmap = value;
                RaisePropertyChanged(nameof(Bitmap));
            }
        }

        public RelayCommand<MouseEventArgs> MouseClickedOnBitmap
        {
            get;
            private set;
        }

        public MainViewModel()
        {
            Width = Convert.ToInt32(ConfigurationSettings.AppSettings["bitmapWidth"]);
            Height = Convert.ToInt32(ConfigurationSettings.AppSettings["bitmapHeight"]);
            Bitmap = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Bgr32, null);


            byte blue = 255;
            byte green = 255;
            byte red = 255;
            byte alpha = 255;
            byte[] colorData = { blue, green, red, alpha };
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Int32Rect rect = new Int32Rect(i, j, 1, 1);
                    Bitmap.WritePixels(rect, colorData, 4, 0);
                }
                
            }

            DrawPoint(50, 50);


            MouseClickedOnBitmap = new RelayCommand<MouseEventArgs>(
                Bitmapclick
                , true);
        }

        
        private void Bitmapclick(MouseEventArgs e)
        {
            DrawPoint(e.GetPosition());
            
        }


        public void DrawPoint(int x, int y)
        {
            byte blue = 0;
            byte green = 0;
            byte red = 255;
            byte alpha = 255;
            byte[] colorData = { blue, green, red, alpha };
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Int32Rect rect = new Int32Rect(x + i, y + j, 1, 1);
                    Bitmap.WritePixels(rect, colorData, 4, 0);
                }

            }
        }

        public void DrawPoint(Point p)
        {
            byte blue = 0;
            byte green = 0;
            byte red = 255;
            byte alpha = 255;
            byte[] colorData = { blue, green, red, alpha };
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Int32Rect rect = new Int32Rect((int)p.X + i, (int)p.Y + j, 1, 1);
                    Bitmap.WritePixels(rect, colorData, 4, 0);
                }

            }
        }

    }
}
