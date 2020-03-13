using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVRP
{
  class CVRP
  {
    //we have
    //basic correction
    //rank correction
    //evaporation decrease correction

    //input graph, constant values
    //demands and distances
    private Graph DataGraph;

    //graph or just an array
    //pheromones and changing demands (from max to 0, 0 means client served) ??
    private Graph VarGraph;

    public Path Run()
    {
      return new Path();
    }
  }
}
