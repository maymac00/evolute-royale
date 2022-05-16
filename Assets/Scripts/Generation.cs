using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class Arena
{
    public Vector3 pos;
    public int[] coords = new int[2];
    public Arena(Vector3 pos, int i, int j)
    {
        this.pos = pos;
        coords[0] = i;
        coords[1] = j;
    }
}

public class Generation : MonoBehaviour
{
    public Camera mainCam;
    public GameObject game;
    
    public GameObject gen_prefab; 
    public GameObject tournament_prefab; 

    static public List<Specie> host_species = new List<Specie>();
    static public List<Specie> parasite_species = new List<Specie>();

    static public readonly int N_INDS = 32;
    static public readonly int N_MAX_GENS = 2;
    static public List<Individual> hall_of_fame = new List<Individual>();
    public static List<Individual> host = new List<Individual>();
    public static List<Individual> parasite = new List<Individual>();

    public int games_played = 0;
    public int target_games_played;
    
    public static int n_gen = 0;

    // Start is called before the first frame update
    void Start()
    {

        tournament_prefab = (GameObject)Resources.Load("Prefabs/Tournament");
        Individual.gen = this;

        // Speciation
        NEAT.distance_thld += 0.3f;
        float host_thld = NEAT.distance_thld;
        float parasite_thld = NEAT.distance_thld;

        foreach (Individual ind in host)
            speciate(host_species, ind, host_thld);

        foreach (Individual ind in parasite)
            speciate(parasite_species, ind, parasite_thld);

        int try_count = 0;
        while ((host_species.Count < 3 || host_species.Count > 7 || parasite_species.Count < 3 || parasite_species.Count > 7) && try_count < 20) 
        { 
            if (host_species.Count < NEAT.species_pool_size)
                host_thld -= UnityEngine.Random.Range(0.2f, 0.6f);
            else if (host_species.Count > NEAT.species_pool_size * 1.75f)
                host_thld += UnityEngine.Random.Range(0.2f, 0.6f);

            if (host_thld < 0.3)
            {
                host_thld = 0.3f;
                break;
            }

            if (parasite_species.Count < NEAT.species_pool_size)
                parasite_thld -= UnityEngine.Random.Range(0.2f, 0.6f);
            else if (parasite_species.Count > NEAT.species_pool_size * 1.2f)
                parasite_thld += UnityEngine.Random.Range(0.2f, 0.6f);

            if (parasite_thld < 0.3)
            {
                parasite_thld = 0.3f;
                break;
            }

            foreach (Specie specie in host_species)
                specie.reset();
            foreach (Specie specie in parasite_species)
                specie.reset();

            foreach (Individual ind in host) 
            {
                ind.specie = null;
                speciate(host_species, ind, host_thld);
            }

            foreach (Individual ind in parasite)
            {
                ind.specie = null;
                speciate(parasite_species, ind, parasite_thld);
            }

            host_species.RemoveAll(s => s.individuals.Count == 0);
            parasite_species.RemoveAll(s => s.individuals.Count == 0);

            try_count++;
        }

        //remove empty species
        host_species.RemoveAll(s => s.individuals.Count == 0);
        parasite_species.RemoveAll(s => s.individuals.Count == 0);

        if(host_species.Count > 7) {
            // put all species with one individual in the same specie
            Specie aux = null;
            List<Specie> toRemove = new List<Specie>();
            foreach (Specie specie in host_species)
            {
                if (specie.individuals.Count == 1)
                {
                    if (aux == null)
                    {
                        aux = new Specie(specie.individuals[0]);
                        toRemove.Add(specie);
                    }
                    else
                    {
                        aux.individuals.Add(specie.individuals[0]);
                        toRemove.Add(specie);
                    }
                }
            }

            foreach (Specie specie in toRemove)
                host_species.Remove(specie);
        }

        if (parasite_species.Count > 7)
        {
            // put all species with one individual in the same specie
            Specie aux = null;
            List<Specie> toRemove = new List<Specie>();
            foreach (Specie specie in parasite_species)
            {
                if (specie.individuals.Count == 1)
                {
                    if (aux == null)
                    {
                        aux = new Specie(specie.individuals[0]);
                        toRemove.Add(specie);
                    }
                    else
                    {
                        aux.individuals.Add(specie.individuals[0]);
                        toRemove.Add(specie);
                    }
                }
            }

            foreach (Specie specie in toRemove)
                parasite_species.Remove(specie);
        }



        Debug.Log("Host Species: " + host_species.Count + "Species: " + parasite_species.Count);

        target_games_played = 0;

        foreach(Specie s in host_species)
        {
            target_games_played += ((s.individuals.Count - 1) * s.individuals.Count / 2);
            target_games_played += s.individuals.Count; // play with coded
        }

        foreach (Specie s in parasite_species)
        {
            target_games_played += ((s.individuals.Count - 1) * s.individuals.Count / 2);
            target_games_played += s.individuals.Count; // play with coded            
        }

        next_gen();
    }

    public void speciate(List<Specie> niche ,Individual ind, float thd=-1) { 
        
        if (niche.Count < 1)
        {
            niche.Add(new Specie(ind));
            return;
        }

        if (thd == -1)
        {
            thd = NEAT.distance_thld;
        }
        
        
        float distance;
        if (ind.specie != null) { 
            distance = ind.calcDistance(ind.specie.adn);
            if (distance < thd * NEAT.bloodrate)
            {
                ind.specie.individuals.Add(ind);
                return;
            }
        }

        List <Specie> sp = new List<Specie>(niche);

        while (sp.Count > 0)
        {
            Specie s = Range.choice(sp);
            distance = ind.calcDistance(s.adn);
            if (distance < thd)
            {
                s.individuals.Add(ind);
                ind.specie = s;
                return;
            }
            else {
                sp.Remove(s);
            }
        }
        niche.Add(new Specie(ind));
    }

    // Update is called once per frame
    void Update()
    {
        if (Enviroment.empty_arena() && games_played >= target_games_played)
        {

            Tournament.host_champions = new List<Individual>();
            Tournament.parasite_champions = new List<Individual>();

            foreach (Specie s in host_species)
            {
                s.adjust_scores();
                Tournament.host_champions.Add(s.individuals[0]);
            }
            foreach (Specie s in parasite_species)
            {
                s.adjust_scores();
                Tournament.parasite_champions.Add(s.individuals[0]);
            }


            /*
            if(n_gen < Enviroment.n_gens)
            {
                GameObject go = Instantiate(gen_prefab);
                go.name = "Generation " + n_gen;
                n_gen++;
            }
            else
            {
                // god.fes_algo
            }*/
            if (n_gen < Enviroment.n_gens)
            {
                GameObject go = Instantiate(tournament_prefab);
                go.name = "Tournament " + n_gen;
                target_games_played = -1;
            }

            Destroy(gameObject, 0.5f);

        }

    }

    // this function saves an array to a text file
    public void save_fitness()
    {
        int k = 0;
        StreamWriter writer = new StreamWriter("fitness_gen" + Enviroment.n_gens + ".txt");
        writer.Write("Host:\n");
        foreach (Specie s in host_species)
        {
            if (s.fitness.Length > 0)
            {
                writer.Write("Mean: " + s.averageFitness + " Adj: " + Utils.mean(s.adj_fitness) + " Size: " + s.individuals.Count);
                writer.WriteLine();
                for (int i = 0; i < s.fitness.Length; i++)
                {
                    writer.Write(s.fitness[i] + "\t");
                }
                writer.WriteLine();
                k++;
            }
        }
        k = 0;
        writer.Write("Parasite:\n");
        foreach (Specie s in parasite_species)
        {
            if (s.fitness.Length > 0)
            {
                writer.Write("Mean: " + s.averageFitness + " Adj: " + Utils.mean(s.adj_fitness) + " Size: " + s.individuals.Count);
                writer.WriteLine();
                for (int i = 0; i < s.fitness.Length; i++)
                {
                    writer.Write(s.fitness[i] + "\t");
                }
                writer.WriteLine();
                k++;
            }
        }

        writer.Close();
    }

    public void next_gen()
    {
        foreach (Specie s in host_species)
        {
            s.mutate();
            s.adn = s.individuals[0].genome;
            s.battle(this);
        }
        foreach (Specie s in parasite_species)
        {
            s.mutate();
            s.adn = s.individuals[0].genome;
            s.battle(this);
        }
        if (true)
        {
            NEAT.innovations = new Dictionary<ConnectionGene, int>();
            NEAT.innovations_arr = new int[2048];
        } 
}
    
    public static void natural_selection()
    {

        if (host_species.Count != 0)
        {
            List<Individual> new_population = new List<Individual>();

            int P = Enviroment.N_INDS;
            List<float> means = new List<float>();
            for (int i = 0; i < host_species.Count; i++)
            {
                means.Add(host_species[i].tournament_wins);
            }
            float S = Utils.sum(means);
            List<Specie> spc = new List<Specie>(host_species);


            foreach (Specie s in spc)
            {
                int n_ind = (int)(s.tournament_wins / S * P);
                if (n_ind > s.individuals.Count * 3)
                {
                    n_ind = s.individuals.Count * 3;
                }
                if (n_ind < s.individuals.Count * 0.5f)
                {
                    n_ind = (int)(s.individuals.Count * 0.5f);
                }

                if (s.tournament_wins == 0)
                {
                    s.uselessness += 1;
                }

                if (n_ind > 0 && s.uselessness < NEAT.dropoff)
                {
                    List<Individual> offspring = s.getOffspring(Math.Max(2, n_ind));
                    new_population.AddRange(offspring);
                }
                else
                {
                    s.individuals = new List<Individual>();
                }
            }
            foreach (Specie s in host_species)
            {
                s.reset();
            }
            if (hall_of_fame.Count > 0)
                new_population.Add(Range.choice(Tournament.hall_of_fame));
            Generation.host = new_population;
        }

        if (parasite_species.Count != 0)
        {
            List<Individual> new_population = new List<Individual>();

            int P = Enviroment.N_INDS;
            List<float> means = new List<float>();
            for (int i = 0; i < parasite_species.Count; i++)
            {
                means.Add(parasite_species[i].tournament_wins);
            }
            float S = Utils.sum(means);
            List<Specie> spc = new List<Specie>(parasite_species);


            foreach (Specie s in spc)
            {
                int n_ind = (int)(s.tournament_wins / S * P);
                if (n_ind > s.individuals.Count * 3)
                {
                    n_ind = s.individuals.Count * 3;
                }
                if (n_ind < s.individuals.Count * 0.5f)
                {
                    n_ind = (int)(s.individuals.Count * 0.5f);
                }

                if (s.tournament_wins == 0)
                {
                    s.uselessness += 1;
                }

                if (n_ind > 0 && s.uselessness < NEAT.dropoff)
                {
                    List<Individual> offspring = s.getOffspring(Math.Max(2, n_ind));
                    new_population.AddRange(offspring);
                }
                else
                {
                    s.individuals = new List<Individual>();
                }
            }
            foreach (Specie s in parasite_species)
            {
                s.reset();
            }
            if (hall_of_fame.Count > 0)
                new_population.Add(Range.choice(Tournament.hall_of_fame));
            Generation.parasite = new_population;
        }

    }
}
