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

    //input graph, constant values
    //demands and distances
    private Graph State { get; set; }
    
    private int MAX_ITER { get; }

    private int Amount { get; }

    private double EvaporationFactor { get; }
    private Type AlgorithmType { get; }
    private int CountOfRankAnts { get; }

    public Solution Run()
    {

      //MAX_ITER iterations
      for (int i = 0; i < MAX_ITER; i++)
      {
        for (int ant = 0; ant < Amount; ant++)
        {
          FindSolution(ant);
        }
        CorrectPheromones(AlgorithmType);
      }


      return new Solution();
    }

    public Solution FindSolution(int ant)
    {
      Solution solution = new Solution();



      return solution;
    }

    public void CorrectPheromones(Type type)
    {

    }
  }
}
