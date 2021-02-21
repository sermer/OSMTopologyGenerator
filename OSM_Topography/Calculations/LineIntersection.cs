using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM_Topography
{
    class LineIntersection
    {
        //(Longitude, Latitude)
        public static double SolveM(Vertice v1, Vertice v2)
        {
            return (v2.Latitude - v1.Latitude) / (v2.Longitude - v1.Longitude);
        }

        public static double SolveB(Vertice v1, double m)
        {
            return (v1.Latitude - m * v1.Longitude);
        }

        public static bool DoLinesIntersect(Vertice v1, Vertice v2, Edge e)
        {
            double m = (v2.Latitude - v1.Latitude) / (v2.Longitude - v1.Longitude);
            double b = (v1.Latitude - m * v1.Longitude);

            double x = Math.Round((e.B - b) / (m - e.M), 6);
            //double y = Math.Round((m * x) + b, 6);

            if(e.V1.Longitude <= e.V2.Longitude)
            {
                if(v1.Longitude <= v2.Longitude)
                {
                    return (x > Math.Round(e.V1.Longitude, 6) && x < Math.Round(e.V2.Longitude, 6) && x > Math.Round(v1.Longitude, 6) && x < Math.Round(v2.Longitude, 6));
                }
                else
                {
                    return (x > Math.Round(e.V1.Longitude, 6) && x < Math.Round(e.V2.Longitude, 6) && x > Math.Round(v2.Longitude, 6) && x < Math.Round(v1.Longitude, 6));
                }
            }
            else
            {
                if (v1.Longitude <= v2.Longitude)
                {
                    return (x > Math.Round(e.V2.Longitude, 6) && x < Math.Round(e.V1.Longitude, 6) && x > Math.Round(v1.Longitude, 6) && x < Math.Round(v2.Longitude, 6));
                }
                else
                {
                    return (x > Math.Round(e.V2.Longitude, 6) && x < Math.Round(e.V1.Longitude, 6) && x > Math.Round(v2.Longitude, 6) && x < Math.Round(v1.Longitude, 6));
                }
                
            }
        }
    }
}
