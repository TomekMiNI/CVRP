﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
      //to save solutions in file
      StringBuilder output = new StringBuilder();
      string problemDirectory = @"..\..\Instances\SetA\Problems\";
      string solutionDirectory = @"..\..\Instances\SetA\Solutions\";
      string file = "A-n80-k10";
      string fileWithExtension = file + ".vrp";
      string filepath = problemDirectory + fileWithExtension;
      output.AppendLine(file);

      bool baseVariant = true;
      int maxIter = 2000;
      output.AppendLine($"Max iterations = {maxIter}");
      //rank variant
      int countOfElite = 10;
      output.AppendLine($"Rank variant E={countOfElite}");

      CVRP algorithm = new CVRP(maxIter, 0.75, 5, 5, filepath, Type.Rank, countOfElite);
      var solution = algorithm.Run(output);
      Console.Write(solution);

      if (baseVariant)
      {
        output.AppendLine("Base variant");
        algorithm = new CVRP(maxIter, 0.75, 5, 5, filepath);
        solution = algorithm.Run(output);
        Console.WriteLine(solution);
      }
      string path = @"..\..\Instances\SetA\Results\" + file + "[" + countOfElite + "][iter" + maxIter + "].txt";

      //overwrite
      output.AppendLine(File.ReadLines(solutionDirectory + file + ".sol").Last());
      File.WriteAllText(path, output.ToString());
      
    }

    static void ReadFromFile(string path)
    {

    }
  }
}
