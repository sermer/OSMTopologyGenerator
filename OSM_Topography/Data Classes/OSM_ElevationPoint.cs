using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM_Topography
{
    class OSM_ElevationPoint
    {
        public string NodeID { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Elevation { get; set; } = 0;

        public bool IsPeak { get; set; } = false;

        public int Rank { get; set; }
    }
}
