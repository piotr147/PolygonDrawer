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
using System.Windows.Media.Animation;
using PolygonDrawer.Converters;
using PolygonDrawer.Model;


namespace PolygonDrawer.ViewModel
{
    public static class Drawer
    {


        public static void Bresenham(WriteableBitmap bitmap, int x1, int y1, int x2, int y2, int r = 255, int g = 0, int b = 0)
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
                    x = x1; y = y1; xe = x2;
                }
                else
                { // Line is drawn right to left (swap ends)
                    x = x2; y = y2; xe = x1;
                }

                DrawPixel(bitmap, x, y); // Draw first pixel

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
                    DrawPixel(bitmap, x, y);
                }

            }
            else
            { // The line is Y-axis dominant

                // Line is drawn bottom to top
                if (dy >= 0)
                {
                    x = x1; y = y1; ye = y2;
                }
                else
                { // Line is drawn top to bottom
                    x = x2; y = y2; ye = y1;
                }

                DrawPixel(bitmap, x, y); // Draw first pixel

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
                    DrawPixel(bitmap, x, y);
                }
            }

            

            ////if (!(x1 < x2 && y1 > y2))
            //if (!(x1 < x2))
            //{
            //    int temp = x1;
            //    x1 = x2;
            //    x2 = temp;
            //    temp = y1;
            //    y1 = y2;
            //    y2 = temp;
            //}
            //if (y1 > y2)
            //{
            //    int m_new = 2 * (y1 - y2);
            //    int slope_error_new = m_new - (x2 - x1);

            //    for (int x = x1, y = y1; x <= x2; x++)
            //    {
            //        DrawPixel(bitmap, x, y);

            //        // Add slope to increment angle formed 
            //        slope_error_new += m_new;

            //        // Slope error reached limit, time to 
            //        // increment y and update slope error. 
            //        if (slope_error_new >= 0)
            //        {
            //            y--;
            //            slope_error_new -= 2 * (x2 - x1);
            //        }
            //    }
            //}
            //else
            //{
            //    y2 = 2 * x2 - y2;

            //    int m_new = 2 * (y1 - y2);
            //    int slope_error_new = m_new - (x2 - x1);

            //    for (int x = x1, y = y1; x <= x2; x++)
            //    {
            //        DrawPixel(bitmap, x, 2*x-y);

            //        // Add slope to increment angle formed 
            //        slope_error_new += m_new;

            //        // Slope error reached limit, time to 
            //        // increment y and update slope error. 
            //        if (slope_error_new >= 0)
            //        {
            //            y--;
            //            slope_error_new -= 2 * (x2 - x1);
            //        }
            //    }
            //}



        }

        public static void EraseEdge(WriteableBitmap bitmap, Edge e)
        {
            Bresenham(bitmap,e.V1.X, e.V1.Y, e.V2.X, e.V2.Y, 0, 0, 0);
        }

        public static void DrawEdge(WriteableBitmap bitmap, Edge e)
        {
            Bresenham(bitmap, e.V1.X, e.V1.Y, e.V2.X, e.V2.Y);
        }
        public static void DrawPixel(WriteableBitmap bitmap, int x, int y, int r = 255, int g = 0, int b = 0)
        {
            byte blue = (byte)b;
            byte green = (byte)g;
            byte red = (byte)r;
            byte alpha = 255;

            byte[] colorData = { blue, green, red, alpha };

            Int32Rect rect = new Int32Rect(x, y, 1, 1);
            bitmap.WritePixels(rect, colorData, 4, 0);

        }

        public static void DrawVertex(WriteableBitmap bitmap, Vertex v)
        {
            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    DrawPixel(bitmap, v.X + i, v.Y +j);
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

    }
}
