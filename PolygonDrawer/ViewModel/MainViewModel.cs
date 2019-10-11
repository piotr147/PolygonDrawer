using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using PolygonDrawer.Converters;
using PolygonDrawer.Model;

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
        private ObservableCollection<Vertex> _vertices;
        private ObservableCollection<Edge> _edges;
        private bool _isFirstVertexOfEdge;
        private bool _isDrawingOn;
        private Vertex _lastVertex;

        public WriteableBitmap Bitmap
        {
            get { return _bitmap; }
            set { _bitmap = value; RaisePropertyChanged(nameof(Bitmap)); }
        }

        public ObservableCollection<Vertex> Vertices
        {
            get { return _vertices; }
            set { _vertices = value; RaisePropertyChanged((nameof(Vertices))); }
        }

        public ObservableCollection<Edge> Edges
        {
            get { return _edges; }
            set { _edges = value; RaisePropertyChanged((nameof(Vertices))); }
        }

        public bool IsFirstVertexOfEdge
        {
            get { return _isFirstVertexOfEdge; }
            set { _isFirstVertexOfEdge = value; RaisePropertyChanged(nameof(IsFirstVertexOfEdge)); }
        }

        public bool IsDrawingOn
        {
            get { return _isDrawingOn; }
            set { _isDrawingOn = value; RaisePropertyChanged(nameof(IsDrawingOn)); }
        }

        public Vertex LastVertex
        {
            get { return _lastVertex; }
            set { _lastVertex = value; RaisePropertyChanged(nameof(LastVertex)); }
        }

        public RelayCommand<Point> MouseClickedOnBitmap
        {
            get;
            private set;
        }



        public MainViewModel()
        {
            Width = Convert.ToInt32(ConfigurationSettings.AppSettings["bitmapWidth"]);
            Height = Convert.ToInt32(ConfigurationSettings.AppSettings["bitmapHeight"]);
            Bitmap = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Bgr32, null);
            IsFirstVertexOfEdge = true;
            IsDrawingOn = true;
            Vertices = new ObservableCollection<Vertex>();
            Edges = new ObservableCollection<Edge>();

            {
                //byte blue = 255;
                //byte green = 255;
                //byte red = 255;
                //byte alpha = 255;
                //byte[] colorData = { blue, green, red, alpha };
                //for (int i = 0; i < Width; i++)
                //{
                //    for (int j = 0; j < Height; j++)
                //    {
                //        Int32Rect rect = new Int32Rect(i, j, 1, 1);
                //        Bitmap.WritePixels(rect, colorData, 4, 0);
                //    }

                //}

                //DrawPoint(100, 50);
            }

            //Drawer.Bresenham(Bitmap, 100, 50, 200, 25);
            //Drawer.Bresenham(Bitmap, 100, 50, 200, 75);
            //Drawer.Bresenham(Bitmap, 500, 350, 700, 225);
            //Drawer.Bresenham(Bitmap, 100, 50, 200, 25);

            var v1 = new Vertex(200, 150);
            var v2 = new Vertex(300, 200);
            var e1 = new Edge(v1, v2);

            Drawer.DrawEdge(Bitmap, e1);

            MouseClickedOnBitmap = new RelayCommand<Point>(Bitmapclick, true);
        }


        private void Bitmapclick(Point p)
        {
            int x = (int)p.X;
            int y = (int)p.Y;
            if (IsDrawingOn)
            {
                if (!CheckIfOnExistingVertex(x, y))
                {
                    
                    var v = new Vertex(x, y);
                    Vertices.Add(v);
                    Drawer.DrawVertex(Bitmap, v);

                    if (!IsFirstVertexOfEdge)
                    {
                        var e = new Edge(LastVertex, v);
                        Edges.Add(e);
                        Drawer.DrawEdge(Bitmap, e);
                    }
                    else
                    {
                        IsFirstVertexOfEdge = false;
                    }

                    LastVertex = v;
                }



            }
        }


        private bool CheckIfOnExistingVertex(int x, int y)
        {
            foreach (var v in Vertices)
            {
                if ((x >= v.X - 1 && x <= v.X + 1) || (y >= v.Y - 1 && y <= v.Y + 1))
                    return true;
            }

            return false;
        }
    }
}
