using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM_Topography
{
    class Vertice
    {
        public string ID { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Elevation { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public Dictionary<string, Vertice> EdgeConnectionDict { get; set; } = new Dictionary<string, Vertice>();

        public Dictionary<int, Tuple<string, double>> ClosestVertices { get; set; } = new Dictionary<int, Tuple<string, double>>();

        public int CurrentCheck { get; set; } = 0;

        public int Rank { get; set; }

        public bool IsPeak { get; set; } = false;
    }
}
