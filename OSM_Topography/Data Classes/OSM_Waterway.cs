using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM_Topography
{
    class OSM_Waterway
    {
        public string ID { get; set; }

        public string Type { get; set; }

        public double Distance { get; set; } = 0;

        public double TotalAngle { get; set; } = 0;

        public double StartingElevation { get; set; }

        public double EndingElevation { get; set; }

        public int ParentStreams { get; set; }

        public HashSet<string> NodeList { get; set; }

        public List<OSM_Node> NodesToKeep { get; set; } = new List<OSM_Node>();
    }
}
