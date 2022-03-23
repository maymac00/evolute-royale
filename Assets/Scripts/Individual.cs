using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Individual
{
    public List<int> input;
    public List<int> output;
    public List<int> hidden;

    public int n_neurons;

    public List<List<ConnectionGene>> matrix;

    public Dictionary<int, ConnectionGene> genome;

    public float fitness = -1.0f;
    public float adj_fitness = -1.0f;

    public Individual(int inp = -1, int _output = -1)
    {

        input = Range.getRange(0, inp);
        output = Range.getRange(inp, inp + _output);
        hidden = new List<int>();

        n_neurons = inp + _output;

        for (int i = 0; i < n_neurons; i++)
        {
            for (int j = 0; j < n_neurons; j++)
            {
                matrix.Add(null);
            }
        }

        genome = new Dictionary<int, ConnectionGene>();

    }

    public int add_connection(int i, int o)
    {
        ConnectionGene n = new ConnectionGene(i, o);
        genome[n.innovation] = n;
        matrix[i][o] = n;
        NEAT.max_len = Mathf.Max(NEAT.max_len, genome.Count);
        return n.innovation;
    }

    public void add_node(int i, int o, float w)
    {
        ConnectionGene n1 = new ConnectionGene(i, n_neurons, 1.0f);
        ConnectionGene n2 = new ConnectionGene(n_neurons, o, w);

        genome[n1.innovation] = n1;
        genome[n2.innovation] = n2;

        foreach (List<ConnectionGene> l in matrix)
        {
            l.Add(null);
        }
        matrix.Add(new List<ConnectionGene>(n_neurons));

        matrix[i][n_neurons] = n1;
        matrix[n_neurons][o] = n2;

        hidden.Add(n_neurons);

        NEAT.max_len = Mathf.Max(NEAT.max_len, genome.Count);

        n_neurons++;
    }

    public void mutate()
    {
        if (Random.Range(0, 1) < 0.8f)
        {
            foreach (ConnectionGene gen in genome.Values)
            {
                if (Random.Range(0, 1) < NEAT.weight_mutation_rate)
                {
                    gen.w = gen.w + NEAT.step * 2 * (Random.Range(-1, 1));
                }
            }
        }

        if (Random.Range(0, 1) < NEAT.new_link_mutation_rate)
        {
            List<List<int>> perms = new List<List<int>>();
            perms.AddRange(Permutations.permute(hidden));

            List<int> aux = new List<int>(input);
            aux.AddRange(output);
            foreach (int i in aux)
            {
                foreach (int j in aux)
                {
                    if (i != j)
                        perms.Add(new List<int>(new int[] { i, j }));
                }
            }

            while (perms.Count != 0) {
                List<int> c = Range.choice(perms);
                int i = c[0];
                int o = c[1];

                if (matrix[i][o] == null)
                {
                    add_connection(i, o);
                    break;
                }
                else
                {
                    perms.Remove(c);
                }
            }
        }

        if (Random.Range(0, 1) < NEAT.new_node_mutation_rate)
        {
            if (genome.Count == 0)
            {
                return;
            }

            ConnectionGene c = Range.choice(new List<ConnectionGene>(genome.Values));
            while (!c.enable) {
                c = Range.choice(new List<ConnectionGene>(genome.Values));
            }

            c.enable = false;
            add_node(c.input, c.output, c.w);
        }
    }

    public void force_mutate()
    {
        if (Random.Range(0, 1) < 0.8f)
        {
            foreach (ConnectionGene gen in genome.Values)
            {
                if (Random.Range(0, 1) < NEAT.weight_mutation_rate)
                {
                    gen.w = gen.w + NEAT.step * 2 * (Random.Range(-1, 1));
                }
            }
        }

        if (Random.Range(0, 1) < 0.5f)
        {
            List<List<int>> perms = new List<List<int>>();
            perms.AddRange(Permutations.permute(hidden));

            List<int> aux = new List<int>(input);
            aux.AddRange(output);
            foreach (int i in aux)
            {
                foreach (int j in aux)
                {
                    if (i != j)
                        perms.Add(new List<int>(new int[] { i, j }));
                }
            }

            while (perms.Count != 0)
            {
                List<int> c = Range.choice(perms);
                int i = c[0];
                int o = c[1];

                if (matrix[i][o] == null)
                {
                    add_connection(i, o);
                    break;
                }
                else
                {
                    perms.Remove(c);
                }
            }
        }
        else
        {
            if (genome.Count == 0)
            {
                return;
            }

            ConnectionGene c = Range.choice(new List<ConnectionGene>(genome.Values));
            while (!c.enable)
            {
                c = Range.choice(new List<ConnectionGene>(genome.Values));
            }

            c.enable = false;
            add_node(c.input, c.output, c.w);
        }
    }

    public float[] process(int[] inputs)
    {
        if(inputs.Length != input.Count)
        {
            throw new NotImplementedException();
        }

        Dictionary<int, float> results = new Dictionary<int, float>();
        
        for (int j = 0; j < inputs.Length; j++)
        {
            results[j] = inputs[j];
        }

        float[] res = new float[output.Count];

        for (int j = 0; j < inputs.Length; j++)
        {
            res[j] = NEAT.f_activation_function(backrec(j, results, new List<int>()));
        }
        return res;
    }
    private float backrec(int neuron, Dictionary<int,float> results, List<int> visited)
    {
        if (results.ContainsKey(neuron))
        {
            return results[neuron];
        }
        if (visited.Contains(neuron))
        {
            return 0;
        }

        float res = 0;

        for (int i = 0; i < n_neurons; i++)
        {
            ConnectionGene entry = matrix[i][neuron];
            if(entry != null && entry.enable)
            {
                visited.Add(neuron);
                res += backrec(i, results, visited) * entry.w;
                visited.Remove(neuron);
            }
        }
        results[neuron] = NEAT.f_inner_activation_function(res);
        return results[neuron];

    }

    public float calcDistance(Individual ind)
    {
        List<int> k1 = new List<int>(genome.Keys);
        List<int> k2 = new List<int>(ind.genome.Keys);
        k1.Sort();
        k2.Sort();

        int border = Mathf.Min(Utils.Max(k1), Utils.Max(k2));

        HashSet<int> set1 = new HashSet<int>(k1);
        HashSet<int> set2 = new HashSet<int>(k2);
        HashSet<int> disjoint1 = new HashSet<int>(k1);
        HashSet<int> disjoint2 = new HashSet<int>(k2);

        disjoint1.ExceptWith(set2);
        disjoint2.ExceptWith(set1);

        disjoint1.UnionWith(disjoint2);

        List<int> differ = new List<int>(disjoint1);

        int disjoint = 0;
        int i = differ[0];
        while(i <= border)
        {
            i = differ[disjoint];
            disjoint++;
        }

        int excess = differ.Count - disjoint;

        float w_mean = 0;

        set1.IntersectWith(set2);

        if (set1.Count == 0)
            return Mathf.Infinity;
        foreach(int g in set1)
        {
            w_mean += Mathf.Abs(genome[g].w - ind.genome[g].w);
        }
        w_mean /= set1.Count;
        int N = 15;
        float distance = NEAT.c1 * (excess / N) + NEAT.c2 * (disjoint / N) + NEAT.c3 * w_mean;

        return distance;

    }

    public void score()
    {
        fitness = NEAT.fitness(this);
    }
}
