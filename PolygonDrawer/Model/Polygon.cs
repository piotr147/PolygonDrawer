using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Annotations;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight;

namespace PolygonDrawer.Model
{
    public class Polygon : ObservableObject
    {
        private ObservableCollection<Vertex> _vertices;
        private ObservableCollection<Edge> _edges;
        public readonly int height;
        public readonly int width;

        public ObservableCollection<Vertex> Vertices
        {
            get { return _vertices; }
            set { _vertices = value; RaisePropertyChanged(nameof(Vertices)); }
        }

        public ObservableCollection<Edge> Edges
        {
            get { return _edges; }
            set { _edges = value; RaisePropertyChanged(nameof(Edges)); }
        }

        public Polygon(ObservableCollection<Vertex> vertices, ObservableCollection<Edge> edges)
        {
            width = Convert.ToInt32(ConfigurationSettings.AppSettings["bitmapWidth"]);
            height = Convert.ToInt32(ConfigurationSettings.AppSettings["bitmapHeight"]);
            this.Vertices = new ObservableCollection<Vertex>(vertices);
            this.Edges = new ObservableCollection<Edge>(edges);
        }


        public bool CanBeMoved(Edge e, Vertex movV, int x, int y, Edge startEdge, int circles)
        {
            if (e.RelType == TypeOfRelation.None)
                return true;
            if (e.RelType == TypeOfRelation.Equal)
            {
                var anV = e.V1 != movV ? e.V1 : e.V2;

                return KeepEqualLength(e, e.RelatedEdge, movV, x, y, startEdge, circles);

            }

            return true;
        }
        //public bool CanBeSet()


        public bool KeepEqualLength(Edge movE, Edge relE, Vertex movV, int xTo, int yTo, Edge startEdge, int circles)
        {
            var wantedLength = relE.Length;
            var currentLength = movE.Length;
            var anV = movV != movE.V1 ? movE.V1 : movE.V2;

            var x = anV.X;
            var y = anV.Y;

            x = xTo + (int) ((double) (x - xTo) * (double) wantedLength / (double) currentLength);
            y = yTo + (int) ((double) (y - yTo) * (double) wantedLength / (double) currentLength);

            if (x > 4 && y > 4 && x < width - 4 && y < height - 4)
            {
                if (true) //can be changed
                {
                    anV.X = x;
                    anV.Y = y;
                    movV.X = xTo;
                    movV.Y = yTo;
                    return true;
                }
            }

            return false;
        }

        public bool SetEqualLength(Edge e1, Edge e2, Edge startEdge, int circle)
        {
            var wantedLength = e1.Length;
            var v1 = e2.V1.X < e2.V2.X ? e2.V1 : e2.V2;
            var v2 = e2.V1 != v1 ? e2.V1 : e2.V2;
            var currentLength = e2.Length;

            var x = v2.X;
            var y = v2.Y;

            x = v1.X + (int) ((double) (v2.X - v1.X) * (double) wantedLength / (double) currentLength);
            y = v1.Y + (int) ((double) (v2.Y - v1.Y) * (double) wantedLength / (double) currentLength);

            if (x > 4 && y > 4 && x < width - 4 && y < height - 4)
            {
                if (true) //can be changed
                {
                    v2.X = x;
                    v2.Y = y;
                    return true;
                }
            }

            x = v1.X;
            y = v1.Y;
            x = v2.X + (int) ((double) (v1.X - v2.X) * (double) wantedLength / (double) currentLength);
            y = v2.Y + (int) ((double) (v1.Y - v2.Y) * (double) wantedLength / (double) currentLength);

            if (x > 4 && y > 4 && x < width - 4 && y < height - 4)
            {
                if (true) //can be changed
                {
                    v1.X = x;
                    v1.Y = y;
                    return true;
                }
            }

            wantedLength = e2.Length;
            v1 = e1.V1.X < e1.V2.X ? e1.V1 : e1.V2;
            v2 = e1.V1 != v1 ? e1.V1 : e1.V2;
            currentLength = e1.Length;

            x = v2.X;
            y = v2.Y;

            x = v1.X + (int)((double)(v2.X - v1.X) * (double)wantedLength / (double)currentLength);
            y = v1.Y + (int)((double)(v2.Y - v1.Y) * (double)wantedLength / (double)currentLength);

            if (x > 4 && y > 4 && x < width - 4 && y < height - 4)
            {
                if (true) //can be changed
                {
                    v2.X = x;
                    v2.Y = y;
                    return true;
                }
            }

            x = v1.X;
            y = v1.Y;
            x = v2.X + (int)((double)(v1.X - v2.X) * (double)wantedLength / (double)currentLength);
            y = v2.Y + (int)((double)(v1.Y - v2.Y) * (double)wantedLength / (double)currentLength);

            if (x > 4 && y > 4 && x < width - 4 && y < height - 4)
            {
                if (true) //can be changed
                {
                    v1.X = x;
                    v1.Y = y;
                    return true;
                }
            }


            return false;
        }


        public bool SetParallel(Edge e1, Edge e2, Edge startEdge, int circle)
        {

            if (e1.V1.X != e1.V2.X)
            {
                var wantedTan = Tan(e1.V1.X, e1.V1.Y, e1.V2.X, e1.V2.Y);
                Vertex v1;
                Vertex v2;
                if(e2.V1.X > e2.V2.X)
                {
                    v2 = e2.V1;
                    v1 = e2.V2;
                }
                else
                {
                    v1 = e2.V1;
                    v2 = e2.V2;
                }

                int furthest = v2.X;
                if (furthest < v2.Y)
                    furthest = v2.Y;
                if (furthest < height - v2.Y)
                    furthest = height - v2.Y;
                if(furthest < width - v2.X)
                    furthest = width - v2.X;
                int x = v2.X;
                int y = v2.Y;

                for(int i = 0; i < furthest - 4; i++)
                {

                    if (CheckAndSet(wantedTan, v2, x + i, y, v1))
                        return true;
                    if (CheckAndSet(wantedTan, v2, x - i, y, v1))
                        return true;
                    if (CheckAndSet(wantedTan, v2, x, y + i, v1))
                        return true;
                    if (CheckAndSet(wantedTan, v2, x, y - i, v1))
                        return true;
                    if (CheckAndSet(wantedTan, v2, x + i, y + i, v1))
                        return true;
                    if (CheckAndSet(wantedTan, v2, x + i, y - i, v1))
                        return true;
                    if (CheckAndSet(wantedTan, v2, x - i, y + i, v1))
                        return true;
                    if (CheckAndSet(wantedTan, v2, x - i, y - i, v1))
                        return true;
                }

                x = v1.X;
                y = v1.Y;

                furthest = v1.X;
                if (furthest < v1.Y)
                    furthest = v1.Y;
                if (furthest < height - v1.Y)
                    furthest = height - v1.Y;
                if (furthest < width - v1.X)
                    furthest = width - v1.X;

                for (int i = 0; i < furthest - 4; i++)
                {

                    if (CheckAndSet(wantedTan, v1, x + i, y, v2))
                        return true;
                    if (CheckAndSet(wantedTan, v1, x - i, y, v2))
                        return true;
                    if (CheckAndSet(wantedTan, v1, x, y + i, v2))
                        return true;
                    if (CheckAndSet(wantedTan, v1, x, y - i, v2))
                        return true;
                    if (CheckAndSet(wantedTan, v1, x + i, y + i, v2))
                        return true;
                    if (CheckAndSet(wantedTan, v1, x + i, y - i, v2))
                        return true;
                    if (CheckAndSet(wantedTan, v1, x - i, y + i, v2))
                        return true;
                    if (CheckAndSet(wantedTan, v1, x - i, y - i, v2))
                        return true;
                }
            }
            else // tan == inf
            {

            }


            
            return false;
        }


        public bool CanBeSet(Edge anE, Vertex v2, int x, int y, Edge startEdge, int circle)
        {
            if (anE.RelType == TypeOfRelation.None)
            {
                return true;
            }
            else if (anE.RelType == TypeOfRelation.Equal)
            {
                var anV = anE.V1 != v2 ? anE.V1 : anE.V2;

                if (anE.RelatedEdge.Length + 10 >= Length(anV.X, anV.Y, x, y))
                    return true;
                else
                    return false;
            }
            return true;
        }

        public bool AdjustLenRel(Edge EMoving, int Xf, int Yf, int Xt, int Yt)//, Edge EToMove)
        {
            var eRel = EMoving.RelatedEdge;
            var v1moving = Xf == EMoving.V1.X && Yf == EMoving.V1.Y ? EMoving.V1 : EMoving.V2;
            var vStaying = v1moving == EMoving.V1 ? EMoving.V2 : EMoving.V1;
            bool lenGood = false;

            var wantedLen = (int)Math.Sqrt((vStaying.X - Xt) *(vStaying.X - Xt) + (vStaying.Y - Yt) *(vStaying.Y - Yt));

            var e = eRel;
            var v1 = eRel.V1;
            var v2 = eRel.V2;

            do
            {
                for(int i = 1; i < 100; i ++)
                {
                    for(int j = v1.X - i; j <= v1.X + i; j++)
                    {
                        for(int k = v1.Y - i; k <= v1.Y + i; k++)
                        {
                            if (k > width - 4 || k < 4 || j > height - 4 || j < 4)
                                continue;
                            if(Length(j, k, v2.X, v2.Y) == wantedLen)
                            {
                                //sprawdz druga krawedz
                                lenGood = true;
                                v1.X = j;
                                v1.Y = k;
                                break;
                            }
                        }
                        if (lenGood)
                            break;
                    }
                    if (lenGood)
                        break;
                }
                if (lenGood)
                    break;

            }
            while (true);

            return lenGood;

            return false;
        }

        private int Length(int x1, int y1, int x2, int y2)
        {
            return (int)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        private double Tan(int x1, int y1, int x2, int y2)
        {
            double bX = x1 > x2 ? (double) x1 : (double) x2;
            double bY = bX == (double) x1 ? (double) y1 : (double) y2;
            double sX = bX != (double) x1 ? (double) x1 : (double) x2;
            double sY = sX == (double) x1 ? (double) y1 : (double) y2;

            if (x1 > x2)
            {

            }

            return (bY - sY) / (bX - sX);
        }

        private bool AlmostEqual(double d1, double d2, double prec)
        {
            if (d1 * d2 < 0)
                return false;
            d1 = d1 > 0 ? d1 : d1 * -1;
            d2 = d2 > 0 ? d2 : d2 * -1;
            double bigger;
            double smaller;
            if (d1 > d2)
            {
                bigger = d1;
                smaller = d2;
            }
            else
            {
                smaller = d1;
                bigger = d2;
            }

            bool res = false;

            
            if (bigger < 0)
            {
                bigger *= -1;
                smaller *= -1;
            }

            return smaller > bigger * prec;
        }

        private bool CheckAndSet(double wantedTan, Vertex vToCheck, int x, int y, Vertex anotherV)
        {
            if (AlmostEqual(wantedTan, Tan(x, y, anotherV.X, anotherV.Y), 0.9))
            {
                if (x > 4 && y > 4 && x < width - 4 && y < height - 4)
                {
                    if (true) // can be set
                    {
                        vToCheck.X = x;
                        vToCheck.Y = y;
                        return true;
                    }
                }
            }

            return false;
        }


    }
}
