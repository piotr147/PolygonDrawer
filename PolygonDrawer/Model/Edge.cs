using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PolygonDrawer.Model
{
    public class Edge : ObservableObject
    {
        private Vertex _v1;
        private Vertex _v2;

        public Vertex V1
        {
            get { return _v1; }
            set { _v1 = value; RaisePropertyChanged(nameof(V1));}
        }

        public Vertex V2
        {
            get { return _v2; }
            set { _v2 = value; RaisePropertyChanged(nameof(V2)); }
        }

        public Edge(Vertex v1)
        {
            this.V1 = v1;
        }

        public Edge(Vertex v1, Vertex v2)
        {
            if (v1.X > v2.X)
            {
                this.V1 = v2;
                this.V2 = v1;
            }
            this.V1 = v1;
            this.V2 = v2;
        }


    }
}
