using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace PolygonDrawer.Converters
{
    public class MouseEventArgsConverter : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            var args = (MouseEventArgs)value;
            var element = (FrameworkElement)parameter;
            var point = args.GetPosition(element);

            var mouseData = new MouseData();
            mouseData.PosX = point.X;
            mouseData.PosY = point.Y;
            mouseData.IsClicked = args.LeftButton == MouseButtonState.Pressed;

            return mouseData;
        }
    }
}
