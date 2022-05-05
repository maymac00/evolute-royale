using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class IndividualFactory
{
   public static Individual buildIndividual(int inp, int ot, List<ConnectionGene> genome)
    {
        Individual ind = new Individual(inp, ot);
        genome.Sort();
       
        foreach (ConnectionGene gen in genome)
        {
            List<int> aux = new List<int>(ind.output);
            aux.AddRange(ind.input);
            aux.AddRange(ind.hidden);
            if (gen.input >= ind.matrix.Count || gen.output >= ind.matrix.Count) {
                int n = Utils.Max(gen.input, gen.output) + 1 - ind.matrix.Count;
                for (int i = 0; i < n; i++)
                {
                    List<ConnectionGene> new_row = new List<ConnectionGene>();
                    foreach (List<ConnectionGene> l in ind.matrix)
                    {
                        l.Add(null);
                        new_row.Add(null);
                    }
                    new_row.Add(null);
                    ind.matrix.Add(new_row);
                }
                if (!aux.Contains(gen.input)) {
                    ind.hidden.Add(gen.input);
                    ind.n_neurons++;
                }
                if (!aux.Contains(gen.output))
                {
                    ind.hidden.Add(gen.output);
                    ind.n_neurons++;
                }

            }

            try
            {
                int n = ind.add_connection(gen.input, gen.output);
                ind.genome[n].w = gen.w;
                ind.genome[n].enable = gen.enable;
                ind.genome[n].innovation = gen.innovation;
            }
            catch
            {
                Debug.Log(gen.input.ToString() +" "+ gen.output.ToString());
            }
            
        }
        return ind;
    }
}
