using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PolygonDrawer.Model
{
    public class Polygon : ObservableObject
    {
        private ObservableCollection<Vertex> _vertices;
        private ObservableCollection<Edge> _edges;

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
            this.Vertices = new ObservableCollection<Vertex>(vertices);
            this.Edges = new ObservableCollection<Edge>(edges);
        }

    }
}
