using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM_Topography
{
    class Edge
    {
        public Vertice V1 { get; set; }

        public Vertice V2 { get; set; }

        public int TriangleCount { get; set; } = 0;

        public string Key { get; set; }

        public double M { get; set; }

        public double B { get; set; }
    }
}
