using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM_Topography
{
    class Face
    {
        public string ID { get; set; }

        public List<Vertice> Vertices { get; set; } = new List<Vertice>();
    }
}
