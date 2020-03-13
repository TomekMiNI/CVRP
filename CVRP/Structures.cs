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
    int[,] edges;

    public int this[int i] => vertices[i];
    public int this[int i, int j] => edges[i, j];
  }

  class Edge
  {
    int v1, v2;
    int val;

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

  class Path
  {
    List<Route> path;
    public override string ToString()
    {
      return base.ToString();
    }
  }
  
}
