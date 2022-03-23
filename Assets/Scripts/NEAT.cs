using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class NEAT
{
    public static int dropoff = 15;
    public static int blood_rate = 3;
    public static double max_reward = 500.0;
    public static float new_node_mutation_rate = 0.03f;
    public static float new_link_mutation_rate = 0.05f;
    public static float weight_mutation_rate = 0.9f;
    public static int n_innovations = 0;
    public static Dictionary<int, ConnectionGene> innovations = new Dictionary<int, ConnectionGene>();

    public static int max_len = 0;
    public static float step = 2.5f;
    public static int species_pool_size = 15;
    public static int reps = 1;
    public static string opt = "max";

    public static float distance_thld = 3.0f;
    public static float c1 = 1.0f; // Disjoint
    public static float c2 = 1.0f; // Excess
    public static float c3 = 0.3f; // Weight

    public const int N_INPUTS = 15;
    public const int N_OUTPUTS = 4;

    public static string activation_function = "";
    public static string inner_activation_function = "";

    public static void check_innovation(ConnectionGene connectionGene)
    {
        throw new NotImplementedException();
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

    public static float fitness(Individual ind)
    {
        return 0;
    }
}
