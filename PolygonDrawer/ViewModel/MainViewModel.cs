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
using System.Windows.Media.Animation;
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

        private List<(int r, int g, int b)> colors;

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
        private bool _settingEqRelMode;
        private Edge _eqRelE1;
        private Edge _eqRelE2;
        private bool _settingParRelMode;
        private Edge _parRelE1;
        private Edge _parRelE2;
        private int _relationCounter;
        private bool _removeRelationMode;

        public int RelationCounter
        {
            get { return _relationCounter; }
            set { _relationCounter = value; RaisePropertyChanged(nameof(RelationCounter)); }
        }
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

        public bool RemoveRelationMode
        {
            get { return _removeRelationMode; }
            set { _removeRelationMode = value; RaisePropertyChanged(nameof(RemoveRelationMode));
                RaisePropertyChanged(nameof(SettingParRelMode));
                RaisePropertyChanged(nameof(DeleteVertexButtonEnabled));
                RaisePropertyChanged(nameof(AddVertexButtonEnabled));
                RaisePropertyChanged(nameof(MovePolygonButtonEnabled));
                RaisePropertyChanged(nameof(SetEqualLengthButtonEnabled));
                RaisePropertyChanged(nameof(SetParallelButtonEnabled));
                RaisePropertyChanged(nameof(ModeButtonEnabled));
            }
        }

        public bool IsMovingModeOn
        {
            get { return _isMovingModeOn; }
            set { _isMovingModeOn = value; RaisePropertyChanged(nameof(IsMovingModeOn));
                RaisePropertyChanged(nameof(DeleteVertexButtonEnabled));
                RaisePropertyChanged(nameof(AddVertexButtonEnabled));
                RaisePropertyChanged(nameof(MovePolygonButtonEnabled));
                RaisePropertyChanged(nameof(SetEqualLengthButtonEnabled));
                RaisePropertyChanged(nameof(SetParallelButtonEnabled));
                RaisePropertyChanged(nameof(RemoveRelationButtonEnabled));
            }
        }

        public bool IsVertexMoving
        {
            get { return _isVertexMoving; }
            set { _isVertexMoving = value; RaisePropertyChanged(nameof(IsVertexMoving));
                RaisePropertyChanged(nameof(DeleteVertexButtonEnabled));
                RaisePropertyChanged(nameof(AddVertexButtonEnabled));
                RaisePropertyChanged(nameof(MovePolygonButtonEnabled));
                RaisePropertyChanged(nameof(SetEqualLengthButtonEnabled));
                RaisePropertyChanged(nameof(SetParallelButtonEnabled));
                RaisePropertyChanged(nameof(ModeButtonEnabled));
                RaisePropertyChanged(nameof(RemoveRelationButtonEnabled));
            }
        }

        public bool IsEdgeMoving
        {
            get { return _isEdgeMoving; }
            set { _isEdgeMoving = value; RaisePropertyChanged(nameof(IsEdgeMoving));
                RaisePropertyChanged(nameof(DeleteVertexButtonEnabled));
                RaisePropertyChanged(nameof(AddVertexButtonEnabled));
                RaisePropertyChanged(nameof(MovePolygonButtonEnabled));
                RaisePropertyChanged(nameof(SetEqualLengthButtonEnabled));
                RaisePropertyChanged(nameof(SetParallelButtonEnabled));
                RaisePropertyChanged(nameof(ModeButtonEnabled));
                RaisePropertyChanged(nameof(RemoveRelationButtonEnabled));
            }
        }

        public bool IsPolygonMoving
        {
            get { return _isPolygonMoving; }
            set { _isPolygonMoving = value; RaisePropertyChanged(nameof(IsPolygonMoving));
                RaisePropertyChanged(nameof(DeleteVertexButtonEnabled));
                RaisePropertyChanged(nameof(AddVertexButtonEnabled));
                RaisePropertyChanged(nameof(SetEqualLengthButtonEnabled));
                RaisePropertyChanged(nameof(SetParallelButtonEnabled));
                RaisePropertyChanged(nameof(ModeButtonEnabled));
                RaisePropertyChanged(nameof(RemoveRelationButtonEnabled));
            }
        }

        public bool PolygonMovingMode
        {
            get { return _polygonMovingMode; }
            set { _polygonMovingMode = value; RaisePropertyChanged(nameof(PolygonMovingMode));
                RaisePropertyChanged(nameof(DeleteVertexButtonEnabled));
                RaisePropertyChanged(nameof(AddVertexButtonEnabled));
                RaisePropertyChanged(nameof(SetEqualLengthButtonEnabled));
                RaisePropertyChanged(nameof(SetParallelButtonEnabled));
                RaisePropertyChanged(nameof(ModeButtonEnabled));
                RaisePropertyChanged(nameof(RemoveRelationButtonEnabled));
            }
        }

        public bool SettingEqRelMode
        {
            get { return _settingEqRelMode; }
            set { _settingEqRelMode = value; RaisePropertyChanged(nameof(SettingEqRelMode));
                RaisePropertyChanged(nameof(DeleteVertexButtonEnabled));
                RaisePropertyChanged(nameof(AddVertexButtonEnabled));
                RaisePropertyChanged(nameof(MovePolygonButtonEnabled));
                RaisePropertyChanged(nameof(SetParallelButtonEnabled));
                RaisePropertyChanged(nameof(ModeButtonEnabled));
                RaisePropertyChanged(nameof(RemoveRelationButtonEnabled));
            }
        }

        public bool SettingParRelMode
        {
            get { return _settingParRelMode; }
            set { _settingParRelMode = value; RaisePropertyChanged(nameof(SettingParRelMode));
                RaisePropertyChanged(nameof(DeleteVertexButtonEnabled));
                RaisePropertyChanged(nameof(AddVertexButtonEnabled));
                RaisePropertyChanged(nameof(MovePolygonButtonEnabled));
                RaisePropertyChanged(nameof(SetEqualLengthButtonEnabled));
                RaisePropertyChanged(nameof(ModeButtonEnabled));
                RaisePropertyChanged(nameof(RemoveRelationButtonEnabled));
            }
        }

        public bool DeleteVertexNextMove
        {
            get { return _deleteVertexNextMove; }
            set { _deleteVertexNextMove = value; RaisePropertyChanged(nameof(DeleteVertexNextMove));
                RaisePropertyChanged(nameof(AddVertexButtonEnabled));
                RaisePropertyChanged(nameof(MovePolygonButtonEnabled));
                RaisePropertyChanged(nameof(SetEqualLengthButtonEnabled));
                RaisePropertyChanged(nameof(SetParallelButtonEnabled));
                RaisePropertyChanged(nameof(ModeButtonEnabled));
                RaisePropertyChanged(nameof(RemoveRelationButtonEnabled));
            }
        }

        public bool AddVertexNextMove
        {
            get { return _addVertexNextMove; }
            set { _addVertexNextMove = value; RaisePropertyChanged(nameof(AddVertexNextMove));
                RaisePropertyChanged(nameof(DeleteVertexButtonEnabled));
                RaisePropertyChanged(nameof(MovePolygonButtonEnabled));
                RaisePropertyChanged(nameof(SetEqualLengthButtonEnabled));
                RaisePropertyChanged(nameof(SetParallelButtonEnabled));
                RaisePropertyChanged(nameof(ModeButtonEnabled));
                RaisePropertyChanged(nameof(RemoveRelationButtonEnabled));
            }
        }

        public Vertex LastVertex
        {
            get { return _lastVertex; }
            set { _lastVertex = value; RaisePropertyChanged(nameof(LastVertex));
                RaisePropertyChanged(nameof(ModeButtonEnabled));
            }
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

        public Edge EqRelE1
        {
            get { return _eqRelE1; }
            set { _eqRelE1 = value; RaisePropertyChanged(nameof(EqRelE1)); }
        }
        public Edge EqRelE2
        {
            get { return _eqRelE2; }
            set { _eqRelE2 = value; RaisePropertyChanged(nameof(EqRelE2)); }
        }

        public Edge ParRelE1
        {
            get { return _parRelE1; }
            set { _parRelE1 = value; RaisePropertyChanged(nameof(ParRelE1)); }
        }
        public Edge ParRelE2
        {
            get { return _parRelE2; }
            set { _parRelE2 = value; RaisePropertyChanged(nameof(ParRelE2)); }
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

        public RelayCommand SetEqRelation
        {
            get;
            private set;
        }

        public RelayCommand SetParRelation
        {
            get;
            private set;
        }

        public RelayCommand ClearBitmap
        {
            get;
            private set;
        }

        public RelayCommand RemoveRelation
        {
            get;
            private set;
        }

        public bool DeleteVertexButtonEnabled
        {
            get
            {
                return IsMovingModeOn && !PolygonMovingMode && !SettingEqRelMode && !SettingParRelMode && !AddVertexNextMove
                    && !IsEdgeMoving && !IsVertexMoving && !IsPolygonMoving && !RemoveRelationMode;
            }
        }

        public bool AddVertexButtonEnabled
        {
            get
            {
                return IsMovingModeOn && !PolygonMovingMode && !SettingEqRelMode && !SettingParRelMode && !DeleteVertexNextMove
                    && !IsEdgeMoving && !IsVertexMoving && !IsPolygonMoving && !RemoveRelationMode;
            }
        }

        public bool MovePolygonButtonEnabled
        {
            get
            {
                return IsMovingModeOn && !AddVertexNextMove && !SettingEqRelMode && !SettingParRelMode && !DeleteVertexNextMove
                  && !IsEdgeMoving && !IsVertexMoving && !RemoveRelationMode;
            }
        }

        public bool SetEqualLengthButtonEnabled
        {
            get
            {
                return IsMovingModeOn && !AddVertexNextMove && !PolygonMovingMode && !IsPolygonMoving && !SettingParRelMode 
                    && !DeleteVertexNextMove && !IsEdgeMoving && !IsVertexMoving && !RemoveRelationMode;
            }
        }

        public bool SetParallelButtonEnabled
        {
            get
            {
                return IsMovingModeOn && !AddVertexNextMove && !PolygonMovingMode && !IsPolygonMoving && !SettingEqRelMode
                    && !DeleteVertexNextMove && !IsEdgeMoving && !IsVertexMoving && !RemoveRelationMode;
            }
        }

        public bool ModeButtonEnabled
        {
            get
            {
                return LastVertex == null && !IsPolygonMoving && !PolygonMovingMode && !SettingEqRelMode && !SettingParRelMode
                    && !DeleteVertexNextMove && !IsEdgeMoving && !IsVertexMoving && !RemoveRelationMode;
            }
        }

        public bool RemoveRelationButtonEnabled
        {
            get
            {
                return IsMovingModeOn && !AddVertexNextMove && !PolygonMovingMode && !IsPolygonMoving && !SettingEqRelMode
                    && !SettingParRelMode && !DeleteVertexNextMove && !IsEdgeMoving && !IsVertexMoving;
            }
        }

        public MainViewModel()
        {
            Width = Convert.ToInt32(ConfigurationSettings.AppSettings["bitmapWidth"]);
            Height = Convert.ToInt32(ConfigurationSettings.AppSettings["bitmapHeight"]);
            Bitmap = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Bgr32, null);
            IsFirstVertexOfEdge = true;
            IsDrawingModeOn = false;
            IsMovingModeOn = true;
            Vertices = new ObservableCollection<Vertex>();
            Edges = new ObservableCollection<Edge>();
            Polygons = new ObservableCollection<Polygon>();
            VerticesInProgress = new ObservableCollection<Vertex>();
            EdgesInProgress = new ObservableCollection<Edge>();
            colors = new List<(int r, int g, int b)>();
            RelationCounter = 0;

            colors.Add((0, 255, 0));
            colors.Add((0, 0, 255));
            colors.Add((0, 255, 255));
            colors.Add((255, 255, 255));
            colors.Add((255, 0, 255));
            colors.Add((255, 255, 0));

            AddInitialPolygon();

            MouseUpOnBitmap = new RelayCommand<Point>(BitmapUpClick, true);
            MouseMoveOnBitmap = new RelayCommand<MouseData>(BitmapMove, true);
            ChangeMode = new RelayCommand(() => {
                if (LastVertex == null && !IsPolygonMoving && !PolygonMovingMode && !SettingEqRelMode && !SettingParRelMode 
                    && !DeleteVertexNextMove && !IsEdgeMoving && !IsVertexMoving && !RemoveRelationMode)
                {
                    IsDrawingModeOn = !IsDrawingModeOn;
                    IsMovingModeOn = !IsMovingModeOn;
                }
            });
            DeleteVertex = new RelayCommand(() => { DeleteVertexNextMove = !DeleteVertexNextMove;});
            AddVertex = new RelayCommand(() => { AddVertexNextMove = !AddVertexNextMove; });
            MovePolygon = new RelayCommand(() => { PolygonMovingMode = !PolygonMovingMode; });
            SetEqRelation = new RelayCommand(() => { SettingEqRelMode = !SettingEqRelMode; });
            SetParRelation = new RelayCommand(() => { SettingParRelMode = !SettingParRelMode; });
            RemoveRelation = new RelayCommand(() => { RemoveRelationMode = !RemoveRelationMode; });
            ClearBitmap = new RelayCommand(() =>
            {
                //if (!IsDrawingModeOn)
                {
                    Polygons = new ObservableCollection<Polygon>();
                    Vertices = new ObservableCollection<Vertex>();
                    Edges = new ObservableCollection<Edge>();
                    VerticesInProgress = new ObservableCollection<Vertex>();
                    EdgesInProgress = new ObservableCollection<Edge>();
                    RelationCounter = 0;
                    LastVertex = null;
                    FirstVertexOfPolygonInProgress = null;
                    RefreshBitmap();
                }
            });

        }

        private void AddInitialPolygon()
        {
            Vertex[] initV = new Vertex[7];
            initV[0] = new Vertex(581, 111);
            initV[1] = new Vertex(646, 75);
            initV[2] = new Vertex(222, 159);
            initV[3] = new Vertex(210, 516);
            initV[4] = new Vertex(708, 672);
            initV[5] = new Vertex(1006, 508);
            initV[6] = new Vertex(1004, 151);

            Edge[] initE = new Edge[7];
            for (int i = 0; i < initE.Length; i++)
                initE[i] = new Edge(initV[i], initV[(i + 1) % initE.Length]);

            initE[0].RelType = TypeOfRelation.Parallel;
            initE[0].RelatedEdge = initE[4];
            initE[4].RelType = TypeOfRelation.Parallel;
            initE[4].RelatedEdge = initE[0];

            initE[2].RelType = TypeOfRelation.Equal;
            initE[2].RelatedEdge = initE[5];
            initE[5].RelType = TypeOfRelation.Equal;
            initE[5].RelatedEdge = initE[2];

            for(int i = 0; i < initV.Length; i++)
            {
                initV[i].AddEdge(initE[i]);
                initV[i].AddEdge(initE[(i + initV.Length - 1) % initV.Length]);

                EdgesInProgress.Add(initE[i]);
                VerticesInProgress.Add(initV[i]);
                Edges.Add(initE[i]);
                Vertices.Add(initV[i]);
            }

            Polygons.Add(new Polygon(VerticesInProgress, EdgesInProgress));
            VerticesInProgress = new ObservableCollection<Vertex>();
            EdgesInProgress = new ObservableCollection<Edge>();

            RefreshBitmap();
        }

        private void BitmapMove(MouseData obj)
        {
            int x = (int) obj.PosX;
            int y = (int) obj.PosY;

            if (IsVertexMoving)
            {
                if (x < 4 || x > Width - 4 || y < 4 || y > Height - 4)
                    return;
                bool ruszyc = true;

                var poly = FindPolygonOfVertex(CapturedVertex);

                if (CapturedVertex.E1.RelType == TypeOfRelation.Equal || CapturedVertex.E2.RelType == TypeOfRelation.Equal)
                {
                    if (CapturedVertex.E1.RelType == TypeOfRelation.Equal)
                    {
                        poly.KeepEqualLength(CapturedVertex.E1, CapturedVertex.E1.RelatedEdge, CapturedVertex, x, y, CapturedVertex.E1, 0, 0);
                        ruszyc = false;
                    }

                    if (CapturedVertex.E2.RelType == TypeOfRelation.Equal)
                    {
                        poly.KeepEqualLength(CapturedVertex.E2, CapturedVertex.E2.RelatedEdge, CapturedVertex, x, y, CapturedVertex.E2, 0, 0);
                        ruszyc = false;
                    }
                }
                else
                {
                    if (CapturedVertex.E1.RelType == TypeOfRelation.Parallel)
                    {
                        poly.KeepParallel(CapturedVertex.E1, CapturedVertex.E1.RelatedEdge, CapturedVertex, x, y, CapturedVertex.E1, 0, 0);
                        ruszyc = false;
                    }

                    if (CapturedVertex.E2.RelType == TypeOfRelation.Parallel)
                    {
                        poly.KeepParallel(CapturedVertex.E2, CapturedVertex.E2.RelatedEdge, CapturedVertex, x, y,
                            CapturedVertex.E2, 0, 0);
                        ruszyc = false;
                    }
                }

                if (ruszyc)
                {
                    CapturedVertex.X = x;
                    CapturedVertex.Y = y;
                }

                RefreshBitmap();
            }
            else if (IsEdgeMoving)
            {
                var v1 = CapturedEdge.V1;
                var v2 = CapturedEdge.V2;
                var v1Xstart = v1.X;
                var v1Ystart = v1.Y;
                var v2Xstart = v2.X;
                var v2Ystart = v2.Y;
                var e1 = v1.E1 != CapturedEdge ? v1.E1 : v1.E2;
                var e2 = v2.E1 != CapturedEdge ? v2.E1 : v2.E2;
                bool ruszyc = true;
                bool e1ok = true;
                bool e2ok = true;
                bool e1rusz = false;
                bool e2rusz = false;
                var poly = FindPolygonOfVertex(v1);
                var copy = new List<int>();
                for (int i = 0; i < poly.Vertices.Count; i++)
                {
                    copy.Add(poly.Vertices[i].X);
                    copy.Add(poly.Vertices[i].Y);
                }

                if (v1.X + x - CapturedPointOfEdge.X < 4 || v1.X + x - CapturedPointOfEdge.X > Width - 4
                                                         || v1.Y + y - CapturedPointOfEdge.Y < 4 ||
                                                         v1.Y + y - CapturedPointOfEdge.Y > Height - 4
                                                         || v2.X + x - CapturedPointOfEdge.X < 4 ||
                                                         v2.X + x - CapturedPointOfEdge.X > Width - 4
                                                         || v2.Y + y - CapturedPointOfEdge.Y < 4 ||
                                                         v2.Y + y - CapturedPointOfEdge.Y > Height - 4)
                    return;

                if (e1.RelType == TypeOfRelation.Equal)
                {
                    e1ok = poly.KeepEqualLength(e1, e1.RelatedEdge, v1, v1.X + x - CapturedPointOfEdge.X, v1.Y + y - CapturedPointOfEdge.Y, e1, 0, 0);
                    ruszyc = false;
                    e1rusz = e1ok;
                }
                else if (e1.RelType == TypeOfRelation.Parallel)
                {
                    e1ok = poly.KeepParallel(e1, e1.RelatedEdge, v1, v1.X + x - CapturedPointOfEdge.X, v1.Y + y - CapturedPointOfEdge.Y, e1, 0, 0);
                    ruszyc = false;
                    e1rusz = e1ok;
                }

                if (e2.RelType == TypeOfRelation.Equal)
                {
                    e2ok = poly.KeepEqualLength(e2, e2.RelatedEdge, v2, v2.X + x - CapturedPointOfEdge.X, v2.Y + y - CapturedPointOfEdge.Y, e2, 0, 0);
                    ruszyc = false;
                    e2rusz = e2ok;
                }
                else if (e2.RelType == TypeOfRelation.Parallel)
                {
                    e2ok = poly.KeepParallel(e2, e2.RelatedEdge, v2, v2.X + x - CapturedPointOfEdge.X, v2.Y + y - CapturedPointOfEdge.Y, e2, 0, 0);
                    ruszyc = false;
                    e2rusz = e2ok;
                }


                if (!e1ok || !e2ok)
                {

                    for (int i = 0; i < poly.Vertices.Count; i++)
                    {
                        poly.Vertices[i].X = copy[2 * i];
                        poly.Vertices[i].Y = copy[2 * i + 1];
                    }

                }
                else if (ruszyc && e1ok && e2ok)
                {
                    v1.X = v1.X + x - CapturedPointOfEdge.X;
                    v1.Y = v1.Y + y - CapturedPointOfEdge.Y;
                    v2.X = v2.X + x - CapturedPointOfEdge.X;
                    v2.Y = v2.Y + y - CapturedPointOfEdge.Y;
                }
                else if (e1rusz && !e2rusz)
                {
                    v2.X = v2.X + v1.X - v1Xstart;
                    v2.Y = v2.Y + v1.Y - v1Ystart;
                }
                else if (!e1rusz && e2rusz)
                {
                    v1.X = v1.X + v2.X - v2Xstart;
                    v1.Y = v1.Y + v2.Y - v2Ystart;
                }


                if (ruszyc || (e1ok && e2ok))
                {
                    CapturedPointOfEdge = (x, y);
                }

                

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
                    if (CapturedPolygon.Vertices[i].Y < YMin)
                        YMin = CapturedPolygon.Vertices[i].Y;
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
                    LastVertex = null;
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
                if(SettingEqRelMode && e != null)
                {
                    if (RelationCounter < 6)
                    {
                        if (EqRelE1 == null)
                        {
                            if (e.RelType != TypeOfRelation.None)
                            {
                                SettingEqRelMode = false;
                                return;
                            }
                            EqRelE1 = e;
                            Drawer.DrawEdge(Bitmap, e, 0, 255, 0);
                        }
                        else if (EqRelE2 == null)
                        {
                            if (e.RelType != TypeOfRelation.None || e == EqRelE1)
                            {
                                EqRelE1 = null;
                                SettingEqRelMode = false;
                                return;
                            }

                            var poly = FindPolygonOfVertex(e.V1);
                            var success = poly.SetEqualLength(EqRelE1, e, e, 0, 0);

                            if(success)
                            {
                                EqRelE1.RelatedEdge = e;
                                EqRelE1.RelType = TypeOfRelation.Equal;
                                e.RelatedEdge = EqRelE1;
                                e.RelType = TypeOfRelation.Equal;
                            }

                            EqRelE1 = null;
                            EqRelE2 = null;
                            SettingEqRelMode = false;
                            RelationCounter++;

                            RefreshBitmap();
                        }
                    }
                }
                else if (SettingParRelMode && e != null)
                {
                    if(RelationCounter < 6)
                    {
                        if (ParRelE1 == null)
                        {
                            if (e.RelType != TypeOfRelation.None)
                            {
                                SettingParRelMode = false;
                                return;
                            }
                            ParRelE1 = e;
                            Drawer.DrawEdge(Bitmap, e, 0, 255, 0);
                        }
                        else if (ParRelE2 == null)
                        {
                            if (!(ParRelE1.V1.E1 != e && ParRelE1.V1.E2 != e && ParRelE1.V2.E1 != e && ParRelE1.V2.E2 != e)
                                || e.RelType != TypeOfRelation.None || e == ParRelE1)
                            {
                                ParRelE1 = null;
                                SettingParRelMode = false;
                                return;
                            }

                            var poly = FindPolygonOfVertex(e.V1);
                            var success = poly.SetParallel(ParRelE1, e, e, 0, 0);

                            if (success)
                            {
                                ParRelE1.RelatedEdge = e;
                                ParRelE1.RelType = TypeOfRelation.Parallel;
                                e.RelatedEdge = ParRelE1;
                                e.RelType = TypeOfRelation.Parallel;
                            }

                            ParRelE1 = null;
                            ParRelE2 = null;
                            SettingParRelMode = false;
                            RelationCounter++;

                            RefreshBitmap();
                        }
                    }
                }
                else if (PolygonMovingMode)
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
                else if (CheckIfOnExistingVertex(x, y) && !IsVertexMoving && !DeleteVertexNextMove && !AddVertexNextMove && !RemoveRelationMode)
                {
                    IsVertexMoving = true;
                    CapturedVertex = FindCapturedVertex(x, y);
                }
                else if (e != null && !IsEdgeMoving && !IsVertexMoving && !AddVertexNextMove && !DeleteVertexNextMove && !RemoveRelationMode)
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
                        DeleteSelectedVertex(SelectedVertex);
                        SelectedVertex = null;

                        RefreshBitmap();
                    }

                    DeleteVertexNextMove = false;
                }
                else if (AddVertexNextMove)
                {
                    if (e != null)// && e.RelType == TypeOfRelation.None)
                    {
                        AddNewVertex(e);
                        RefreshBitmap();
                    }

                    AddVertexNextMove = false;
                }
                else if(RemoveRelationMode)
                {
                    if(e != null)
                    {
                        RemoveRelationIfPresent(e);
                    }
                    RemoveRelationMode = false;
                    RefreshBitmap();
                }
            }
        }

        private void AddNewVertex(Edge e)
        {
            RemoveRelationIfPresent(e);

            var poly = FindPolygonOfVertex(e.V1);
            var v1 = e.V1;
            var v2 = e.V2;
            var index = poly.Vertices.IndexOf(e.V1) < poly.Vertices.IndexOf(e.V2) ?
                poly.Vertices.IndexOf(e.V1) : poly.Vertices.IndexOf(e.V2);
            if (poly.Vertices.IndexOf(v1) == 0 && poly.Vertices.IndexOf(v2) == poly.Vertices.Count - 1)
            {
                index = poly.Vertices.IndexOf(v2);
            }

            if (poly.Vertices.IndexOf(v2) == 0 && poly.Vertices.IndexOf(v1) == poly.Vertices.Count - 1)
            {
                index = poly.Vertices.IndexOf(v1);
            }

            var vnew = new Vertex((v1.X + v2.X) / 2, (v1.Y + v2.Y) / 2);

            poly.Vertices.Insert(index + 1, vnew);
            Vertices.Add(vnew);
            Edges.Remove(e);
            poly.Edges.Remove(e);

            var e1 = new Edge(v1, vnew);
            var e2 = new Edge(v2, vnew);

            vnew.AddEdge(e1);
            vnew.AddEdge(e2);

            Edges.Add(e1);
            Edges.Add(e2);
            poly.Edges.Add(e1);
            poly.Edges.Add(e2);
        }

        private void DeleteSelectedVertex(Vertex selectedVertex)
        {
            if (FindPolygonOfVertex(SelectedVertex).Vertices.Count < 4)
            {

                RemoveRelationIfPresent(SelectedVertex.E1);
                RemoveRelationIfPresent(SelectedVertex.E2);

                SelectedVertex = null;
                DeleteVertexNextMove = false;
                return;
            }

            var v1 = SelectedVertex.E1.V1 == selectedVertex ? SelectedVertex.E1.V2 : SelectedVertex.E1.V1;
            var v2 = SelectedVertex.E2.V1 == selectedVertex ? SelectedVertex.E2.V2 : SelectedVertex.E1.V1;

            var e1 = (v1.E1.V1 == v1 && v1.E1.V2 == selectedVertex) || (v1.E1.V2 == v1 && v1.E1.V1 == selectedVertex) ? v1.E1 : v1.E2;
            var e2 = (v2.E1.V1 == v1 && v2.E1.V2 == selectedVertex) || (v2.E1.V2 == v1 && v2.E1.V1 == selectedVertex) ? v2.E1 : v2.E2;

            var p = FindPolygonOfVertex(selectedVertex);

            RemoveRelationIfPresent(SelectedVertex.E1);

            RemoveRelationIfPresent(SelectedVertex.E2);

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
                if (x >= v.X - 3 && x <= v.X + 3 && y >= v.Y - 3 && y <= v.Y + 3)
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
                if (x >= v.X - 3 && x <= v.X + 3 && y >= v.Y - 3 && y <= v.Y + 3)
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

        private void RemoveRelationIfPresent(Edge e)
        {
            if (e.RelType != TypeOfRelation.None)
            {
                e.RelType = TypeOfRelation.None;
                e.RelatedEdge.RelType = TypeOfRelation.None;
                e.RelatedEdge.RelatedEdge = null;
                e.RelatedEdge = null;
            }
        }

        public void RefreshBitmap()
        {

            byte[] pixels1d = new byte[Width * Height * 4];
            Int32Rect rect = new Int32Rect(0, 0, Width, Height);
            int stride = 4 * Width;
            Bitmap.WritePixels(rect, pixels1d, stride, 0);



            foreach (var poly in Polygons)
            {
                var firstV = poly.Vertices[0];
                for (int i = 0; i < poly.Vertices.Count; i++)
                {
                    Drawer.DrawVertex(Bitmap, poly.Vertices[i]);
                    Drawer.DrawEdge(Bitmap, poly.Vertices[i], poly.Vertices[(i + 1) % poly.Vertices.Count]);
                }
            }

            int j = 0;

            var Edges2 = new Edge[Edges.Count];
            Edges.CopyTo(Edges2, 0);
            var Edges3 = Edges2.ToList();

            while(Edges3.Count > 0)
            {
                var e = Edges3[0];
                if (e.RelType == TypeOfRelation.None)
                {
                    Edges3.RemoveAt(0);
                    continue;
                }
                    


                else if (e.RelType != TypeOfRelation.None)
                {
                    Drawer.DrawMark(Bitmap, e, colors[j].r, colors[j].g, colors[j].b);
                    Drawer.DrawMark(Bitmap, e.RelatedEdge, colors[j].r, colors[j].g, colors[j].b);
                    j++;
                    Edges3.Remove(e.RelatedEdge);
                    Edges3.Remove(e);
                }
            }
        }    
    }
}
