using System;
using System.Collections.ObjectModel;
using System.Configuration;
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

        public bool KeepEqualLength(Edge movE, Edge relE, Vertex movV, int xTo, int yTo, Edge startEdge, int circles, int count)
        {
            var secEdgeOfMovV = movE != movV.E1 ? movV.E1 : movV.E2;

            if (count > 3 * this.Vertices.Count)
                return false;
            count++;

            if ((circles > 0 && movE == startEdge) || circles >= 2)
            {
                return false;
            }
            if (movE == startEdge && circles == 0)
                circles++;

            if (secEdgeOfMovV.RelType == TypeOfRelation.Parallel)
            {
                if (!KeepParallel(secEdgeOfMovV, secEdgeOfMovV.RelatedEdge, movV, xTo, yTo, startEdge, circles, count))
                    return false;
            }

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
                if(CanBeSet(relE, v1, x, y, startEdge, circles, count))
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
                if (CanBeSet(relE, v1, x, y, startEdge, circles, count))
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


        public bool SetEqualLength(Edge e1, Edge e2, Edge startEdge, int circles, int count)
        {
            if (count > 3 * this.Vertices.Count)
                return false;
            count++;

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

            var x = v2.X;
            var y = v2.Y;

            x = v1.X + (int) ((double) (v2.X - v1.X) * (double) wantedLength / (double) currentLength);
            y = v1.Y + (int) ((double) (v2.Y - v1.Y) * (double) wantedLength / (double) currentLength);

            if (x > 4 && y > 4 && x < width - 4 && y < height - 4)
            {
                if(CanBeSet(e2, v2, x, y, startEdge, circles, count))
                {
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
                if(CanBeSet(e2, v1, x, y, startEdge, circles, count))
                {
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
                if(CanBeSet(e1, v2, x, y, startEdge, circles, count))
                {
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
                if(CanBeSet(e1, v1, x, y, startEdge, circles, count))
                {
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


        public bool KeepParallel(Edge movE, Edge relE, Vertex movV, int xTo, int yTo, Edge startEdge, int circles, int count)
        {
            if (count > 3 * this.Vertices.Count)
                return false;
            count++; 

            var oldX = movV.X;
            var oldY = movV.Y;

            if ((circles > 0 && movE == startEdge) || circles >= 2)
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
                    if (CanBeSet(relE, v1, x, v1.Y, startEdge, circles, count))
                    {
                        v1.X = x;
                        return true;
                    }
                }

                x = v1.X;

                y = v2.Y + (int)((double)(v1.X - v2.X) * wantedTan);
                if (y > 4 && y < height - 4)
                {
                    if (CanBeSet(relE, v1, v1.X, y, startEdge, circles, count))
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
                    if (CanBeSet(relE, v1, x, v1.Y, startEdge, circles, count))
                    {
                        v1.X = x;
                        return true;
                    }
                }

                x = v1.X;

                y = v2.Y + (int)((double)(v1.X - v2.X) * wantedTan);
                if (y > 4 && y < height - 4)
                {
                    if (CanBeSet(relE, v1, v1.X, y, startEdge, circles, count))
                    {
                        v1.Y = y;
                        return true;
                    }
                }
            }
            else // x != x
            {
                if (CanBeSet(relE, relE.V1, relE.V2.X, relE.V1.Y, startEdge, circles, count))
                {
                    relE.V1.X = relE.V2.X;
                    return true;
                }

                if (CanBeSet(relE, relE.V2, relE.V1.X, relE.V2.Y, startEdge, circles, count))
                {
                    relE.V2.X = relE.V1.X;
                    return true;
                }
            }


            movV.X = oldX;
            movV.Y = oldY;

            return false;
        }

        public bool SetParallel(Edge e1, Edge e2, Edge startEdge, int circles, int count)
        {
            if (count > 3 * this.Vertices.Count)
                return false;
            count++;

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
                    if(CanBeSet(e2, v1, x, v1.Y, startEdge, circles, count))
                    {
                        v1.X = x;
                        return true;
                    }
                }

                x = v1.X;

                y = v2.Y + (int) ((double) (v1.X - v2.X) * wantedTan);
                if (y > 4 && y < height - 4)
                {
                    if(CanBeSet(e2, v1, v1.X, y, startEdge, circles, count))
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
                    if(CanBeSet(e2, v1, x, v1.Y, startEdge, circles, count))
                    {
                        v1.X = x;
                        return true;
                    }
                }

                x = v1.X;

                y = v2.Y + (int)((double)(v1.X - v2.X) * wantedTan);
                if (y > 4 && y < height - 4)
                {
                    if(CanBeSet(e2, v1, v1.X, y, startEdge, circles, count))
                    {
                        v1.Y = y;
                        return true;
                    }
                }
            }
            else // x != x
            {
                if(CanBeSet(e2, e2.V1, e2.V2.X, e2.V1.Y, startEdge, circles, count))
                {
                    e2.V1.X = e2.V2.X;
                    return true;
                }

                if(CanBeSet(e2, e2.V2, e2.V1.X, e2.V2.Y, startEdge, circles, count))
                {
                    e2.V2.X = e2.V1.X;
                    return true;
                }
            }

            return false;
        }


        public bool CanBeSet(Edge e, Vertex v2, int x, int y, Edge startEdge, int circles, int count)
        {
            if (v2.IsFixed)
            {
                return false;
            }

            var secEdge = v2.E1 != e ? v2.E1 : v2.E2;
            if (circles > 0 && secEdge == startEdge)
            {
                return false;
            }
            if (secEdge == startEdge && circles == 0)
                circles++;

            if (secEdge.RelType == TypeOfRelation.Equal)
            {
                return KeepEqualLength(secEdge, secEdge.RelatedEdge, v2, x, y, startEdge, circles, count);
            }
            else if(secEdge.RelType == TypeOfRelation.Parallel)
            {
                return KeepParallel(secEdge, secEdge.RelatedEdge, v2, x, y, startEdge, circles, count);
            }

            return secEdge.RelType == TypeOfRelation.None;
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

            return (bY - sY) / (bX - sX);
        }

    }
}
