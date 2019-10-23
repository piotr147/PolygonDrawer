using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private Edge _relatedEdge;
        private TypeOfRelation _relType;
        private ObservableCollection<Edge> _neighbors;
        private int _length;

        public Vertex V1
        {
            get { return _v1; }
            set
            {
                _v1 = value;
                RaisePropertyChanged(nameof(V1));
                //if(V2 != null)
                //    Length = (int)Math.Sqrt(((V1.X - V2.X) * (V1.X - V2.X) + (V1.Y - V2.Y) * (V1.Y - V2.Y)));
            }
        }

        public Vertex V2
        {
            get { return _v2; }
            set
            {
                _v2 = value;
                RaisePropertyChanged(nameof(V2));
                //if(V1 != null)
                //    Length = (int)Math.Sqrt(((V1.X - V2.X) * (V1.X - V2.X) + (V1.Y - V2.Y) * (V1.Y - V2.Y)));
            }
        }

        public int Length
        {
            //get { return _length; }
            //private set { _length = value; RaisePropertyChanged(nameof(Length)); }
            get { return (int)Math.Sqrt(((V1.X - V2.X) * (V1.X - V2.X) + (V1.Y - V2.Y) * (V1.Y - V2.Y))); }
            set
            {
                //TryToAdjustEdge(value, V1);
            }
        }
        

        public Edge RelatedEdge
        {
            get { return _relatedEdge; }
            set { _relatedEdge = value; RaisePropertyChanged(nameof(RelatedEdge)); }
        }

        public TypeOfRelation RelType
        {
            get { return _relType; }
            set { _relType = value; RaisePropertyChanged(nameof(RelType)); }
        }

        public ObservableCollection<Edge> Neighbors
        {
            get { return _neighbors; }
            set { _neighbors = value; RaisePropertyChanged(nameof(Neighbors)); }
        }

        public Edge(Vertex v1)
        {
            this.V1 = v1;
            RelType = TypeOfRelation.None;
        }

        public Edge(Vertex v1, Vertex v2)
        {
            this.V1 = v1;  
            this.V2 = v2;
            RelType = TypeOfRelation.None;
        }

        //public bool TryToAdjustEdge(int len, Vertex v)
        //{
        //    var anV = V1 == v ? V2 : V1;
        //    var anE = anV.E1 == this ? anV.E2 : anV.E1;

        //    if (anE.CanBeMoved())
        //    {





        //    }


        //}

        public bool CanBeMoved()
        {
            return RelatedEdge == null;
        }

        //public bool CanBeMovedHere(int)
    }
}
