using System;
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
	  int perInstanceExec = 10;
      int maxIter = 10000;
	  double evaporationFactor = 0.5;
	  Type variant = Type.Evaporation;	    
	  int countOfElite = 10;

      string problemDirectory = @"..\..\Instances\SetA\FewProblems\";
      string solutionDirectory = @"..\..\Instances\SetA\Solutions\";
	  var tasks = new List<Task>();
	  var files = Directory.GetFiles(problemDirectory, "*.vrp", SearchOption.TopDirectoryOnly);

	  for (int i = 0; i < perInstanceExec; i++)
	  {
		foreach (var filepath in files)
		{
		  //to save solutions in file
		  var file = Path.GetFileName(filepath).Split('.')[0];

		  StringBuilder output = PrepareOutputString(maxIter, countOfElite, variant, file);
		  StringBuilder outputLocals = PrepareOutputString(maxIter, countOfElite, variant, "loc_" + file);

		  CVRP algorithm = new CVRP(maxIter, evaporationFactor, 5, 5, filepath, i, variant, countOfElite);
		  var solution = algorithm.Run(output, outputLocals);
		  //Console.Write(solution);

		  string path = PreparePath(maxIter, variant, file, countOfElite, i);
		  string pathLocal = PreparePath(maxIter, variant, "loc_" + file, countOfElite, i);
		  tasks.Add(Task.Run(SaveResults(output, solutionDirectory, file, path, true)));  //overwrites
		  tasks.Add(Task.Run(SaveResults(outputLocals, solutionDirectory, file, pathLocal, false)));  //overwrites
		} 
	  }
	  Task.WaitAll(tasks.ToArray());
    }

	private static StringBuilder PrepareOutputString(int maxIter, int modificationFactor, Type variant, string file)
	{
	  StringBuilder output = new StringBuilder();
	  output.AppendLine(file);
	  output.AppendLine($"Max iterations = {maxIter}");

	  //rank variant - countOfElite
	  //evaporation variant - extra evaporation factor
	  if (variant == Type.Rank)
		output.AppendLine($"Count of Elite = {modificationFactor}");
	 // else
		//output.AppendLine($"Evaporation theta = {modificationFactor}");
	  return output;
	}

	private static string PreparePath(int maxIter, Type variant, string file, int modificationFactor, int iterationNumber)
	{
	  string variantDirectory;
	  switch (variant)
	  {
		case Type.Basic:
		  variantDirectory = @"Basic\";
      modificationFactor = 0;
      break;
		case Type.Rank:
		  variantDirectory = @"Rank\";
		  break;
		case Type.Evaporation:
		  variantDirectory = @"Evaporation\";
      modificationFactor = 0;
		  break;
		default:
		  throw new ArgumentException();
	  }
	  string path = @"..\..\Instances\SetA\Results\" + variantDirectory + file + "[" + modificationFactor + "][iter" + maxIter + "]["+ iterationNumber +"].txt";
	  return path;
	}

	public static Action SaveResults(StringBuilder output, string solutionDirectory, string file, string path, bool best)
	{
	  return new Action(() => {
		if(best)
		  output.AppendLine(File.ReadLines(solutionDirectory + file + ".sol").Last());
		File.WriteAllText(path, output.ToString());
	  });
	}
  }
}
