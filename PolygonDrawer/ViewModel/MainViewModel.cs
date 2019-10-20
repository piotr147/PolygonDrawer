using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Security.Cryptography;
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
        private bool _isEdgeMoving;
        private Vertex _capturedVertex;
        private Edge _capturedEdge;
        private (int, int) _capturedPointOfEdge;
        private bool _deleteVertexNextMove;
        private Vertex _selectedVertex;
        private bool _isVertexMoving;
        private bool _addVertexNextMove;
        private bool _polygonMovingMode;
        private Polygon _capturedPolygon;
        private (int X, int Y) _capturedPointOfPoly;
        private bool _isPolygonMoving;

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

        public bool IsVertexMoving
        {
            get { return _isVertexMoving; }
            set { _isVertexMoving = value; RaisePropertyChanged(nameof(IsVertexMoving)); }
        }

        public bool IsEdgeMoving
        {
            get { return _isEdgeMoving; }
            set { _isEdgeMoving = value; RaisePropertyChanged(nameof(IsEdgeMoving)); }
        }

        public bool IsPolygonMoving
        {
            get { return _isPolygonMoving; }
            set { _isPolygonMoving = value; RaisePropertyChanged(nameof(IsPolygonMoving)); }
        }

        public bool PolygonMovingMode
        {
            get { return _polygonMovingMode; }
            set { _polygonMovingMode = value; RaisePropertyChanged(nameof(PolygonMovingMode)); }
        }

        public bool DeleteVertexNextMove
        {
            get { return _deleteVertexNextMove; }
            set { _deleteVertexNextMove = value; RaisePropertyChanged(nameof(DeleteVertexNextMove)); }
        }

        public bool AddVertexNextMove
        {
            get { return _addVertexNextMove; }
            set { _addVertexNextMove = value; RaisePropertyChanged(nameof(AddVertexNextMove)); }
        }

        public Vertex LastVertex
        {
            get { return _lastVertex; }
            set { _lastVertex = value; RaisePropertyChanged(nameof(LastVertex)); }
        }

        public Vertex CapturedVertex
        {
            get { return _capturedVertex; }
            set { _capturedVertex = value; RaisePropertyChanged(nameof(CapturedVertex)); }
        }

        public Vertex SelectedVertex
        {
            get { return _selectedVertex; }
            set { _selectedVertex = value; RaisePropertyChanged(nameof(SelectedVertex)); }
        }

        public Edge CapturedEdge
        {
            get { return _capturedEdge; }
            set { _capturedEdge = value; RaisePropertyChanged(nameof(CapturedEdge)); }
        }

        public Polygon CapturedPolygon
        {
            get { return _capturedPolygon; }
            set { _capturedPolygon = value; RaisePropertyChanged(nameof(CapturedPolygon)); }
        }

        public (int X, int Y) CapturedPointOfEdge
        {
            get { return _capturedPointOfEdge; }
            set { _capturedPointOfEdge = value; RaisePropertyChanged(nameof(CapturedPointOfEdge)); }
        }

        public (int X, int Y) CapturedPointOfPoly
        {
            get { return _capturedPointOfPoly; }
            set { _capturedPointOfPoly = value; RaisePropertyChanged(nameof(CapturedPointOfPoly)); }
        }

        public RelayCommand<Point> MouseUpOnBitmap
        {
            get;
            private set;
        }

        public RelayCommand<MouseData> MouseMoveOnBitmap
        {
            get;
            private set;
        }

        public RelayCommand ChangeMode
        {
            get;
            private set;
        }

        public RelayCommand DeleteVertex
        {
            get;
            private set;
        }

        public RelayCommand AddVertex
        {
            get;
            private set;
        }

        public RelayCommand MovePolygon
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



            MouseUpOnBitmap = new RelayCommand<Point>(BitmapUpClick, true);
            MouseMoveOnBitmap = new RelayCommand<MouseData>(BitmapMove, true);
            ChangeMode = new RelayCommand(() => { IsDrawingModeOn = !IsDrawingModeOn;
                IsMovingModeOn = !IsMovingModeOn;
            }, !DeleteVertexNextMove);
            DeleteVertex = new RelayCommand(() => { DeleteVertexNextMove = !DeleteVertexNextMove;});
            AddVertex = new RelayCommand(() => { AddVertexNextMove = !AddVertexNextMove; });
            MovePolygon = new RelayCommand(() => { PolygonMovingMode = !PolygonMovingMode; });
        }

        private void BitmapMove(MouseData obj)
        {
            int x = (int) obj.PosX;
            int y = (int) obj.PosY;

            if (IsVertexMoving)
            {
                if (x < 4 || x > Width - 4 || y < 4 || y > Height - 4)
                    return;

                CapturedVertex.X = x;
                CapturedVertex.Y = y;

                RefreshBitmap();
            }
            else if (IsEdgeMoving)
            {
                var v1 = CapturedEdge.V1;
                var v2 = CapturedEdge.V2;

                if (v1.X + x - CapturedPointOfEdge.X < 4 || v1.X + x - CapturedPointOfEdge.X > Width - 4
                                                         || v1.Y + y - CapturedPointOfEdge.Y < 4 ||
                                                         v1.Y + y - CapturedPointOfEdge.Y > Height - 4
                                                         || v2.X + x - CapturedPointOfEdge.X < 4 ||
                                                         v2.X + x - CapturedPointOfEdge.X > Width - 4
                                                         || v2.Y + y - CapturedPointOfEdge.Y < 4 ||
                                                         v2.Y + y - CapturedPointOfEdge.Y > Height - 4)
                    return;

                v1.X = v1.X + x - CapturedPointOfEdge.X;
                v1.Y = v1.Y + y - CapturedPointOfEdge.Y;
                v2.X = v2.X + x - CapturedPointOfEdge.X;
                v2.Y = v2.Y + y - CapturedPointOfEdge.Y;

                CapturedPointOfEdge = (x, y);

                RefreshBitmap();
            }
            else if(IsPolygonMoving)
            {
                var XMin = CapturedPolygon.Vertices[0].X;
                var XMax = CapturedPolygon.Vertices[0].X;
                var YMin = CapturedPolygon.Vertices[0].Y;
                var YMax = CapturedPolygon.Vertices[0].Y;

                for (int i = 0; i < CapturedPolygon.Vertices.Count; i++)
                {
                    if (CapturedPolygon.Vertices[i].X > XMax)
                        XMax = CapturedPolygon.Vertices[i].X;
                    if (CapturedPolygon.Vertices[i].X < XMin)
                        XMin = CapturedPolygon.Vertices[i].X;
                    if (CapturedPolygon.Vertices[i].Y > YMax)
                        YMax = CapturedPolygon.Vertices[i].Y;
                    if (CapturedPolygon.Vertices[i].Y > YMax)
                        YMax = CapturedPolygon.Vertices[i].Y;
                }

                if (XMin + x - CapturedPointOfPoly.X < 4)
                    return;
                if (YMin + y - CapturedPointOfPoly.Y < 4)
                    return;
                if (XMax + x - CapturedPointOfPoly.X > Width - 4)
                    return;
                if (YMax + y - CapturedPointOfPoly.Y > Height - 4)
                    return;


                for (int i = 0; i < CapturedPolygon.Vertices.Count; i++)
                {
                    CapturedPolygon.Vertices[i].X = CapturedPolygon.Vertices[i].X + x - CapturedPointOfPoly.X;
                    CapturedPolygon.Vertices[i].Y = CapturedPolygon.Vertices[i].Y + y - CapturedPointOfPoly.Y;
                }

                CapturedPointOfPoly = (x, y);

                RefreshBitmap();
            }

        }

        private void BitmapUpClick(Point p)
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
                        LastVertex.AddEdge(e);
                        v.AddEdge(e);
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
            else if (IsMovingModeOn)                            // Moving mode
            {
                var e = CheckIfOnExistingEdge(x, y);
                if (PolygonMovingMode)
                {
                    if(!IsPolygonMoving)
                    {
                        if(CheckIfOnExistingVertex(x, y))
                        {
                            var v = FindCapturedVertex(x, y);
                            CapturedPolygon = FindPolygonOfVertex(v);
                            CapturedPointOfPoly = (x, y);
                        }
                        else if (e != null)
                        {
                            CapturedPolygon = FindPolygonOfVertex(e.V1);
                            CapturedPointOfPoly = (x, y);
                        }

                        IsPolygonMoving = true;
                    }
                    else if(IsPolygonMoving)
                    {
                        IsPolygonMoving = false;
                    }
                }
                else if (CheckIfOnExistingVertex(x, y) && !IsVertexMoving )
                {
                    IsVertexMoving = true;
                    CapturedVertex = FindCapturedVertex(x, y);
                }
                else if (e != null && !IsEdgeMoving && !IsVertexMoving && !AddVertexNextMove)
                {
                    CapturedEdge = e;
                    IsEdgeMoving = true;
                    CapturedPointOfEdge = (x, y);
                }
                else if (IsVertexMoving)
                {
                    IsVertexMoving = false;
                    CapturedVertex = null;
                }
                else if (IsEdgeMoving)
                {
                    IsEdgeMoving = false;
                    CapturedEdge = null;
                }
                else if (DeleteVertexNextMove)
                {
                    if (CheckIfOnExistingVertex(x, y))
                    {
                        SelectedVertex = FindCapturedVertex(x, y);

                        if (FindPolygonOfVertex(SelectedVertex).Vertices.Count < 4)
                        {
                            SelectedVertex = null;
                            DeleteVertexNextMove = false;
                            return;
                        }

                        DeleteSelectedVertex(SelectedVertex);
                        SelectedVertex = null;

                        RefreshBitmap();
                    }

                    DeleteVertexNextMove = false;
                }
                else if (AddVertexNextMove)
                {
                    

                    if (e != null)
                    {
                        var poly = FindPolygonOfVertex(e.V1);
                        var v1 = e.V1;
                        var v2 = e.V2;
                        var index = poly.Vertices.IndexOf(e.V1) > poly.Vertices.IndexOf(e.V2) ?
                            poly.Vertices.IndexOf(e.V1) : poly.Vertices.IndexOf(e.V2);

                        var vnew = new Vertex((v1.X + v2.X) / 2, (v1.Y + v2.Y) / 2);

                        poly.Vertices.Insert(index + 1, vnew);
                        Vertices.Add(vnew);
                        Edges.Remove(e);
                        poly.Edges.Remove(e);

                        var e1 = new Edge(v1, vnew);
                        var e2 = new Edge(v2, vnew);

                        Edges.Add(e1);
                        Edges.Add(e2);
                        poly.Edges.Add(e1);
                        poly.Edges.Add(e2);


                        RefreshBitmap();
                    }

                    AddVertexNextMove = false;
                }
            }
            

        }

        private void DeleteSelectedVertex(Vertex selectedVertex)
        {
            var v1 = SelectedVertex.E1.V1 == selectedVertex ? SelectedVertex.E1.V2 : SelectedVertex.E1.V1;
            var v2 = SelectedVertex.E2.V1 == selectedVertex ? SelectedVertex.E2.V2 : SelectedVertex.E1.V1;

            var e1 = (v1.E1.V1 == v1 && v1.E1.V2 == selectedVertex) || (v1.E1.V2 == v1 && v1.E1.V1 == selectedVertex) ? v1.E1 : v1.E2;
            var e2 = (v2.E1.V1 == v1 && v2.E1.V2 == selectedVertex) || (v2.E1.V2 == v1 && v2.E1.V1 == selectedVertex) ? v2.E1 : v2.E2;

            var p = FindPolygonOfVertex(selectedVertex);

            var e = new Edge(v1, v2);
            v1.AddEdge(e);
            v2.AddEdge(e);

            Vertices.Remove(selectedVertex);
            Edges.Remove(e1);
            Edges.Remove(e2);

            Edges.Add(e);

            p.Vertices.Remove(selectedVertex);
            p.Edges.Remove(e1);
            p.Edges.Remove(e2);
            p.Edges.Add(e);

            e1 = null;
            e2 = null;
            selectedVertex = null;

        }

        private void FinishPolygonInProgress()
        {
            if (VerticesInProgress.Count < 3)
                return;
            var e = new Edge(LastVertex, FirstVertexOfPolygonInProgress);
            LastVertex.AddEdge(e);
            FirstVertexOfPolygonInProgress.AddEdge(e);
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

        private Edge CheckIfOnExistingEdge(int x, int y)
        {
            foreach (var e in Edges)
            {
                if (Drawer.BresenhamBool(Bitmap, e.V1.X, e.V1.Y, e.V2.X, e.V2.Y, x, y))
                    return e;
            }

            return null;
        }

        private Vertex FindCapturedVertex(int x, int y)
        {
            foreach (var v in Vertices)
            {
                if (x >= v.X - 2 && x <= v.X + 2 && y >= v.Y - 2 && y <= v.Y + 2)
                    return v;
            }

            return null;
        }

        private Polygon FindPolygonOfVertex(Vertex v)
        {
            foreach (var poly in Polygons)
            {
                foreach (var vert in poly.Vertices)
                {
                    if (vert == v)
                        return poly;
                }
            }

            return null;
        }

        private bool IsVertexInsideBitmap()
        {


            return true;
        }

        public void RefreshBitmap()
        {
            WriteableBitmap bitmap2 = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Bgr32, null);

            foreach (var poly in Polygons)
            {
                var firstV = poly.Vertices[0];
                for (int i = 0; i < poly.Vertices.Count; i++)
                {
                    Drawer.DrawVertex(bitmap2, poly.Vertices[i]);
                    Drawer.DrawEdge(bitmap2, poly.Vertices[i], poly.Vertices[(i + 1) % poly.Vertices.Count]);
                }
            }

            Bitmap = bitmap2;

        }
    }
}
