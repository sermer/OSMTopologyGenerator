using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM_Topography
{
    class OSM_Road
    {
        public string WayID { get; set; }

        public HashSet<string> NodeList { get; set; } = new HashSet<string>();

        public double LengthMiles { get; set; }

        public double CumulativeTurnAngle { get; set; }
    }
}
