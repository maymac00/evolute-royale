using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Specie
{
    public static Generation god;
    public List<Individual> individuals;

    public Dictionary<int, ConnectionGene> adn;

    public float[] fitness;
    public float[] adj_fitness;
    public float averageFitness = 0;
    public float bestFitness = 0;
    public int uselessness;
    public int games_played = 0;
    public int tournament_wins = 0;

    public Specie(Individual ind) {
        ind.specie = this;
        individuals = new List<Individual>();
        individuals.Add(ind);
        adn = ind.genome;
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

    public void battle(MonoBehaviour mymonoB)
    {
        foreach (Individual ind in individuals)
        {
            ind.fitness = 0;
            ind.adj_fitness = 0;
            ind.log = "";
            ind.wins = 0;
            ind.violations = 0;
        }

        for (int i = 0; i < individuals.Count; i++)
        {
            for (int j = i + 1; j < individuals.Count; j++)
            {
                games_played++;
                mymonoB.StartCoroutine(individuals[i].fight(individuals[j]));
            }

            mymonoB.StartCoroutine(individuals[i].fight());
        }
        
    }

    public void adjust_scores()
    {
        individuals.Sort();
        adj_fitness = new float[individuals.Count];

        
        int i = 0;
        foreach (Individual ind in individuals)
        {
            float[] sh = new float[individuals.Count];
            int j = 0;
            foreach (Individual bro in individuals)
            {
                
                float dist = ind.calcDistance(bro.genome);
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
            float sum = Utils.sum(sh);
            if(sum == 0) {
                ind.adj_fitness = ind.fitness * 10;
                adj_fitness[i] = ind.fitness * 10;
            }
            else
            {
                ind.adj_fitness = ind.fitness * 10 / sum;
                adj_fitness[i] = ind.fitness * 10 / sum;
            }
            i++;
        }
        // METRIQUES
    }

    public List<Individual> getOffspring(int n)
    {
        List<Individual> offspring = new List<Individual>();
        // Champion
        offspring.Add(individuals[0]);
        offspring[0].fitness = 0;
        int f = -1;
        if(individuals.Count > 6)
            f = (int)(individuals.Count * NEAT.survival_threshold);
        List<Individual> parents;
        if (f != -1)
            parents = individuals.GetRange(0, f);
        else
            parents = individuals;
        for (int i = 0; i < n - 1; i++)
        {
            Individual ind1 = Range.choice(parents);
            Individual ind2 = Range.choice(parents);
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

    public void reset()
    {
        individuals = new List<Individual>();
        tournament_wins = 0;
    }
}
