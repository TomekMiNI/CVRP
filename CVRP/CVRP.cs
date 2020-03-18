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
    //we have
    //basic correction
    //rank correction
    //evaporation decrease correction

    /// <summary>
    /// Current state 
    /// </summary>
    private Graph State { get; set; }
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

    //modifications
    private Type AlgorithmType { get; }
    private int CountOfRankAnts { get; }

    public CVRP(int countOfAnts, int maxIter, double evaporationFactor, int[,] distances, int capacity, int[] demands, double alpha, double beta, double[,] pheromones = null)
    {
      CountOfAnts = countOfAnts;
      MaxIter = maxIter;
      EvaporationFactor = evaporationFactor;
      CountOfVertices = CountOfAnts + 1;
      Capacity = capacity;

      Alpha = alpha;
      Beta = beta;

      State = new Graph(CountOfVertices, distances, demands, pheromones);
    }

    public Solution Run()
    {
      var bestSolution = new Solution();
      //MAX_ITER iterations
      for (int i = 0; i < MaxIter; i++)
      {
        var solutions = new Solution[CountOfAnts];
        for (int ant = 0; ant < CountOfAnts; ant++)
        {
           solutions[ant] = FindSolution(ant);
        }
        var minLocalSolution = solutions.Min();

        if (minLocalSolution.Value < bestSolution.Value)
          bestSolution = minLocalSolution;

        UpdatePheromones(solutions);
      }
      return bestSolution;
    }

    public Solution FindSolution(int ant)
    {
      Solution solution = new Solution();

      bool[] alreadyVisitedArr = new bool[CountOfVertices];
      //ant starts in [ant + 1]th vertice in GRAPH [0 is a depot]
      alreadyVisitedArr[ant + 1] = true;
      int alreadyVisited = 1;

      int start = ant + 1;
      int currentCapacity = Capacity;

      Route route = new Route();

      while(alreadyVisited < CountOfAnts)
      {
        //1. phase: Choosing next vertice to visit
        double currentMax = 0;
        int currentWinner = 0;
        //Until we have capacity we do not choose depot (vertice 0)
        for(int i = 1; i < CountOfVertices; i++)
          if(!alreadyVisitedArr[i] && State[i] <= currentCapacity)
          {
            double candidate = Math.Pow(State[start, i].Pheromone, Alpha) * Math.Pow(State[start, i].Distance, Beta);
            if(candidate > currentMax)
            {
              currentMax = candidate;
              currentWinner = i;
            }
          }


        //next vertice chosen
        route.Add(new Edge(start, currentWinner, State[start, currentWinner].Distance, 0));

        if(currentWinner != 0)
        {
          currentCapacity -= State[currentWinner];
          alreadyVisited++;
          alreadyVisitedArr[currentWinner] = true;
        }
        //back to depot
        else
        {
          currentCapacity = Capacity;
          solution.Add(route);
          route = new Route();
        }
        start = currentWinner;
      }

      //last edge to base
      route.Add(new Edge(start, 0, State[start, 0].Distance, 0));
      solution.Add(route);

      return solution;
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
          State[i, j].Pheromone = State[i, j].Pheromone * EvaporationFactor + pheromonesIncrease[i, j].Pheromone;
    }
  }
}
