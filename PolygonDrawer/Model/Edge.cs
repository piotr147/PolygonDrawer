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
        private ObservableCollection<(Edge, TypeOfRelation)> _relatedEdges;
        private ObservableCollection<Edge> _neighbors;

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

        public ObservableCollection<(Edge, TypeOfRelation)> RelatedEdges
        {
            get { return _relatedEdges; }
            set { _relatedEdges = value; RaisePropertyChanged(nameof(RelatedEdges)); }
        }

        public ObservableCollection<Edge> Neighbors
        {
            get { return _neighbors; }
            set { _neighbors = value; RaisePropertyChanged(nameof(Neighbors)); }
        }

        public Edge(Vertex v1)
        {
            this.V1 = v1;
            RelatedEdges = new ObservableCollection<(Edge, TypeOfRelation)>();
        }

        public Edge(Vertex v1, Vertex v2)
        {
            this.V1 = v1;
            this.V2 = v2;
            RelatedEdges = new ObservableCollection<(Edge, TypeOfRelation)>();
        }


    }
}
