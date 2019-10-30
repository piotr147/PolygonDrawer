using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

//using GalaSoft.MvvmLight.

namespace PolygonDrawer.Model
{
    public class Vertex : ObservableObject
    {
        private int _x;
        private int _y;
        private Edge _e1;
        private Edge _e2;
        private bool _isFixed;

        public bool IsFixed
        {
            get { return _isFixed; }
            set { _isFixed = value; RaisePropertyChanged(nameof(IsFixed)); }
        }

        public int X
        {
            get { return _x; }
            set { _x = value; RaisePropertyChanged(nameof(X)); }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; RaisePropertyChanged(nameof(Y)); }
        }

        public Edge E1
        {
            get { return _e1; }
            set { _e1 = value; RaisePropertyChanged(nameof(E1)); }
        }
        public Edge E2
        {
            get { return _e2; }
            set { _e2 = value; RaisePropertyChanged(nameof(E2)); }
        }

        public Vertex(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public void AddEdge(Edge e)
        {
            if (E1 == null)
            {
                this.E1 = e;
            }
            else if (E2 == null)
            {
                this.E2 = e;
            }
        }

        public void DeleteEdge()
        {
            if (E2 != null)
            {
                E2 = null;
            }
            else if (E1 != null)
            {
                E1 = null;
            }
        }
    }
}
