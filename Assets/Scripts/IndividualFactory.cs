using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class IndividualFactory
{
   public static Individual buildIndividual(int inp, int ot, List<ConnectionGene> genome)
    {
        Individual ind = new Individual(inp, ot);
        foreach(ConnectionGene gen in genome)
        {
            List<int> aux = new List<int>(ind.output);
            aux.AddRange(ind.hidden);
            if (!aux.Contains(gen.output))
            {
                foreach (List<ConnectionGene> l in ind.matrix)
                {
                    l.Add(null);
                }
                ind.matrix.Add(new List<ConnectionGene>(ind.n_neurons));
                ind.hidden.Add(gen.output);
                ind.n_neurons++;
            }
            int n = ind.add_connection(gen.input, gen.output);
            ind.genome[n].w = gen.w;
            ind.genome[n].enable = gen.enable;
            ind.genome[n].innovation = gen.innovation;
        }
        return ind;
    }
}
