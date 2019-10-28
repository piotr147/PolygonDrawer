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

        public bool KeepEqualLength(Edge movE, Edge relE, Vertex movV, int xTo, int yTo, Edge startEdge, int circles)
        {
            var secEdgeOfMovV = movE != movV.E1 ? movV.E1 : movV.E2;

            if(secEdgeOfMovV.RelType == TypeOfRelation.Parallel)
            {
                if (!KeepParallel(secEdgeOfMovV, secEdgeOfMovV.RelatedEdge, movV, xTo, yTo, startEdge, circles))
                    return false;
            }

            //if (movV.X == xTo && movV.Y == yTo)
            //    return false;
            if (circles > 0 && movE == startEdge)
            {
                return false;
            }
            if (movE == startEdge && circles == 0)
                circles++;

            var oldX = movV.X;
            var oldY = movV.Y;

            movV.X = xTo;
            movV.Y = yTo;

            var success = false;

            var wantedLength = Length(movE.V1.X, movE.V1.Y, movE.V2.X, movE.V2.Y);
            var currentLength = relE.Length;

            var v1 = relE.V1;
            var v2 = relE.V2;
            var x = v1.X;
            var y = v1.Y;

            x = v2.X + (int)((double)(x - v2.X) * (double)wantedLength / (double)currentLength);
            y = v2.Y + (int)((double)(y - v2.Y) * (double)wantedLength / (double)currentLength);

            if (x > 4 && y > 4 && x < width - 4 && y < height - 4)
            {
                if(CanBeSet(relE, v1, x, y, startEdge, circles))
                {
                    v1.X = x;
                    v1.Y = y;
                    return true;
                }
            }

            v1 = relE.V2;
            v2 = relE.V1;
            x = v1.X;
            y = v1.Y;

            x = v2.X + (int)((double)(x - v2.X) * (double)wantedLength / (double)currentLength);
            y = v2.Y + (int)((double)(y - v2.Y) * (double)wantedLength / (double)currentLength);

            if (x > 4 && y > 4 && x < width - 4 && y < height - 4)
            {
                if (CanBeSet(relE, v1, x, y, startEdge, circles))
                {
                    v1.X = x;
                    v1.Y = y;
                    return true;
                }
            }

            movV.X = oldX;
            movV.Y = oldY;

            return false;
        }


        public bool KeepEqualLength22(Edge movE, Edge relE, Vertex movV, int xTo, int yTo, Edge startEdge, int circles)
        {
            if(movV.X == xTo && movV.Y == yTo)
                return false;
            if (circles > 0 && movE == startEdge)
            {
                return false;
            }
            if (movE == startEdge && circles == 0)
                circles++;

            var secEdgeOfMovV = movE != movV.E1 ? movV.E1 : movV.E2;

            //if (!CanBeSet(secEdgeOfMovV, movV, xTo, yTo, startEdge, 0))
            //    return false;

            var wantedLength = relE.Length;
            var currentLength = movE.Length;
            var anV = movV != movE.V1 ? movE.V1 : movE.V2;

            var x = anV.X;
            var y = anV.Y;

            x = xTo + (int) ((double) (x - xTo) * (double) wantedLength / (double) currentLength);
            y = yTo + (int) ((double) (y - yTo) * (double) wantedLength / (double) currentLength);

            if (x > 4 && y > 4 && x < width - 4 && y < height - 4)
            {
                //if (true) //can be changed
                //if(CanBeSet(movE, anV, x, y, startEdge, circles) 
                //   && CanBeSet(movE, movV, xTo, yTo, startEdge, circles))
                if(CanBeSet(movE, anV, x, y, startEdge, circles))
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

        public bool SetEqualLength(Edge e1, Edge e2, Edge startEdge, int circles)
        {
            if (circles > 0 && e2 == startEdge)
            {
                return false;
            }
            if (e2 == startEdge && circles == 0)
                circles++;

            var wantedLength = e1.Length;
            var v1 = e2.V1.X < e2.V2.X ? e2.V1 : e2.V2;
            var v2 = e2.V1 != v1 ? e2.V1 : e2.V2;
            var currentLength = e2.Length;


            //if((v1.E1 == e1 && v1.E2 == e2) || (v1.E2 == e1 && v1.E1 == e2))
            //{

            //}
            var x = v2.X;
            var y = v2.Y;

            x = v1.X + (int) ((double) (v2.X - v1.X) * (double) wantedLength / (double) currentLength);
            y = v1.Y + (int) ((double) (v2.Y - v1.Y) * (double) wantedLength / (double) currentLength);

            if (x > 4 && y > 4 && x < width - 4 && y < height - 4)
            {
                //if (true) //can be changed
                if(CanBeSet(e2, v2, x, y, startEdge, circles))
                {
                    //if ((v1.E1 == e1 && v1.E2 == e2) || (v1.E2 == e1 && v1.E1 == e2))
                    if (!((v2.E1 == e1 && v2.E2 == e2) || (v2.E2 == e1 && v2.E1 == e2)))
                    {
                        v2.X = x;
                        v2.Y = y;
                        return true;
                    }
                }
            }

            x = v1.X;
            y = v1.Y;
            x = v2.X + (int) ((double) (v1.X - v2.X) * (double) wantedLength / (double) currentLength);
            y = v2.Y + (int) ((double) (v1.Y - v2.Y) * (double) wantedLength / (double) currentLength);

            if (x > 4 && y > 4 && x < width - 4 && y < height - 4)
            {
                //if (true) //can be changed
                if(CanBeSet(e2, v1, x, y, startEdge, circles))
                {
                    //if ((v2.E1 == e1 && v2.E2 == e2) || (v2.E2 == e1 && v2.E1 == e2))
                    if (!((v1.E1 == e1 && v1.E2 == e2) || (v1.E2 == e1 && v1.E1 == e2)))
                    {
                        v1.X = x;
                        v1.Y = y;
                        return true;
                    }
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
                //if (true) //can be changed
                if(CanBeSet(e1, v2, x, y, startEdge, circles))
                {
                    //if ((v1.E1 == e1 && v1.E2 == e2) || (v1.E2 == e1 && v1.E1 == e2))
                    if (!((v2.E1 == e1 && v2.E2 == e2) || (v2.E2 == e1 && v2.E1 == e2)))
                    {
                        v2.X = x;
                        v2.Y = y;
                        return true;
                    }
                }
            }

            x = v1.X;
            y = v1.Y;
            x = v2.X + (int)((double)(v1.X - v2.X) * (double)wantedLength / (double)currentLength);
            y = v2.Y + (int)((double)(v1.Y - v2.Y) * (double)wantedLength / (double)currentLength);

            if (x > 4 && y > 4 && x < width - 4 && y < height - 4)
            {
                //if (true) //can be changed
                if(CanBeSet(e1, v1, x, y, startEdge, circles))
                {
                    //if ((v2.E1 == e1 && v2.E2 == e2) || (v2.E2 == e1 && v2.E1 == e2))
                    if(!((v1.E1 == e1 && v1.E2 == e2) || (v1.E2 == e1 && v1.E1 == e2)))
                    {
                        v1.X = x;
                        v1.Y = y;
                        return true;
                    }
                }
            }


            return false;
        }


        public bool KeepParallel(Edge movE, Edge relE, Vertex movV, int xTo, int yTo, Edge startEdge, int circles)
        {
            var oldX = movV.X;
            var oldY = movV.Y;

            if (circles > 0 && movE == startEdge)
            {
                return false;
            }
            if (movE == startEdge && circles == 0)
                circles++;

            movV.X = xTo;
            movV.Y = yTo;

            if (movE.V1.X != movE.V2.X)
            {
                var wantedTan = Tan(movE.V1.X, movE.V1.Y, movE.V2.X, movE.V2.Y);

                var v1 = relE.V1;
                var v2 = relE.V2;

                var x = v1.X;
                var y = v1.Y;
                x = v2.X + (int)((double)(v1.Y - v2.Y) / wantedTan);

                if (x > 4 && x < width - 4)
                {
                    //if (true) //can be moved
                    if (CanBeSet(relE, v1, x, v1.Y, startEdge, circles))
                    {
                        v1.X = x;
                        return true;
                    }
                }

                x = v1.X;

                y = v2.Y + (int)((double)(v1.X - v2.X) * wantedTan);
                if (y > 4 && y < height - 4)
                {
                    //if(true) //can be moved
                    if (CanBeSet(relE, v1, v1.X, y, startEdge, circles))
                    {
                        v1.Y = y;
                        return true;
                    }
                }

                v1 = relE.V2;
                v2 = relE.V1;

                x = v1.X;
                y = v1.Y;

                x = v2.X + (int)((double)(v1.Y - v2.Y) / wantedTan);

                if (x > 4 && x < width - 4)
                {
                    //if (true) //can be moved
                    if (CanBeSet(relE, v1, x, v1.Y, startEdge, circles))
                    {
                        v1.X = x;
                        return true;
                    }
                }

                x = v1.X;

                y = v2.Y + (int)((double)(v1.X - v2.X) * wantedTan);
                if (y > 4 && y < height - 4)
                {
                    //if (true) //can be moved
                    if (CanBeSet(relE, v1, v1.X, y, startEdge, circles))
                    {
                        v1.Y = y;
                        return true;
                    }
                }
            }
            else // x != x
            {
                //if (true) //can be moved
                if (CanBeSet(relE, relE.V1, relE.V2.X, relE.V1.Y, startEdge, circles))
                {
                    relE.V1.X = relE.V2.X;
                    return true;
                }

                //if (true) //can be moved
                if (CanBeSet(relE, relE.V2, relE.V1.X, relE.V2.Y, startEdge, circles))
                {
                    relE.V2.X = relE.V1.X;
                    return true;
                }
            }


            movV.X = oldX;
            movV.Y = oldY;

            return false;
        }


        public bool KeepParallel22(Edge movE, Edge relE, Vertex movV, int xTo, int yTo, Edge startEdge, int circles)
        {
            if (movV.X == xTo && movV.Y == yTo)
                return true;
            if (circles > 0 && movE == startEdge)
            {
                return false;
            }
            if (movE == startEdge && circles == 0)
                circles++;

            var secEdgeOfMovV = movE != movV.E1 ? movV.E1 : movV.E2;

            //if (!CanBeSet(secEdgeOfMovV, movV, xTo, yTo, startEdge, 0))
            //    return false;

            var anV = movE.V1 != movV ? movE.V1 : movE.V2;

            var x = anV.X;
            var y = anV.Y;

            x = x + xTo - movV.X;
            y = y + yTo - movV.Y;

            if (x > 4 && x < width - 4 && y > 4 && y < height - 4)
            {
                //if (true) // can be moved
                //if(CanBeSet(movE, movV, xTo, yTo, startEdge, circles) 
                //   && CanBeSet(movE, anV, x, y, startEdge, circles))
                if(CanBeSet(movE, anV, x, y, startEdge, circles))
                {
                    movV.X = xTo;
                    movV.Y = yTo;
                    anV.X = x;
                    anV.Y = y;
                    return true;
                }
            }


            return false;
        }

        public bool SetParallel(Edge e1, Edge e2, Edge startEdge, int circles)
        {
            if (circles > 0 && e2 == startEdge)
            {
                return false;
            }
            if (e2 == startEdge && circles >= 1)
                circles++;

            if (e1.V1.X != e1.V2.X)
            {
                var wantedTan = Tan(e1.V1.X, e1.V1.Y, e1.V2.X, e1.V2.Y);

                var v1 = e2.V1;
                var v2 = e2.V2;

                var x = v1.X;
                var y = v1.Y;
                x = v2.X + (int)((double)(v1.Y - v2.Y) / wantedTan);

                if (x > 4 && x < width - 4)
                {
                    //if (true) //can be moved
                    if(CanBeSet(e2, v1, x, v1.Y, startEdge, circles))
                    {
                        v1.X = x;
                        return true;
                    }
                }

                x = v1.X;

                y = v2.Y + (int) ((double) (v1.X - v2.X) * wantedTan);
                if (y > 4 && y < height - 4)
                {
                    //if(true) //can be moved
                    if(CanBeSet(e2, v1, v1.X, y, startEdge, circles))
                    {
                        v1.Y = y;
                        return true;
                    }
                }

                v1 = e2.V2;
                v2 = e2.V1;

                x = v1.X;
                y = v1.Y;

                x = v2.X + (int)((double)(v1.Y - v2.Y) / wantedTan);

                if (x > 4 && x < width - 4)
                {
                    //if (true) //can be moved
                    if(CanBeSet(e2, v1, x, v1.Y, startEdge, circles))
                    {
                        v1.X = x;
                        return true;
                    }
                }

                x = v1.X;

                y = v2.Y + (int)((double)(v1.X - v2.X) * wantedTan);
                if (y > 4 && y < height - 4)
                {
                    //if (true) //can be moved
                    if(CanBeSet(e2, v1, v1.X, y, startEdge, circles))
                    {
                        v1.Y = y;
                        return true;
                    }
                }
            }
            else // x != x
            {
                //if (true) //can be moved
                if(CanBeSet(e2, e2.V1, e2.V2.X, e2.V1.Y, startEdge, circles))
                {
                    e2.V1.X = e2.V2.X;
                    return true;
                }

                //if (true) //can be moved
                if(CanBeSet(e2, e2.V2, e2.V1.X, e2.V2.Y, startEdge, circles))
                {
                    e2.V2.X = e2.V1.X;
                    return true;
                }
            }

            return false;
        }


        public bool CanBeSet(Edge e, Vertex v2, int x, int y, Edge startEdge, int circles)
        {
            var secEdge = v2.E1 != e ? v2.E1 : v2.E2;
            if (circles > 0 && secEdge == startEdge)
            {
                return false;
            }
            if (secEdge == startEdge && circles == 0)
                circles++;

            if (secEdge.RelType == TypeOfRelation.Equal)
            {
                return KeepEqualLength(secEdge, secEdge.RelatedEdge, v2, x, y, startEdge, circles);
            }
            else if(secEdge.RelType == TypeOfRelation.Parallel)
            {
                return KeepParallel(secEdge, secEdge.RelatedEdge, v2, x, y, startEdge, circles);
            }

            return secEdge.RelType == TypeOfRelation.None;

            

            //if (anE.RelType == TypeOfRelation.None)
            //{
            //    return true;
            //}
            //else if (anE.RelType == TypeOfRelation.Equal)
            //{
            //    var anV = anE.V1 != v2 ? anE.V1 : anE.V2;

            //    if (anE.RelatedEdge.Length + 10 >= Length(anV.X, anV.Y, x, y))
            //        return true;
            //    else
            //        return false;
            //}
            //return true;
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
