using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVRP
{
  public static class Utilities
  {
	public static (Graph, int capacity) ReadInputCvrp(string fileName)
	{
	  using (StreamReader input = new StreamReader(fileName))
	  {
		int vertexNum = 0;
		int truckCapacity = 0;
		string[] splitted;
		while (true)
		{
		  splitted = input.ReadLine().Split(':');
		  if (splitted[0].Contains("DIMENSION"))
		  {
			vertexNum = int.Parse(splitted[1]);
		  }
		  else if (splitted[0].Contains("CAPACITY"))
		  {
			truckCapacity = int.Parse(splitted[1]);
		  }
		  else if (splitted[0].Contains("EDGE_WEIGHT_TYPE"))
		  {
			if (!splitted[1].Trim().Equals("EUC_2D"))
			  throw new Exception("Edge Weight Type " + splitted[1] + " is not supported (only EUC_2D)");
		  }
		  else if (splitted[0].Contains("NODE_COORD_SECTION"))
		  {
			break;
		  }
		}
		(int x, int y)[] vertexCoords = new (int x, int y)[vertexNum];
		for (int n = 1; n <= vertexNum; n++)
		{
		  splitted = input.ReadLine().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
		  if (int.Parse(splitted[0]) != n)
			throw new Exception("Unexpected index");
		  vertexCoords[n - 1] = (int.Parse(splitted[1]), int.Parse(splitted[2]));
		}

		var distMatrix = ComputeDistanceMatrix(vertexCoords);

		splitted = input.ReadLine().Split(':');
		if (!splitted[0].Contains("DEMAND_SECTION"))
		  throw new Exception("Expected keyword DEMAND_SECTION");

		var demands = new int[vertexNum];
		for (int n = 1; n <= vertexNum; n++)
		{
		  splitted = input.ReadLine().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
		  if (int.Parse(splitted[0]) != n)
			throw new Exception("Unexpected index");
		  var demand = int.Parse(splitted[1]);
		  demands[n - 1] = demand;
		}

		splitted = input.ReadLine().Split(':');
		if (!splitted[0].Contains("DEPOT_SECTION"))
		  throw new Exception("Expected keyword DEPOT_SECTION");

		int warehouseId = int.Parse(input.ReadLine());
		if (warehouseId != 1)
		  throw new Exception("Warehouse id is supposed to be 1");

		int endOfDepotSection = int.Parse(input.ReadLine());
		if (endOfDepotSection != -1)
		  throw new Exception("Expecting only one warehouse, more than one found");
		double[,] pheromones = initializePheromones(vertexNum);
		return (new Graph(vertexNum, distMatrix, demands, pheromones), truckCapacity);
	  }
	}

	private static double[,] initializePheromones(int vertexNum)
	{
	  var pheromones = new double[vertexNum, vertexNum];
	  for (int i = 0; i < vertexNum; i++)
	  {
		for (int j = i + 1; j < vertexNum; j++)
		{
		  pheromones[i, j] = 100;
		}
	  }

	  return pheromones;
	}

	private static double[,] ComputeDistanceMatrix((int x, int y)[] vertexCoords)
	{
	  var distMatrix = new double[vertexCoords.Length, vertexCoords.Length];
	  for (int i = 0; i < vertexCoords.Length; i++)
	  {
		for (int j = i + 1; j < vertexCoords.Length; j++)
		{
		  distMatrix[i, j] = Math.Sqrt((vertexCoords[i].x - vertexCoords[j].x) * (vertexCoords[i].x - vertexCoords[j].x)
									 + (vertexCoords[i].y - vertexCoords[j].y) * (vertexCoords[i].y - vertexCoords[j].y));
		}
	  }
	  return distMatrix;
	}
  }
}
