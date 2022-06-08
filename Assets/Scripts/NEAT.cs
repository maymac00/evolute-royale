using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class NEAT
{
    public static float game_speed = 1.0f;

    public static float new_node_mutation_rate = 0.03f;
    //public static float new_node_mutation_rate = 0.3f;
    public static float new_link_mutation_rate = 0.05f;
    //public static float new_link_mutation_rate = 0.2f;
    public static float weight_mutation_rate = 0.9f;
    public static int n_innovations = 1;
    public static Dictionary<ConnectionGene, int> innovations = new Dictionary<ConnectionGene, int>();
    public static int[] innovations_arr = new int[2048];

    public static int dropoff = 3;
    public static int max_len = 0;
    public static float step = 1.5f;
    public static int species_pool_size = 7;
    public static float bloodrate = 1.0f;
    public static float survival_threshold = 0.334f;

    public static float distance_thld = 6.0f;
    public static float c1 = 2.0f; // Disjoint
    public static float c2 = 2.0f; // Excess
    public static float c3 = 1.0f; // Weight

    public const int N_INPUTS = 12;
    public const int N_OUTPUTS = 6;

    public static string activation_function = "relu";
    public static string inner_activation_function = "relu";
    
    //internal static string distance_type = "fenotype";
    internal static string distance_type = "genotype";
    internal static bool penalty = false;

    public static void check_innovation(ConnectionGene connectionGene)
    {
        int i = connectionGene.input;
        int j = connectionGene.output;
        if (NEAT.innovations_arr[connectionGene.GetHashCode()] != 0)
        {
            connectionGene.innovation = NEAT.innovations_arr[connectionGene.GetHashCode()];
        }
        else
        {
            NEAT.innovations_arr[connectionGene.GetHashCode()] = NEAT.n_innovations;
            connectionGene.innovation = NEAT.n_innovations;
            NEAT.n_innovations++;
        }
    }

    public static float f_activation_function(float x)
    {
        switch (activation_function)
        {
            case "relu":
                return Mathf.Max(0, x);
            case "sigmoid_mod":
                return (4 / (1+Mathf.Pow((float)Math.E, -x))) - 2;

            default:
                return (1 / (1 + Mathf.Pow((float)Math.E, -x)));
        }
    }

    public static float f_inner_activation_function(float x)
    {
        switch (inner_activation_function)
        {
            case "relu":
                return Mathf.Max(0, x);
            case "sigmoid_mod":
                return (4 / (1 + Mathf.Pow((float)Math.E, -x))) - 2;
            default:
                return (1 / (1 + Mathf.Pow((float)Math.E, -x)));
        }
    }

    public static Individual crossover(Individual ind1, Individual ind2)
    {
        if (ind1.fitness > ind2.fitness)
        {
            Individual temp = ind1;
            ind1 = ind2;
            ind2 = temp;
        }

        Dictionary<int, ConnectionGene> genome1 = ind1.genome;
        Dictionary<int, ConnectionGene> genome2 = ind2.genome;

        List<ConnectionGene> genome3 = new List<ConnectionGene>();

        for (int i = 0; i < genome1.Count; i++)
        {
            ConnectionGene connectionGene = genome1.Values.ToArray()[i];
            if (genome2.ContainsKey(connectionGene.innovation) && genome2[connectionGene.innovation].enable && Random.Range(0, 1) < 0.5f)
            {
                genome3.Add(genome2[connectionGene.innovation]);
            }
            else
            {
                genome3.Add(connectionGene);
            }
        }
        Individual child = IndividualFactory.buildIndividual(ind1.input.Count, ind1.output.Count, genome3);
        child.parents.Add(ind1);
        child.parents.Add(ind2);
        child.specie = ind1.specie;
        return child;
    }

}
