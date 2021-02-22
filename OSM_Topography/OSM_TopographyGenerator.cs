using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using KdTree;
using KdTree.Math;

namespace OSM_Topography
{
    public partial class OSM_TopographyGenerator : Form
    {
        public OSM_TopographyGenerator()
        {
            InitializeComponent();
        }

        Dictionary<string, OSM_Road> wayDict;
        Dictionary<string, OSM_Node> nodeDict;
        Dictionary<string, OSM_ElevationPoint> elevationDict;
        Dictionary<string, OSM_Waterway> waterwayDict;
        Dictionary<string, HashSet<string>> stateBorderDict;

        string saveLoc;
        int rank = 0;
        private void RunButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(saveLoc))
            {
                MessageBox.Show("Please select a save location before running");
                return;
            }

            rank = 0;
            wayDict = new Dictionary<string, OSM_Road>();
            nodeDict = new Dictionary<string, OSM_Node>();
            elevationDict = new Dictionary<string, OSM_ElevationPoint>();
            waterwayDict = new Dictionary<string, OSM_Waterway>();
            stateBorderDict = new Dictionary<string, HashSet<string>>();
            int closestX = Convert.ToInt32(closestXTextBox.Text);
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "OSM files(*.osm)| *.osm";
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string file = openFileDialog.FileName;
                ParseOSM(file);
                
                if(closestX > elevationDict.Count())
                {
                    closestX = elevationDict.Count() - 4;
                }
                Dictionary<string, Vertice> verticeDict = BuildVerticeDict(elevationDict);
                Console.WriteLine("---------------");
                KdTree<double, string> tree = BuildTree(verticeDict);
                CollectWaterwayInfo(waterwayDict, verticeDict, tree);

                MergeDicts();
                verticeDict = BuildVerticeDict(elevationDict);
                elevationDict.Clear();
                waterwayDict.Clear();
                nodeDict.Clear();
                wayDict.Clear();

                //PrintVertices(verticeDict);
                //tree = BuildTree(verticeDict);

                BuildFaces(verticeDict, tree, closestX);
                TopMost = true;
                TopMost = false;
                MessageBox.Show("Done");
            }
            else
            {
                return;
            }
        }

        private void ParseOSM(string osmURL)
        {
            List<string> acceptedRoadTypeList = GetRoadTypeList();
            wayDict = new Dictionary<string, OSM_Road>();
            nodeDict = new Dictionary<string, OSM_Node>();
            elevationDict = new Dictionary<string, OSM_ElevationPoint>();
            waterwayDict = new Dictionary<string, OSM_Waterway>();
            stateBorderDict = new Dictionary<string, HashSet<string>>();
            using (XmlReader reader = XmlReader.Create(osmURL))
            {
                reader.MoveToContent();
                char[] splitChar = { '"' };

                string wayID = "";
                while (reader.Read())
                {
                    if (reader.Name.Equals("node"))
                    {
                        XElement thisNode = (XElement)XDocument.ReadFrom(reader);
                        OSM_Node node = new OSM_Node();
                        node.NodeID = Convert.ToString(thisNode.Attribute("id")).Split(splitChar)[1];
                        node.Latitude = Convert.ToDouble(Convert.ToString(thisNode.Attribute("lat")).Split(splitChar)[1]);
                        node.Longitude = Convert.ToDouble(Convert.ToString(thisNode.Attribute("lon")).Split(splitChar)[1]);
                        nodeDict[node.NodeID] = node;

                        var nodeElements = thisNode.Elements();
                        if (nodeElements.Count() > 0)
                        {
                            OSM_ElevationPoint elevationPoint = new OSM_ElevationPoint();
                            elevationPoint.NodeID = Convert.ToString(thisNode.Attribute("id")).Split(splitChar)[1];
                            elevationPoint.Latitude = Convert.ToDouble(Convert.ToString(thisNode.Attribute("lat")).Split(splitChar)[1]);
                            elevationPoint.Longitude = Convert.ToDouble(Convert.ToString(thisNode.Attribute("lon")).Split(splitChar)[1]);

                            foreach (var child in nodeElements)
                            {
                                if (child.Name.LocalName.Equals("tag"))
                                {
                                    if (Convert.ToString(child.Attribute("k")).Split(splitChar)[1] == "natural" && Convert.ToString(child.Attribute("v")).Split(splitChar)[1] == "peak")
                                    {
                                        elevationPoint.IsPeak = true;
                                    }
                                    if (Convert.ToString(child.Attribute("k")).Split(splitChar)[1] == "ele")
                                    {
                                        try
                                        {
                                            elevationPoint.Elevation = Convert.ToDouble(Convert.ToString(child.Attribute("v")).Split(splitChar)[1]);
                                        }
                                        catch
                                        {
                                            Console.WriteLine("NotaNumba!");
                                        }
                                    }
                                }
                            }

                            if (elevationPoint.Elevation > 0)
                            {
                                elevationDict[elevationPoint.NodeID] = elevationPoint;
                            }
                        }
                    }
                    else if (reader.Name.Equals("way"))
                    {
                        HashSet<string> nodeList = new HashSet<string>();
                        bool keepWay = false;
                        bool overrideKeepWay = false;

                        XElement thisWay = (XElement)XDocument.ReadFrom(reader);

                        wayID = Convert.ToString(thisWay.Attribute("id")).Split(splitChar)[1];

                        wayDict[wayID] = new OSM_Road();
                        var elements = thisWay.Elements();

                        foreach (var child in thisWay.Elements())
                        {
                            if (child.Name.LocalName.Equals("nd"))
                            {
                                if (Convert.ToString(child.Attribute("ref")) != "")
                                {
                                    nodeList.Add(Convert.ToString(child.Attribute("ref")).Split(splitChar)[1]);
                                }
                            }
                            else if (child.Name.LocalName.Equals("tag"))
                            {
                                if (Convert.ToString(child.Attribute("v")) != "" && Convert.ToString(child.Attribute("k")) != "" && acceptedRoadTypeList.Contains(Convert.ToString(child.Attribute("v")).Split(splitChar)[1]) && Convert.ToString(child.Attribute("k")).Split(splitChar)[1] == "highway")
                                {
                                    keepWay = true;
                                }
                                if (Convert.ToString(child.Attribute("k")).Split(splitChar)[1] == "waterway")
                                {
                                    OSM_Waterway waterway = new OSM_Waterway();
                                    waterway.NodeList = nodeList;
                                    waterway.Type = Convert.ToString(child.Attribute("v")).Split(splitChar)[1];
                                    waterway.ID = wayID;
                                    if(waterway.Type == "stream" || waterway.Type == "river")
                                    {
                                        waterwayDict[wayID] = waterway;
                                    }
                                }
                                if (Convert.ToString(child.Attribute("k")).Split(splitChar)[1] == "border_type" && Convert.ToString(child.Attribute("v")).Split(splitChar)[1] == "state")
                                {
                                    stateBorderDict[wayID] = nodeList;
                                }


                                if (Convert.ToString(child.Attribute("k")).Split(splitChar)[1] == "service")
                                {
                                    if (Convert.ToString(child.Attribute("v")).Split(splitChar)[1] == "driveway" || Convert.ToString(child.Attribute("v")).Split(splitChar)[1] == "parking_aisle" || Convert.ToString(child.Attribute("v")).Split(splitChar)[1] == "weigh_station" || Convert.ToString(child.Attribute("v")).Split(splitChar)[1] == "drive-through")
                                    {
                                        overrideKeepWay = true;
                                    }
                                }
                            }
                        }
                        if (!keepWay || wayDict[wayID].NodeList.Count < 2 || overrideKeepWay)
                        {
                            wayDict.Remove(wayID);
                        }
                        else
                        {
                            wayDict[wayID].NodeList = nodeList;
                        }
                        wayID = "";
                    }
                }
            }
        }

        private List<string> GetRoadTypeList()
        {
            List<string> roadTypeList = new List<string>();
            roadTypeList.Add("residential");
            roadTypeList.Add("living_street");
            roadTypeList.Add("motorway");
            roadTypeList.Add("motorway_link");
            roadTypeList.Add("primary");
            roadTypeList.Add("primary_link");
            roadTypeList.Add("secondary");
            roadTypeList.Add("secondary_link");
            roadTypeList.Add("tertiary");
            roadTypeList.Add("tertiary_link");
            roadTypeList.Add("trunk");
            roadTypeList.Add("trunk_link");
            roadTypeList.Add("pedestrian");
            roadTypeList.Add("service");
            roadTypeList.Add("unclassified");

            return roadTypeList;
        }

        private KdTree<double, string> BuildTree(Dictionary<string, Vertice> verticeDict)
        {
            KdTree<double, string> tree = new KdTree<double, string>(2, new DoubleMath());
            foreach (var vertice in verticeDict)
            {
                tree.Add(new[] { vertice.Value.Latitude, vertice.Value.Longitude }, vertice.Key);
            }
            return tree;
        }

        private Vertice CompareVerticeLat(Vertice v1, Vertice v2)
        {
            if (v1.Latitude < v2.Latitude)
            {
                return v1;
            }
            else
            {
                return v2;
            }
        }

        /*private Vertice BuildKDVerticeTree(List<Vertice> verticeList, double depth = 0)
        {
            int n = verticeList.Count();

            if(n == 0)
            {
                return null;
            }

            if(n == 1)
            {
                return verticeList[0];
            }

            if (depth % 2 == 0)
            {
                verticeList = verticeList.OrderBy(Vertice => Vertice.Latitude).ToList();
            }
            else
            {
                verticeList = verticeList.OrderBy(Vertice => Vertice.Longitude).ToList();
            }
            
            Vertice rootNode = new Vertice();

            rootNode = verticeList[n / 2];
            //Left branch
            //Console.WriteLine(rootNode.Latitude + "," + rootNode.Longitude + "," + depth + "," + n);
            Vertice leftChild = BuildKDVerticeTree(verticeList.GetRange(0, n / 2), depth + 1);
            if(leftChild != rootNode)
            {
                rootNode.LeftChild = leftChild;
            }
            else
            {
                rootNode.LeftChild = null;
            }

            Vertice rightChild = BuildKDVerticeTree(verticeList.GetRange((n / 2), n / 2), depth + 1);
            //Right branch
            if (rightChild != rootNode)
            {
                rootNode.RightChild = rightChild;
            }
            else
            {
                rootNode.RightChild = null;
            }

            return rootNode;
        }

        private Vertice kdFindClosestNode(Vertice rootNode, OSM_Node node, int depth = 0)
        {
            if (rootNode == null)
            {
                return null;
            }

            Vertice nextBranch = new Vertice(); 
            Vertice oppositeBranch = new Vertice();
            nextBranch = null;
            oppositeBranch = null;

            if (depth % 2 == 0)
            {
                //Latitude
                if (rootNode.LeftChild != null && rootNode.LeftChild.Latitude < node.Latitude)
                {
                    nextBranch = rootNode.LeftChild;
                    oppositeBranch = rootNode.RightChild;
                }
                else if (rootNode.RightChild != null)
                {
                    nextBranch = rootNode.RightChild;
                    oppositeBranch = rootNode.LeftChild;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                //Longitude
                if (rootNode.LeftChild != null && rootNode.LeftChild.Longitude < node.Longitude)
                {
                    nextBranch = rootNode.LeftChild;
                    oppositeBranch = rootNode.RightChild;
                }
                else if (rootNode.RightChild != null)
                {
                    nextBranch = rootNode.RightChild;
                    oppositeBranch = rootNode.LeftChild;
                }
                else
                {
                    return null;
                }
            }

            Vertice best = kdCloserDistance(node, kdFindClosestNode(nextBranch, node, depth + 1), rootNode);

            if (depth % 2 == 0)
            {
                //Latitude
                if (GetDistance(node.Latitude, node.Longitude, best.Latitude, best.Longitude) > Math.Abs(node.Latitude - rootNode.Latitude))
                {
                    best = kdCloserDistance(node, kdFindClosestNode(oppositeBranch, node, depth + 1), best);
                }
            }
            else
            {
                //Longitude
                if (GetDistance(node.Latitude, node.Longitude, best.Latitude, best.Longitude) > Math.Abs(node.Longitude - rootNode.Longitude))
                {
                    best = kdCloserDistance(node, kdFindClosestNode(oppositeBranch, node, depth + 1), best);
                }
            }

            return best;
        }

        private Vertice kdCloserDistance(OSM_Node pivot, Vertice p1, Vertice p2)
        {
            if(p1 == null)
            {
                return p2;
            }
            if(p2 == null)
            {
                return p1;
            }

            double dist1 = GetDistance(pivot.Latitude, pivot.Longitude, p1.Latitude, p1.Longitude);
            double dist2 = GetDistance(pivot.Latitude, pivot.Longitude, p2.Latitude, p2.Longitude);
            
            if(dist1 < dist2)
            {
                return p1;
            }
            else
            {
                return p2;
            }
        }*/

        private Dictionary<string, Vertice> BuildVerticeDict(Dictionary<string, OSM_ElevationPoint> elevationDict)
        {
            Dictionary<string, Vertice> verticeDict = new Dictionary<string, Vertice>();
            rank = 0;
            foreach (OSM_ElevationPoint ep in elevationDict.Values)
            {
                rank++;
                Vertice vertice = new Vertice();
                vertice.ID = ep.NodeID;
                vertice.Latitude = ep.Latitude;
                vertice.Longitude = ep.Longitude;
                vertice.Elevation = ep.Elevation;
                vertice.Rank = rank;
                vertice.IsPeak = ep.IsPeak;
                verticeDict[vertice.ID] = vertice;
            }
            return verticeDict;
        }

        private void BuildFaces(Dictionary<string, Vertice> verticeDict, KdTree<double, string> tree, int closestX)
        {
            double maxLat = -8888;
            double maxLon = -8888;
            double minLat = 8888;
            double minLon = 8888;
            foreach (var vertice in verticeDict)
            {
                if (vertice.Value.Latitude > maxLat)
                {
                    maxLat = vertice.Value.Latitude;
                }
                if (vertice.Value.Longitude > maxLon)
                {
                    maxLon = vertice.Value.Longitude;
                }
                if (vertice.Value.Latitude < minLat)
                {
                    minLat = vertice.Value.Latitude;
                }
                if (vertice.Value.Longitude < minLon)
                {
                    minLon = vertice.Value.Longitude;
                }

                var nearestNeighbors = tree.GetNearestNeighbours(new[] { vertice.Value.Latitude, vertice.Value.Longitude }, closestX + 4);
                int i = 0;
                foreach(var NN in nearestNeighbors)
                {
                    if(NN.Value != vertice.Key && i < closestX)
                    {
                        i++;
                        vertice.Value.ClosestVertices[i] = new Tuple<string, double>(NN.Value, GetDistance(vertice.Value.Latitude, vertice.Value.Longitude, NN.Point[0], NN.Point[1]));
                    }
                }
            }

            Dictionary<string, Edge> edgeDict = new Dictionary<string, Edge>();
            Dictionary<string, Face> faceDict = new Dictionary<string, Face>();

            //edgeDict = DrawEdges(verticeDict, tree, closestX);
            //faceDict = DrawFaces(verticeDict, edgeDict);

            
            double distMultiplier = (GetDistance(minLat, minLon, maxLat, maxLon) 
                * Math.Sqrt(Math.Pow(maxLat - minLat, 2) + Math.Pow(maxLon - minLat, 2)));

            SetXYZ(verticeDict, minLat, minLon, distMultiplier);

            PrintVertices(verticeDict);
            WriteOBJFile(verticeDict, faceDict);
        }

        private Dictionary<string, Edge> DrawEdges(Dictionary<string, Vertice> verticeDict, KdTree<double, string> tree, int closestX)
        {
            int sharedEdge = 0;
            int interceptCount = 0;
            Dictionary<string, Edge> edgeDict = new Dictionary<string, Edge>();
            bool firstLoop = true;
            bool stillActive = true;

            while (stillActive)
            {
                stillActive = false;
                foreach (Vertice v in verticeDict.Values)
                {
                    v.CurrentCheck++;
                    bool edgeDrawn = false;
                    while (!edgeDrawn && v.CurrentCheck < closestX)
                    {
                        Vertice otherV = verticeDict[v.ClosestVertices[v.CurrentCheck].Item1];
                        string edgeKey = GetEdgeKey(v, otherV);

                        if (firstLoop)
                        {
                            firstLoop = false;
                            Edge firstEdge = new Edge();
                            firstEdge = CreateEdge(v, otherV, edgeKey);
                            edgeDict[edgeKey] = firstEdge;
                            edgeDrawn = true;
                        }
                        if (!edgeDict.ContainsKey(edgeKey))
                        {
                            bool intersects = false;
                            foreach (Edge e in edgeDict.Values)
                            {
                                if (!intersects && LineIntersection.DoLinesIntersect(v, otherV, e))
                                {
                                    intersects = true;
                                }
                            }
                            if (!intersects)
                            {
                                stillActive = true;
                                v.EdgeConnectionDict[otherV.ID] = otherV;
                                otherV.EdgeConnectionDict[v.ID] = v;
                                edgeDict[edgeKey] = CreateEdge(v, otherV, edgeKey);
                                edgeDrawn = true;
                            }
                            else
                            {
                                interceptCount++;
                            }
                        }
                        else
                        {
                            sharedEdge++;
                        }
                        v.CurrentCheck++;
                    }
                }
            }

            return edgeDict;
        }

        private Dictionary<string, Face> DrawFaces(Dictionary<string, Vertice> verticeDict, Dictionary<string, Edge> edgeDict)
        {
            Dictionary<string, Face> faceDict = new Dictionary<string, Face>();

            foreach(Vertice v in verticeDict.Values)
            {
                Dictionary<string, Face> newFaces = new Dictionary<string, Face>();
                newFaces = CheckNeighbors(v, faceDict, edgeDict);

                foreach(var f in newFaces)
                {
                    faceDict[f.Key] = f.Value;
                }
            }

            return faceDict;
        }

        private Dictionary<string, Face> CheckNeighbors(Vertice v, Dictionary<string, Face> faceDict, Dictionary<string, Edge> edgeDict)
        {
            Dictionary<string, Face> newFaces = new Dictionary<string, Face>();
            foreach(Vertice neighbor in v.EdgeConnectionDict.Values)
            {
                //Need to make sure every edge actually exists
                if(edgeDict.ContainsKey(GetEdgeKey(v, neighbor)))
                {
                    foreach (Vertice neighbor2 in neighbor.EdgeConnectionDict.Values)
                    {
                        if (edgeDict.ContainsKey(GetEdgeKey(v, neighbor2)) && edgeDict.ContainsKey(GetEdgeKey(neighbor, neighbor2)) && v.EdgeConnectionDict.ContainsKey(neighbor2.ID))
                        {

                            string faceKey = "";
                            if (v.Rank <= neighbor.Rank)
                            {
                                if (neighbor.Rank <= neighbor2.Rank)
                                {
                                    faceKey = v.Rank + "_" + neighbor.Rank + "_" + neighbor2.Rank;
                                }
                                else
                                {
                                    faceKey = v.Rank + "_" + neighbor2.Rank + "_" + neighbor.Rank;
                                }
                            }
                            else if (neighbor.Rank <= neighbor2.Rank)
                            {
                                if (neighbor2.Rank <= v.Rank)
                                {
                                    faceKey = neighbor.Rank + "_" + neighbor2.Rank + "_" + v.Rank;
                                }
                                else
                                {
                                    faceKey = neighbor.Rank + "_" + v.Rank + "_" + neighbor2.Rank;
                                }
                            }
                            else
                            {
                                if (v.Rank <= neighbor.Rank)
                                {
                                    faceKey = neighbor2.Rank + "_" + v.Rank + "_" + neighbor.Rank;
                                }
                                else
                                {
                                    faceKey = neighbor2.Rank + "_" + neighbor.Rank + "_" + v.Rank;
                                }
                            }
                            if (!faceDict.ContainsKey(faceKey))
                            {
                                newFaces[faceKey] = CreateFace(v, neighbor, neighbor2);
                            }
                        }
                    }

                }
            }
            return newFaces;
        }

        private string GetEdgeKey(Vertice v1, Vertice v2)
        {
            //edge key is ID1_ID2 where ID1 is the vertice ID that is the most to the left. In cases of ties, V1 is listed first.
            string edgeKey = "";

            if(v1.Rank <= v2.Rank)
            {
                edgeKey = v1.Rank + "_" + v2.Rank;
            }
            else
            {
                edgeKey = v2.Rank + "_" + v1.Rank;
            }

            return edgeKey;
        }

        private Face CreateFace(Vertice v1, Vertice v2, Vertice v3)
        {

            Face face = new Face();
            /*double triangleAngle = GetAngle(v1, v2, v3) + GetAngle(v2, v3, v1) + GetAngle(v3, v1, v2);

            if (triangleAngle < 360)
            {
                face.Vertices.Add(v1);
                face.Vertices.Add(v2);
                face.Vertices.Add(v3);
            }
            else
            {
                face.Vertices.Add(v3);
                face.Vertices.Add(v2);
                face.Vertices.Add(v1);
            }*/

            
            double angle = GetAngle(v1, v2, v3);

            if (angle > 180)
            {
                face.Vertices.Add(v1);
                face.Vertices.Add(v2);
                face.Vertices.Add(v3);
            }
            else
            {
                face.Vertices.Add(v3);
                face.Vertices.Add(v2);
                face.Vertices.Add(v1);
            }

            return face;
        }

        private Edge CreateEdge(Vertice v1, Vertice v2, string edgeKey)
        {
            Edge e = new Edge();
            e.Key = edgeKey;
            e.V1 = v1;
            e.V2 = v2;
            e.M = LineIntersection.SolveM(e.V1, e.V2);
            e.B = LineIntersection.SolveB(e.V1, e.M);
            return e;
        }

        //Combining various dictionaries
        private void MergeDicts()
        {
            foreach(OSM_Waterway w in waterwayDict.Values)
            {
                foreach(OSM_Node node in w.NodesToKeep)
                {
                    OSM_ElevationPoint ep = new OSM_ElevationPoint();
                    ep.Latitude = node.Latitude;
                    ep.Longitude = node.Longitude;
                    ep.Elevation = node.Elevation;
                    ep.NodeID = node.NodeID;
                    if (!elevationDict.ContainsKey(ep.NodeID))
                    {
                        rank++;
                        ep.Rank = rank;
                        elevationDict[ep.NodeID] = ep;
                    }
                }
            }
        }

        //Waterways
        private void CollectWaterwayInfo(Dictionary<string, OSM_Waterway> waterwayDict, Dictionary<string, Vertice> verticeDict, KdTree<double, string> tree)
        {
            List<OSM_Waterway> waterwayList = new List<OSM_Waterway>();
            foreach (OSM_Waterway w in waterwayDict.Values)
            {
                OSM_Node thisNode = new OSM_Node();
                OSM_Node previousNode = new OSM_Node();
                OSM_Node twoNodesAgo = new OSM_Node();
                bool firstNode = true;
                int counter = 0;
                foreach (string node in w.NodeList)
                {
                    Vertice closestVert = new Vertice();
                    counter++;
                    thisNode = nodeDict[node];
                    if (!firstNode)
                    {
                        w.Distance += GetDistance(previousNode.Latitude, previousNode.Longitude, thisNode.Latitude, thisNode.Longitude);
                        if(Math.Round(w.Distance / .5) > w.NodesToKeep.Count() - 1)
                        {
                            w.NodesToKeep.Add(thisNode);
                        }
                        if (counter > 2)
                        {
                            w.TotalAngle += Math.Abs(GetAngleDoubles(twoNodesAgo.Latitude, twoNodesAgo.Longitude, previousNode.Latitude, previousNode.Longitude, thisNode.Latitude, thisNode.Longitude));
                        }
                        twoNodesAgo = previousNode;
                        previousNode = thisNode;
                    }
                    else
                    {
                        //closestVert = kdFindClosestNode(tree, thisNode);

                        w.NodesToKeep.Add(thisNode);
                        firstNode = false;
                        previousNode = thisNode;
                    }
                }
                w.NodesToKeep.Add(thisNode);
                waterwayList.Add(w);
            }
            //PrintWaterways(waterwayList);

            foreach (OSM_Waterway w in waterwayDict.Values)
            {
                bool firstLoop = true;
                double previousElevation = 0;
                foreach(OSM_Node node in w.NodesToKeep)
                {
                    node.Elevation = CalculateWaterwayNodeHeight(node, verticeDict, tree, 8);
                    if (firstLoop)
                    {
                        firstLoop = false;
                        previousElevation = node.Elevation;
                        continue;
                    }
                    /* This check will only work if we know that we are going downhill(find endpoints and work way down)
                    if(node.Elevation < previousElevation)
                    {
                        node.Elevation = previousElevation;
                    }
                    */
                }
            }
        }


        private double CalculateWaterwayNodeHeight(OSM_Node node, Dictionary<string, Vertice> verticeDict, KdTree<double, string> tree, int closestX)
        {
            //Calculate the height for a waterway point based on the closest x elevation points
            double elevation = 0;
            double divisionFactor = 0;
            var nearsestNeighbours = tree.GetNearestNeighbours(new[] { node.Latitude, node.Longitude }, closestX);
            Vertice furthestPoint = verticeDict[nearsestNeighbours[closestX - 1].Value];
            double furthestDist = GetDistance(node.Latitude, node.Longitude, furthestPoint.Latitude, furthestPoint.Longitude);

            foreach (var NN in nearsestNeighbours)
            {
                Vertice NNPoint = verticeDict[NN.Value];
                double thisDist = GetDistance(node.Latitude, node.Longitude, NNPoint.Latitude, NNPoint.Longitude);
                double relativeDist = furthestDist / thisDist;
                divisionFactor += relativeDist;
                elevation = elevation + (verticeDict[NN.Value].Elevation * relativeDist);
            }

            elevation = elevation / divisionFactor;
            return elevation; //will need an adjustment
        }

        private double GetAngle(Vertice v1, Vertice v2, Vertice v3)
        {
            //V1 should be V1 from edge, V2 should be V2, and V3 should be the point we are trying out
            double angle = 0;
            double[] p1 = { v1.Longitude, v1.Latitude };
            double[] p2 = { v2.Longitude, v2.Latitude };
            double[] p3 = { v3.Longitude, v3.Latitude };

            //arccos((P12^2 + P13^2 - P23^2) / (2 * P12 * P13))

            angle = Math.Acos((Math.Pow(AngleDistance(p1, p2), 2) + Math.Pow(AngleDistance(p1, p3), 2) - Math.Pow(AngleDistance(p2, p3), 2)) / (2 * AngleDistance(p1, p2) * AngleDistance(p1, p3))) * (180.0 / Math.PI);

            if(p1[0] > p2[0])
            {
                angle = 360 - angle;
            }

            return angle;
        }

        private double AngleDistance(double[] p1, double[] p2)
        {
            return Math.Sqrt((Math.Pow(p1[0] - p2[0], 2) + Math.Pow(p1[1] - p2[1], 2)));
        }

        private double GetAngleDoubles(double lat1, double lon1, double lat2, double lon2, double lat3, double lon3)
        {
            double[] p1 = { lon1, lat1 };
            double[] p2 = { lon2, lat2 };
            double[] p3 = { lon3, lat3 };

            double angle = Math.Acos((Math.Pow(AngleDistance(p1, p2), 2) + Math.Pow(AngleDistance(p1, p3), 2) - Math.Pow(AngleDistance(p2, p3), 2)) / (2 * AngleDistance(p1, p2) * AngleDistance(p1, p3))) * (180.0 / Math.PI);

            if (p1[0] > p2[0])
            {
                angle = 360 - angle;
            }

            return angle;
        }

        private void SetXYZ(Dictionary<string, Vertice> verticeDict, double minLat, double minLon, double distMultiplier = 1)
        {
            //Doing rough distances for now...Earth being round and what not is always getting in the way!
            //Measurements are in km

            foreach(Vertice v in verticeDict.Values)
            {
                v.X = (v.Longitude - minLon) * distMultiplier;
                v.Y = (v.Latitude - minLat) * distMultiplier;
                v.Z = (v.Elevation / 3280.84) * Convert.ToDouble(heightScalerTextBox.Text);
            }
        }

        private double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;
            return dist;
        }

        private void WriteOBJFile(Dictionary<string, Vertice> verticeDict, Dictionary<string, Face> faceDict)
        {
            StreamWriter objWriter = new StreamWriter(@"E:\Desktop\Personal Projects\Output Files\topology.obj");
            objWriter.WriteLine("# 3ds Max Wavefront OBJ file generated from OSM data");
            objWriter.WriteLine();

            foreach(Vertice v in verticeDict.Values)
            {
                objWriter.WriteLine("v  " + v.X + " " + v.Y + " " + v.Z);
            }
            /*int counter = 0;
            while(counter <= verticeDict.Count())
            {
                counter++;
                Vertice v = verticeDict[counter];
                objWriter.WriteLine("v  " + v.X + " " + v.Y + " " + v.Z);
            }*/
            objWriter.WriteLine("# " + verticeDict.Count() +" total vertices");
            objWriter.WriteLine();
            objWriter.WriteLine("g Top_Map");
            foreach(Face f in faceDict.Values)
            {
                string lineToWrite = "f ";
                foreach(Vertice v in f.Vertices)
                {
                    lineToWrite += " " + v.Rank;
                }
                objWriter.WriteLine(lineToWrite);
            }
            objWriter.WriteLine("# " + faceDict.Count() + " total faces");
            objWriter.WriteLine();
            objWriter.Close();
        }

        private void PrintVertices(Dictionary<string, Vertice> verticeDict)
        {
            StreamWriter verticeWriter = new StreamWriter(string.Concat(saveLoc, @"\elevationPoints.csv"));
            verticeWriter.WriteLine("Latitude,Longitude,Elevation,Rank,isEdge");

            foreach (Vertice v in verticeDict.Values)
            {
                verticeWriter.WriteLine(v.Latitude + "," + v.Longitude + "," + v.Elevation + "," + v.Rank);
            }
            verticeWriter.Close();
        }

        private void PrintWaterways(List<OSM_Waterway> waterwayDict)
        {
            StreamWriter waterWriter = new StreamWriter(string.Concat(saveLoc, @"\waterways.csv"));
            waterWriter.WriteLine("Latitude,Longitude,ID,Type,Distance,Total Angle,KeepNode");

            foreach (OSM_Waterway w in waterwayDict)
            {
                foreach(string node in w.NodeList)
                {
                    waterWriter.WriteLine(nodeDict[node].Latitude + "," + nodeDict[node].Longitude + "," + w.ID + "," + w.Type + "," + w.Distance + "," + w.TotalAngle + "," + w.NodesToKeep.Contains(nodeDict[node]));
                }
            }
            waterWriter.Close();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            saveLoc = fbd.SelectedPath;
            saveLocTextBox.Text = saveLoc;
        }
    }
}
