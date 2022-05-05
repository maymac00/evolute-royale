using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Specie
{
    public static God god;
    public List<Individual> individuals;

    public float[] fitness;
    public float[] adj_fitness;
    public float averageFitness = 0;
    public float bestFitness = 0;
    public int uselessness;

    public Specie(Individual ind) {
        individuals = new List<Individual>();
        individuals.Add(ind);
    }

    public void score_individuals()
    {
        fitness = new float[individuals.Count];
        int i = 0;
        foreach (Individual ind in individuals)
        {
            ind.pos = i;
            ind.specie = this;
            ind.score();
            i++;
        }
    }

    public void adjust_scores()
    {
        individuals.Sort();
        adj_fitness = new float[individuals.Count];
        float sum = Utils.sum(fitness);
        averageFitness = sum / fitness.Length;
        averageFitness = sum / fitness.Length;
        if(fitness.Length > 0)
            bestFitness = Utils.Max(fitness);

        
        int i = 0;
        foreach (Individual ind in individuals)
        {
            float[] sh = new float[individuals.Count];
            int j = 0;
            foreach (Individual bro in individuals)
            {
                
                float dist = ind.calcDistance(bro);
                if(dist > NEAT.distance_thld)
                {
                    sh[j] = 0.0f;
                }
                else
                {
                    if(dist == 0)
                        sh[j] = 1.0f;
                    else
                        sh[j] = 1.0f - (dist/NEAT.distance_thld);
                }
                j++;
            }
            sum = Utils.sum(sh);
            if(sum == 0) {
                ind.adj_fitness = ind.fitness;
                adj_fitness[i] = ind.fitness;
            }
            else
            {
                ind.adj_fitness = ind.fitness / sum;
                adj_fitness[i] = ind.fitness / sum;
            }
            i++;
        }
        // METRIQUES
    }

    public List<Individual> getOffspring(int n)
    {
        List<Individual> offspring = new List<Individual>();
        // Champion
        offspring.Add(individuals[Utils.Max_index(fitness)]);

        int f = -1;
        if(individuals.Count > 15)
            f = (int)individuals.Count / 5;
        List<Individual> parents;
        if (f != -1)
            parents = individuals.GetRange(0, f);
        else
            parents = individuals;
        for (int i = 0; i < n - 1; i++)
        {
            Individual ind1 = Range.choice(parents);
            Individual ind2 = Range.choice(individuals);
            offspring.Add(NEAT.crossover(ind1, ind2));
            offspring[offspring.Count-1].mutate();
        }
        return offspring;
    }

    internal void mutate()
    {
        for (int i = 0; i < individuals.Count; i++)
        {
            individuals[i].mutate();
        }
    }
}
