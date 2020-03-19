using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVRP
{
  public enum Type
  {
    Basic,
    Rank,
    EvaporationDecrease
  }
  class CVRP
  {
	private const double evalutaionMinimum = 0.00001;
	private double evalutaionMax { get { return Double.MaxValue / (CountOfVertices * 100); } }
	//we have
	//basic correction
	//rank correction
	//evaporation decrease correction

	/// <summary>
	/// Current state 
	/// </summary>
	private Graph GraphState { get; set; }
    /// <summary>
    /// Max number of iterations
    /// </summary>
    private int MaxIter { get; }
    /// <summary>
    /// Count of ants
    /// </summary>
    private int CountOfAnts { get; }
    /// <summary>
    /// Evaporation factor
    /// </summary>
    private double EvaporationFactor { get; }
    /// <summary>
    /// Each ant has the same capacity
    /// </summary>
    private int Capacity { get; }

    /// <summary>
    /// Count of vertices
    /// </summary>
    private int CountOfVertices { get; }
    /// <summary>
    /// Weight of pheromone
    /// </summary>
    private double Alpha { get; }
    /// <summary>
    /// Weight of distance
    /// </summary>
    private double Beta { get; }

	private Random Generator { get; }

    //modifications
    private Type AlgorithmType { get; }
    private int CountOfRankAnts { get; }

    public CVRP(int maxIter, double evaporationFactor, double alpha, double beta, string filePath, double[,] pheromones = null)
    {
      MaxIter = maxIter;
      EvaporationFactor = evaporationFactor;
      Alpha = alpha;
      Beta = beta;
	  (GraphState, Capacity) = Utilities.ReadInputCvrp(filePath);
	  CountOfVertices = GraphState.vertexCount;
	  CountOfAnts = CountOfVertices - 1;
	  Generator = new Random(123);
	}

	public Solution Run()
    {
	  Solution bestSolution = null;
      //MAX_ITER iterations
      for (int i = 0; i < MaxIter; i++)
      {
        var solutions = new Solution[CountOfAnts];
        for (int ant = 0; ant < CountOfAnts; ant++)
        {
           solutions[ant] = FindSolution(ant);
        }
        var minLocalSolution = solutions.Min();

        if (bestSolution == null || minLocalSolution.Value < bestSolution.Value)
		  bestSolution = minLocalSolution;

		UpdatePheromones(solutions);
      }
      return bestSolution;
    }

    public Solution FindSolution(int ant)
    {
      Solution solution = new Solution();

      bool[] alreadyVisitedArr = new bool[CountOfVertices];
	  int start = ant + 1;
	  //ant starts in [ant + 1]th vertice in GRAPH [0 is a depot]
	  alreadyVisitedArr[start] = true;

      int alreadyVisited = 1;
      int currentCapacity = Capacity;
      Route route = new Route();

      while(alreadyVisited < CountOfVertices - 1)
	  {
		//1. phase: Choosing next vertice to visit
		//double bestEvaluation = 0;
		//int nextVertex = 0;
		//Until we have capacity we do not choose depot (vertex 0)
		//for (int i = 1; i < CountOfVertices; i++)
		//  if (!alreadyVisitedArr[i] && i != start && GraphState[i] <= currentCapacity)
		//  {
		//	double currentEvaluation = Math.Pow(GraphState[start, i].Pheromone, Alpha) * 1 / Math.Pow(GraphState[start, i].Distance, Beta);

		//	if (currentEvaluation > bestEvaluation)
		//	{
		//	  bestEvaluation = currentEvaluation;
		//	  nextVertex = i;
		//	}
		//  }

		double[] rouletteWheel = CalculateProbabilities(alreadyVisitedArr, start, currentCapacity);
		int nextVertex = CountOfVertices - 1;
		if (rouletteWheel != null)
		{
		  var p = Generator.NextDouble();
		  for (int i = 0; i < CountOfVertices; i++)
		  {
			if (p < rouletteWheel[i])
			{
			  nextVertex = i;
			  break;
			}
		  }
		}
		else
		{
		  nextVertex = 0;
		}
		

		//next vertice chosen
		route.Add(new Edge(start, nextVertex, GraphState[start, nextVertex].Distance, 0));

		if (nextVertex != 0)
		{
		  currentCapacity -= GraphState[nextVertex];
		  alreadyVisited++;
		  alreadyVisitedArr[nextVertex] = true;
		}
		//back to depot
		else
		{
		  currentCapacity = Capacity;
		  solution.Add(route);
		  route = new Route();
		}
		start = nextVertex;
	  }

	  //last edge to base
	  route.Add(new Edge(start, 0, GraphState[start, 0].Distance, 0));
      solution.Add(route);

      return solution;
    }

	private double[] CalculateProbabilities(bool[] alreadyVisitedArr, int start, int currentCapacity)
	{
	  double[] rouletteWheel = new double[CountOfVertices];
	  for (int i = 1; i < CountOfVertices; i++)
	  {
		rouletteWheel[i] = rouletteWheel[i - 1];
		if (!alreadyVisitedArr[i] && i != start && GraphState[i] <= currentCapacity)
		{
		  var evalutaion = Math.Pow(GraphState[start, i].Pheromone, Alpha) * 1 / Math.Pow(GraphState[start, i].Distance, Beta);
		  if (evalutaion < evalutaionMinimum)
			evalutaion = evalutaionMinimum;
		  if (evalutaion > evalutaionMax)
			evalutaion = evalutaionMax;
		  rouletteWheel[i] += evalutaion;
		}
	  }
	  if(rouletteWheel.Last() > evalutaionMinimum / 10)
	  {
		for (int i = 0; i < CountOfVertices; i++)
		{
		  rouletteWheel[i] = rouletteWheel[i] / rouletteWheel.Last();
		}
	  }
	  else
	  {
		return null;
	  }
	  

	  return rouletteWheel;
	}

	public void UpdatePheromones(Solution[] solutions)
    {
      double constW = CountOfAnts; //??
      Graph pheromonesIncrease = new Graph(CountOfVertices);

      //leaving pheromone
      foreach (var sol in solutions)
      {
        foreach (var route in sol.Routes)
        {
          foreach (var edge in route.Edges)
          {
            double val = constW / sol.Value;
			pheromonesIncrease[edge.V1, edge.V2].Pheromone += val;
          }
        }
      }

      //evaporation and pheromone increase
      for (int i = 0; i < CountOfVertices - 1; i++)
        for (int j = i + 1; j < CountOfVertices; j++)
          GraphState[i, j].Pheromone = GraphState[i, j].Pheromone * EvaporationFactor + pheromonesIncrease[i, j].Pheromone;
    }
  }
}
