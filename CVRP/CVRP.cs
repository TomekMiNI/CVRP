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
    private double Capacity { get; }

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

    public CVRP(int countOfAnts, int maxIter, double evaporationFactor, int[,] distances, double alpha, double beta, double[,] pheromones = null)
    {
      CountOfAnts = countOfAnts;
      MaxIter = maxIter;
      EvaporationFactor = evaporationFactor;
      CountOfVertices = CountOfAnts + 1;

      Alpha = alpha;
      Beta = beta;

      State = new Graph(CountOfVertices, distances, pheromones);
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

      while(alreadyVisited < CountOfAnts)
      {
        //1. phase: Choosing next vertice to visit
        double currentMax = 0;
        int currentWinner = -1;
        //We avoid depot. We go there when we have no other option
        for(int i = 1; i < CountOfVertices; i++)
          if(!alreadyVisitedArr[i])
          {
            double candidate = Math.Pow(State[start, i].Pheromone, Alpha) * Math.Pow(State[start, i].Distance, Beta);
            if(candidate > currentMax)
            {
              currentMax = candidate;
              currentWinner = i;
            }
          }

        //next vertice chosen
        start = currentWinner;
      }

      return solution;
    }

    public void UpdatePheromones(Solution[] solutions)
    {
      double constW = CountOfAnts; //??
      double[,] pheromonesIncrease = new double[CountOfVertices, CountOfVertices];

      //leaving pheromone
      foreach (var sol in solutions)
      {
        foreach (var route in sol.Routes)
        {
          foreach (var edge in route.Edges)
          {
            double val = constW / sol.Value;
            pheromonesIncrease[edge.V1, edge.V1] += val;
            pheromonesIncrease[edge.V2, edge.V2] += val;
          }
        }
      }

      //evaporation and pheromone increase
      for (int i = 0; i < CountOfVertices - 1; i++)
        for (int j = i + 1; j < CountOfVertices; j++)
          State[i, j].Pheromone = State[i, j].Pheromone * EvaporationFactor + pheromonesIncrease[i, j];
    }
  }
}
