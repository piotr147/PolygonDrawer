﻿using System;
using System.Windows;
using System.Windows.Media.Imaging;

using PolygonDrawer.Model;


namespace PolygonDrawer.ViewModel
{
    public static class Drawer
    {

        public static void Bresenham(WriteableBitmap bitmap, int x1, int y1, int x2, int y2, int r = 255, int g = 0,
            int b = 0)
        {
            int dx = Math.Abs(x2 - x1);
            int sx = x1 < x2 ? 1 : -1;
            int dy = Math.Abs(y2 - y1);
            int sy = y1 < y2 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2;
            int e2;

            while(true)
            {
                DrawPixel(bitmap, x1, y1, r, g, b);

                if (x2 == x1 && y2 == y1)
                    return;
                e2 = err;
                if (e2 > -dx)
                {
                    err -= dy;
                    x1 += sx;
                }
                if (e2 < dy)
                {
                    err += dx;
                    y1 += sy;
                }
            }
        }


        public static void DrawEdge(WriteableBitmap bitmap, Edge e)
        {
            Bresenham(bitmap, e.V1.X, e.V1.Y, e.V2.X, e.V2.Y);
        }

        public static void DrawEdge(WriteableBitmap bitmap, Edge e, int r, int g, int b)
        {
            Bresenham(bitmap, e.V1.X, e.V1.Y, e.V2.X, e.V2.Y, r, g, b);
        }

        public static void DrawEdge(WriteableBitmap bitmap, Vertex v1, Vertex v2)
        {
            Bresenham(bitmap, v1.X, v1.Y,v2.X, v2.Y);
        }

        public static void DrawPixel(WriteableBitmap bitmap, int x, int y, int r = 255, int g = 0, int b = 0)
        {
            byte blue = (byte) b;
            byte green = (byte) g;
            byte red = (byte) r;
            byte alpha = 255;

            byte[] colorData = {blue, green, red, alpha};

            Int32Rect rect = new Int32Rect(x, y, 1, 1);
            bitmap.WritePixels(rect, colorData, 4, 0);

        }

        public static void DrawVertex(WriteableBitmap bitmap, Vertex v)
        {
            if (!v.IsFixed)
            {
                for (int i = -2; i <= 2; i++)
                {
                    for (int j = -2; j <= 2; j++)
                    {
                        DrawPixel(bitmap, v.X + i, v.Y + j);
                    }
                }
            }
            else
            {
                for (int i = -4; i <= 4; i++)
                {
                    for (int j = -4; j <= 4; j++)
                    {
                        if (Math.Abs(i) == Math.Abs(j) || Math.Abs(i - 1) == Math.Abs(j) || Math.Abs(i) == Math.Abs(j - 1)
                            || Math.Abs(i + 1) == Math.Abs(j) || Math.Abs(i) == Math.Abs(j + 1))
                        {
                            DrawPixel(bitmap, v.X + i, v.Y + j, 0, 0, 0);
                        }
                        else
                        {
                            DrawPixel(bitmap, v.X + i, v.Y + j, 255, 255, 255);
                        }
                    }
                }
            }
        }

        public static bool BresenhamBool(WriteableBitmap bitmap, int x1, int y1, int x2, int y2, int xs, int ys)
        {
            int dx = Math.Abs(x2 - x1);
            int sx = x1 < x2 ? 1 : -1;
            int dy = Math.Abs(y2 - y1);
            int sy = y1 < y2 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2;
            int e2;

            while (true)
            {
                //DrawPixel(bitmap, x1, y1, r, g, b);
                if (xs > x1 - 3 && xs < x1 + 3 && ys > y1 - 3 && ys < y1 + 3)
                    return true;

                if (x2 == x1 && y2 == y1)
                    return false;
                e2 = err;
                if (e2 > -dx)
                {
                    err -= dy;
                    x1 += sx;
                }
                if (e2 < dy)
                {
                    err += dx;
                    y1 += sy;
                }
            }

            //return false;
        }

        public static bool DrawMark(WriteableBitmap bitmap, Edge e, int r, int g, int b)
        {
            if(e.RelType == TypeOfRelation.None)
            {
                return true;
            }

            if (e.RelType == TypeOfRelation.Parallel)
            {
                DrawParallelMark(bitmap, e, r, g, b);
            }

            if (e.RelType == TypeOfRelation.Equal)
            {
                DrawEqualMark(bitmap, e, r, g, b);
            }
            return false;
        }

        private static void DrawEqualMark(WriteableBitmap bitmap, Edge e, int r, int g, int b)
        {
            DrawColorVertexEq(bitmap, (e.V1.X + e.V2.X) / 2, (int)(e.V1.Y + e.V2.Y) / 2, r, g, b);
        }

        private static bool DrawParallelMark(WriteableBitmap bitmap,Edge e, int r, int g, int b)
        {
            DrawColorVertexPar(bitmap, (e.V1.X + e.V2.X) / 2, (int)(e.V1.Y + e.V2.Y) / 2, r, g, b);


            return true;
        }

        private static void DrawColorVertexEq(WriteableBitmap bitmap, int x, int y, int r, int g, int b)
        {
            for (int i = -4; i <= 4; i++)
            {
                for (int j = -4; j <= 4; j++)
                {
                    DrawPixel(bitmap, x + i, y + j, r, g, b);
                }
            }
        }

        private static void DrawColorVertexPar(WriteableBitmap bitmap, int x, int y, int r, int g, int b)
        {
            for (int i = -4; i <= 4; i++)
            {
                for (int j = -4; j <= 4; j++)
                {
                    if (j == 1 || j == 2 || j == 3 || j == -1 || j == -2 || j == -3)
                        DrawPixel(bitmap, x + i, y + j, 0, 0, 0);
                    else
                        DrawPixel(bitmap, x + i, y + j, r, g, b);
                }
            }
        }
    }
}
