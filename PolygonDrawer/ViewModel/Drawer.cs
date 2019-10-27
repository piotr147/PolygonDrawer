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
using System.Security.Cryptography.X509Certificates;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using PolygonDrawer.Converters;
using PolygonDrawer.Model;


namespace PolygonDrawer.ViewModel
{
    public static class Drawer
    {


        public static void Bresenham(WriteableBitmap bitmap, int x1, int y1, int x2, int y2, int r = 255, int g = 0,
            int b = 0)
        {
            int x, y, dx, dy, dx1, dy1, px, py, xe, ye, i;

            // Calculate line deltas
            dx = x2 - x1;
            dy = y2 - y1;

            // Create a positive copy of deltas (makes iterating easier)
            dx1 = dx >= 0 ? dx : -dx;
            dy1 = dy >= 0 ? dy : -dy;

            // Calculate error intervals for both axis
            px = 2 * dy1 - dx1;
            py = 2 * dx1 - dy1;

            // The line is X-axis dominant
            if (dy1 <= dx1)
            {

                // Line is drawn left to right
                if (dx >= 0)
                {
                    x = x1;
                    y = y1;
                    xe = x2;
                }
                else
                {
                    // Line is drawn right to left (swap ends)
                    x = x2;
                    y = y2;
                    xe = x1;
                }

                DrawPixel(bitmap, x, y, r, g, b); // Draw first pixel

                // Rasterize the line
                for (i = 0; x < xe; i++)
                {
                    x = x + 1;

                    // Deal with octants...
                    if (px < 0)
                    {
                        px = px + 2 * dy1;
                    }
                    else
                    {
                        if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0))
                        {
                            y = y + 1;
                        }
                        else
                        {
                            y = y - 1;
                        }

                        px = px + 2 * (dy1 - dx1);
                    }

                    // Draw pixel from line span at
                    // currently rasterized position
                    DrawPixel(bitmap, x, y, r, g, b);
                }

            }
            else
            {
                // The line is Y-axis dominant

                // Line is drawn bottom to top
                if (dy >= 0)
                {
                    x = x1;
                    y = y1;
                    ye = y2;
                }
                else
                {
                    // Line is drawn top to bottom
                    x = x2;
                    y = y2;
                    ye = y1;
                }

                DrawPixel(bitmap, x, y, r, g, b); // Draw first pixel

                // Rasterize the line
                for (i = 0; y < ye; i++)
                {
                    y = y + 1;

                    // Deal with octants...
                    if (py <= 0)
                    {
                        py = py + 2 * dx1;
                    }
                    else
                    {
                        if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0))
                        {
                            x = x + 1;
                        }
                        else
                        {
                            x = x - 1;
                        }

                        py = py + 2 * (dx1 - dy1);
                    }

                    // Draw pixel from line span at
                    // currently rasterized position
                    DrawPixel(bitmap, x, y, r, g, b);
                }
            }
        }

        public static void EraseEdge(WriteableBitmap bitmap, Edge e)
        {
            Bresenham(bitmap, e.V1.X, e.V1.Y, e.V2.X, e.V2.Y, 0, 0, 0);
        }

        public static void DrawEdge(WriteableBitmap bitmap, Edge e)
        {
            Bresenham(bitmap, e.V1.X, e.V1.Y, e.V2.X, e.V2.Y);
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
            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    DrawPixel(bitmap, v.X + i, v.Y + j);
                }
            }
        }

        public static void DrawVertex(WriteableBitmap bitmap, int x, int y)
        {
            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    DrawPixel(bitmap, x + i, y + j);
                }
            }
        }

        public static void EraseVertex(WriteableBitmap bitmap, Vertex v)
        {
            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    DrawPixel(bitmap, v.X + i, v.Y + j, 0, 0, 0);
                }
            }
        }

        public static void EdgeOneSideMoving(WriteableBitmap bitmap, Edge e, int x1, int y1, int x2, int y2)
        {
            if (e.V1.X == x1 && e.V1.Y == y1)
            {
                Bresenham(bitmap, e.V2.X, e.V2.Y, x1, y1, 0, 0, 0);
                Bresenham(bitmap, e.V2.X, e.V2.Y, x2, y2);
            }
            else
            {
                Bresenham(bitmap, e.V1.X, e.V1.Y, x1, y1, 0, 0, 0);
                Bresenham(bitmap, e.V1.X, e.V1.Y, x2, y2);
            }
        }

        public static void VertexMoving(WriteableBitmap bitmap, Vertex v, int x, int y)
        {
            EraseVertex(bitmap, v);
            DrawVertex(bitmap, x, y);
        }










        public static bool BresenhamBool(WriteableBitmap bitmap, int x1, int y1, int x2, int y2, int xs, int ys)
        {
            int x, y, dx, dy, dx1, dy1, px, py, xe, ye, i;

            // Calculate line deltas
            dx = x2 - x1;
            dy = y2 - y1;

            // Create a positive copy of deltas (makes iterating easier)
            dx1 = dx >= 0 ? dx : -dx;
            dy1 = dy >= 0 ? dy : -dy;

            // Calculate error intervals for both axis
            px = 2 * dy1 - dx1;
            py = 2 * dx1 - dy1;

            // The line is X-axis dominant
            if (dy1 <= dx1)
            {

                // Line is drawn left to right
                if (dx >= 0)
                {
                    x = x1;
                    y = y1;
                    xe = x2;
                }
                else
                {
                    // Line is drawn right to left (swap ends)
                    x = x2;
                    y = y2;
                    xe = x1;
                }

                if (x <= xs + 3 && x >= xs - 3 && y <= ys + 3 && y >= ys - 3)
                    return true;

                // Rasterize the line
                for (i = 0; x < xe; i++)
                {
                    x = x + 1;

                    // Deal with octants...
                    if (px < 0)
                    {
                        px = px + 2 * dy1;
                    }
                    else
                    {
                        if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0))
                        {
                            y = y + 1;
                        }
                        else
                        {
                            y = y - 1;
                        }

                        px = px + 2 * (dy1 - dx1);
                    }

                    // Draw pixel from line span at
                    // currently rasterized position
                    if (x <= xs + 3 && x >= xs - 3 && y <= ys + 3 && y >= ys - 3)
                        return true;
                }

            }
            else
            {
                // The line is Y-axis dominant

                // Line is drawn bottom to top
                if (dy >= 0)
                {
                    x = x1;
                    y = y1;
                    ye = y2;
                }
                else
                {
                    // Line is drawn top to bottom
                    x = x2;
                    y = y2;
                    ye = y1;
                }

                if (x <= xs + 3 && x >= xs - 3 && y <= ys + 3 && y >= ys - 3)
                    return true;

                // Rasterize the line
                for (i = 0; y < ye; i++)
                {
                    y = y + 1;

                    // Deal with octants...
                    if (py <= 0)
                    {
                        py = py + 2 * dx1;
                    }
                    else
                    {
                        if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0))
                        {
                            x = x + 1;
                        }
                        else
                        {
                            x = x - 1;
                        }

                        py = py + 2 * (dx1 - dy1);
                    }

                    // Draw pixel from line span at
                    // currently rasterized position
                    if (x <= xs + 3 && x >= xs - 3 && y <= ys + 3 && y >= ys - 3)
                        return true;
                }
            }

            return false;
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
