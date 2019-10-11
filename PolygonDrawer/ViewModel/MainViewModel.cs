﻿using System;
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
        private bool _isDrawingModeOn;
        private Vertex _lastVertex;
        private ObservableCollection<Polygon> _polygons;
        private ObservableCollection<Vertex> _verticesInProgress;
        private ObservableCollection<Edge> _edgesInProgress;
        private Vertex _firstVertexOfPolygonInProgress;
        private bool _isMovingModeOn;

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

        public ObservableCollection<Polygon> Polygons
        {
            get { return _polygons; }
            set { _polygons = value; RaisePropertyChanged(nameof(Polygons)); }
        }

        public ObservableCollection<Vertex> VerticesInProgress
        {
            get { return _verticesInProgress; }
            set { _verticesInProgress = value; RaisePropertyChanged(nameof(VerticesInProgress)); }
        }

        public ObservableCollection<Edge> EdgesInProgress
        {
            get { return _edgesInProgress; }
            set { _edgesInProgress = value; RaisePropertyChanged(nameof(Edges)); }
        }

        public Vertex FirstVertexOfPolygonInProgress
        {
            get { return _firstVertexOfPolygonInProgress; }
            set { _firstVertexOfPolygonInProgress = value; RaisePropertyChanged(nameof(FirstVertexOfPolygonInProgress)); }
        }

        public bool IsFirstVertexOfEdge
        {
            get { return _isFirstVertexOfEdge; }
            set { _isFirstVertexOfEdge = value; RaisePropertyChanged(nameof(IsFirstVertexOfEdge)); }
        }

        public bool IsDrawingModeOn
        {
            get { return _isDrawingModeOn; }
            set { _isDrawingModeOn = value; RaisePropertyChanged(nameof(IsDrawingModeOn)); }
        }

        public bool IsMovingModeOn
        {
            get { return _isMovingModeOn; }
            set { _isMovingModeOn = value; RaisePropertyChanged(nameof(IsMovingModeOn)); }
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
            IsDrawingModeOn = true;
            Vertices = new ObservableCollection<Vertex>();
            Edges = new ObservableCollection<Edge>();
            Polygons = new ObservableCollection<Polygon>();
            VerticesInProgress = new ObservableCollection<Vertex>();
            EdgesInProgress = new ObservableCollection<Edge>();

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


            MouseClickedOnBitmap = new RelayCommand<Point>(Bitmapclick, true);
        }


        private void Bitmapclick(Point p)
        {
            int x = (int)p.X;
            int y = (int)p.Y;
            if (IsDrawingModeOn)
            {
                if (FirstVertexOfPolygonInProgress != null 
                    && x >= FirstVertexOfPolygonInProgress.X - 4 && x <= FirstVertexOfPolygonInProgress.X + 4 
                    && y >= FirstVertexOfPolygonInProgress.Y - 4 && y <= FirstVertexOfPolygonInProgress.Y + 4)
                {
                    FinishPolygonInProgress();
                }
                else if (!CheckIfOnExistingVertex(x, y))
                {
                    
                    var v = new Vertex(x, y);
                    Vertices.Add(v);
                    VerticesInProgress.Add(v);
                    Drawer.DrawVertex(Bitmap, v);
                    if (FirstVertexOfPolygonInProgress != null)
                    {
                        
                    
                        var e = new Edge(LastVertex, v);
                        Edges.Add(e);
                        EdgesInProgress.Add(e);
                        Drawer.DrawEdge(Bitmap, e);
                    }
                    else
                    {
                        IsFirstVertexOfEdge = false;
                        FirstVertexOfPolygonInProgress = v;
                    }

                    LastVertex = v;
                }
            }
        }

        private void FinishPolygonInProgress()
        {
            if (VerticesInProgress.Count < 3)
                return;
            var e = new Edge(LastVertex, FirstVertexOfPolygonInProgress);
            Drawer.DrawEdge(Bitmap, e);
            EdgesInProgress.Add(e);
            Edges.Add(e);

            var poly = new Polygon(VerticesInProgress, EdgesInProgress);
            Polygons.Add(poly);
            FirstVertexOfPolygonInProgress = null;
            VerticesInProgress = new ObservableCollection<Vertex>();
            EdgesInProgress = new ObservableCollection<Edge>();
        }

        private bool CheckIfOnExistingVertex(int x, int y)
        {
            foreach (var v in Vertices)
            {
                if (x >= v.X - 2 && x <= v.X + 2 && y >= v.Y - 2 && y <= v.Y + 2)
                    return true;
            }

            return false;
        }
    }
}
