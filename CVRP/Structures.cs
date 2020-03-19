using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVRP
{
  public class Graph
  {
    int[] demands;
    Edge[,] edges;

	public int vertexCount { get; private set;}
    public int this[int i] => demands[i];
    public Edge this[int i, int j]
    {
      get {
        return edges[Math.Min(i, j), Math.Max(i, j)]; //upper
      }
      set {
        edges[Math.Min(i, j), Math.Max(i, j)] = value;
      }
    }

    /// <summary>
    /// Graph with distances and pheromones
    /// </summary>
    /// <param name="size"></param>
    /// <param name="distances"></param>
    /// <param name="pheromones">If null every edge starts with pheromone equal to 0</param>
    public Graph(int size, double[,] distances, int[] demands, double[,] pheromones = null)
    {
	  vertexCount = size;
      this.demands = demands;
      if (pheromones == null)
        pheromones = new double[size, size];
      edges = new Edge[size, size];
      for (int i = 0; i < size - 1; i++)
        for (int j = i + 1; j < size; j++)
          edges[i, j] = new Edge(i, j, distances[i, j], pheromones[i, j]);
    }

	public Graph(int size)
	{
	  vertexCount = size;
	  demands = new int[size];
	  edges = new Edge[size, size];
	  for (int i = 0; i < size - 1; i++)
		for (int j = i + 1; j < size; j++)
		  edges[i, j] = new Edge(i, j, 0, 0);
	}
  }

  public class Edge
  {
    /// <summary>
    /// (v1, v2) = (v2, v1) - because of undirected graph
    /// </summary>
    public int V1 { get; set; }
    public int V2 { get; set; }
    /// <summary>
    /// Euclidean distance between demands with indexes v1 and v2
    /// </summary>
    public double Distance { get; set; }
    /// <summary>
    /// current pheromone value
    /// </summary>
    public double Pheromone { get; set; }

    public Edge(int v1, int v2, double distance, double pheromone)
    {
      V1 = v1;
      V2 = v2;
      Distance = distance;
      Pheromone = pheromone;
    }

    public override string ToString()
    {
      return string.Format("<{0}, {1}>", Math.Min(V1, V2), Math.Max(V1, V2));
    }
  }

  public class Route
  {
    public List<Edge> Edges = new List<Edge>();
    public double Value => Edges.Sum(e => e.Distance);

    public void Add(Edge e)
    {
      Edges.Add(e);
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append('[');
      Edges.ForEach(e => sb.Append(e.ToString()));
      sb.Append(']');
      return sb.ToString();
    }
  }

  class Solution : IComparable<Solution>
  {
    public List<Route> Routes = new List<Route>();
    public double Value => Routes.Sum(r => r.Value);

    public void Add(Route route)
    {
      Routes.Add(route);
    }

    public int CompareTo(Solution other)
    {
	  if (Value == other.Value)
		return 0;
      return Value < other.Value ? -1 : 1;
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      Routes.ForEach(r => sb.AppendLine(r.ToString()));
	  sb.AppendLine("Best solution value: " + Value.ToString());
      return sb.ToString();
    }
  }
  
  
}
