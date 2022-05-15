using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class Individual : IComparable, ICloneable
{
    public List<int> input;
    public List<int> output;
    public List<int> hidden;

    public static Generation gen;
    public static Tournament tour;

    public Specie specie;

    public int n_neurons;

    public List<List<ConnectionGene>> matrix = new List<List<ConnectionGene>>();

    public Dictionary<int, ConnectionGene> genome;

    public float adj_fitness = -1.0f;
    public float fitness = 0f;
    public int violations = 0;
    public int wins = 0;
    
    
    public int pos;
    public string name;

    public string log = "";
    public List<Individual> parents = new List<Individual>();

    public Individual(int inp = -1, int _output = -1)
    {

        input = Range.getRange(0, inp);
        output = Range.getRange(inp, inp + _output);
        hidden = new List<int>();

        n_neurons = inp + _output;

        for (int i = 0; i < n_neurons; i++)
        {
            matrix.Add(new List<ConnectionGene>());
            for (int j = 0; j < n_neurons; j++)
            {
                matrix[i].Add(null);
            }
        }

        genome = new Dictionary<int, ConnectionGene>();

        name = God.getName();

    }

    public int add_connection(int i, int o)
    {

        try
        {
            ConnectionGene n = new ConnectionGene(i, o);
            genome[n.innovation] = n;
            matrix[i][o] = n;
            NEAT.max_len = Mathf.Max(NEAT.max_len, genome.Count);
            return n.innovation;
        }
        catch
        {
            return -1;
        }

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


        List<ConnectionGene> aux = new List<ConnectionGene>();
        for (int j = 0; j < n_neurons + 1; j++)
            aux.Add(null);

        matrix.Add(aux);


        matrix[i][n_neurons] = n1;
        matrix[n_neurons][o] = n2;


        hidden.Add(n_neurons);

        n_neurons++;


        NEAT.max_len = Mathf.Max(NEAT.max_len, genome.Count);


    }

    public void mutate()
    {
        if (Random.Range(0, 1) < 0.8f)
        {
            foreach (ConnectionGene gen in genome.Values)
            {
                if (Random.Range(0, 1) < NEAT.weight_mutation_rate)
                {
                    float jump = Random.Range(-1.0f, 1.0f);
                    gen.w = gen.w + NEAT.step * jump;
                    if (gen.w > 4.0f)
                        gen.w = 4.0f;
                    if (gen.w < -4.0f)
                        gen.w = -4.0f;
                }
            }
        }

        if (Random.Range(0, 1) < NEAT.new_link_mutation_rate)
        {
            List<List<int>> perms = new List<List<int>>();

            List<int> outs = new List<int>(output);
            outs.AddRange(hidden);
            List<int> ins = new List<int>(input);
            ins.AddRange(hidden);

            foreach (int i in ins)
            {
                foreach (int o in outs)
                {
                    if (i != o)
                        perms.Add(new List<int>() { i, o });
                }
            }

            while (perms.Count != 0)
            {
                List<int> c = Range.choice(perms);
                int i = c[0];
                int o = c[1];


                if (matrix[i][o] is null)
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
            while (!c.enable)
            {
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
                    gen.w = gen.w + NEAT.step * (Random.Range(-1, 1));
                    if (gen.w > 4.0f)
                        gen.w = 4.0f;
                    if (gen.w < -4.0f)
                        gen.w = -4.0f;
                }
            }
        }

        if (Random.Range(0, 1) < 0.5f)
        {
            List<List<int>> perms = new List<List<int>>();

            List<int> outs = new List<int>(output);
            outs.AddRange(hidden);
            List<int> ins = new List<int>(input);
            ins.AddRange(hidden);

            foreach (int i in ins)
            {
                foreach (int o in outs)
                {
                    if (i != o)
                        perms.Add(new List<int>() { i, o });
                }
            }

            while (perms.Count != 0)
            {
                List<int> c = Range.choice(perms);
                int i = c[0];
                int o = c[1];

                if (matrix[i][o] is null)
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
        if (Random.Range(0, 1) < 0.5f)
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

            add_node(c.input, c.output, c.w);

        }
    }

    public float[] process(float[] inputs)
    {
        if (inputs.Length != input.Count)
        {
            throw new NotImplementedException();
        }

        Dictionary<int, float> results = new Dictionary<int, float>();

        for (int j = 0; j < inputs.Length; j++)
        {
            results[j] = inputs[j];
        }

        float[] res = new float[output.Count];
        int i = 0;
        foreach (int j in output)
        {
            res[i] = NEAT.f_activation_function(backrec(j, results, new List<int>()));
            i++;
        }
        return res;
    }
    private float backrec(int neuron, Dictionary<int, float> results, List<int> visited)
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
            if (!(entry is null))
            {
                if (entry.enable)
                {
                    visited.Add(neuron);
                    res += backrec(i, results, visited) * entry.w;
                    visited.Remove(neuron);
                }
            }
        }
        results[neuron] = NEAT.f_inner_activation_function(res);
        return results[neuron];

    }

    public float calcDistance(Dictionary<int, ConnectionGene> adn)
    {
        try
        {
            List<int> k1 = new List<int>(genome.Keys);
            List<int> k2 = new List<int>(adn.Keys);
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
            differ.Sort();

            float disjoint = 0;
            if (differ.Count > 0)
            {
                int aux = differ[0];
                int i = 0;
                while (aux < border)
                {
                    aux = differ[i];
                    disjoint++;
                    i++;
                }
            }

            float excess = differ.Count - disjoint;

            float w_mean = 0;

            set1.IntersectWith(set2);

            if (set1.Count == 0)
                return Mathf.Infinity;
            foreach (int g in set1)
            {
                w_mean += Mathf.Abs(genome[g].w - adn[g].w);
            }
            w_mean /= set1.Count;
            float N = Utils.Max(genome.Count, adn.Count);
            float distance = NEAT.c1 * (excess / N) + NEAT.c2 * (disjoint / N) + NEAT.c3 * w_mean;

            return distance;
        }
        catch
        {
            return Mathf.Infinity;
        }


    }

    public void score()
    {
        Arena a = Enviroment.getArena();
        while (a is null)
        {
            WaitForSeconds w = new WaitForSeconds(1.0f);
            a = Enviroment.getArena();
        }
        int i = a.coords[0];
        int j = a.coords[1];
        God.spawn_game(this, i, j);

    }

    public IEnumerator fight(Individual enemy)
    {
        Arena a = Enviroment.getArena();
        while (a is null)
        {
            WaitForSeconds w = new WaitForSeconds(1.0f);
            yield return w;
            a = Enviroment.getArena();
        }
        int i = a.coords[0];
        int j = a.coords[1];
        God.spawn_game(this, enemy, a.pos, i, j);

        yield return null;

    }

    public int CompareTo(object obj)
    {
        int r = fitness.CompareTo(((Individual)obj).fitness);
        return r*-1;
    }

    public object Clone()
    {
        return IndividualFactory.buildIndividual(input.Count, output.Count, new List<ConnectionGene>(genome.Values));
    }

    public void save(string path)
    {
        //Save the genome as json
        string json = "{";
        json += "\"log\":" + log + ",";
        json += "\"input\":" + input.Count + ",";
        json += "\"output\":" + output.Count + ",";

        json += "\"genome\":[";
        bool first = true;
        foreach (ConnectionGene c in genome.Values)
        {
            if (!first)
                json += ",";
            first = false;
            json += "{";
            json += "\"input\":" + c.input + ",";
            json += "\"output\":" + c.output + ",";
            json += "\"w\":" + c.w.ToString().Replace(',', '.') + ",";
            json += "\"enable\":" + (c.enable ? 1:0)+ ",";
            json += "\"innovation\":" + c.innovation;
            json += "}";
        }
        json += "]";
        json += "}";

        File.WriteAllText("Champions/"+path, json);
    }
}
