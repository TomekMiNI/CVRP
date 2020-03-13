using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVRP
{
  class Graph
  {
    int[] vertices;
    Edge[,] edges;

    public int this[int i] => vertices[i];
    public Edge this[int i, int j] => edges[Math.Min(i, j), Math.Max(i,j)]; //upper
  }

  class Edge
  {
    int v1, v2;
    int distance;
    double pheromone;

    public Edge(int v1, int v2, int distance)
    {
        this.v1 = v1;
        this.v2 = v2;
        this.distance = distance;
    }

    public override string ToString()
    {
      return base.ToString();
    }
  }

  class Route
  {
    List<Edge> route;

    public override string ToString()
    {
      return base.ToString();
    }
  }

  class Solution
  {
    List<Route> path;
    public override string ToString()
    {
      return base.ToString();
    }
  }
  
}
