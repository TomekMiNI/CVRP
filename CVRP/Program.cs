using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVRP
{
  class Program
  {
    /// <summary>
    ///
    //n + 1 wierzcholkow
    //n - liczba klientow = liczba mrowek
    //Q - capacity
    //MAX_ITER
    //q[] - demands
    //d[,] - distances
    //p[,] - pheromones

    //a - wspolczynnik wagi dystansu
    //b - wspolczynnik wagi sladu feromonu

    //algorithm:
    //initially:
    //every edge has pheromone equals 0
    //we need to remember last chosen path of every ant to correct pheromone
    //also current best solution

    ///
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
      //if (args.Length != 2)
      //  throw new Exception("we need path to data file!");

      //ReadFromFile(args[1]);

      //initialization
      //get data from file

      CVRP algorithm = new CVRP(1000, 0.75, 5, 5, @"C:\MiniProjects\CVRP\CVRP\Instances\SetA\Problems\A-n32-k5.vrp");
      var solution = algorithm.Run();

      Console.WriteLine(solution);
    }

    static void ReadFromFile(string path)
    {

    }
  }
}
