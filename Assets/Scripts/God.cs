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

public class God : MonoBehaviour
{
    private TextMesh textmesh;
    public Camera mainCam;
    public GameObject game;
    
    public Vector3[,] grid;
    public byte[,] arena;
    public int grid_size = 8;
    public int grid_x = 0;
    public int grid_y = 0;
    public int speed;

    public int n_gens = 0;

    public int n = 64;

    public List<Specie> species = new List<Specie>();
    public bool finished_gen = false;

    // Start is called before the first frame update
    void Start()
    {
        NEAT.game_speed = speed;

        textmesh = GetComponent<TextMesh>();

        arena = new byte[grid_size, grid_size];

        grid = new Vector3[grid_size, grid_size];
        Vector3 h_vector = new Vector3(25.0f, 0, 0);
        Vector3 v_vector = new Vector3(0, 15.0f, 0);
        for (int i = 0; i < grid_size; i++) {
            for (int j = 0; j < grid_size; j++)
            {
                grid[i, j] = new Vector3(0.0f, 0.0f, 0.0f) + i * v_vector + j * h_vector;
                arena[i, j] = 0;
            }
        }

        Individual[] population = new Individual[64];
        Individual.god = this;
       
        for (int i = 0; i < population.Length; i++)
        {
            population[i] = new Individual(12,5);
            for (int j = 0; j < 16; j++)
            {
                population[i].force_mutate();
            }
            speciate(population[i]);
        }
        Debug.Log("Species: " + species.Count);
        next_gen();
    }

    public void speciate(Individual ind) {
        if (species.Count == 0)
        {
            species.Add(new Specie(ind));
            return;
        }
        
        List <Specie> sp = new List<Specie>(species);

        while (sp.Count > 0)
        {
            Specie s = Range.choice(sp);
            Individual leader = Range.choice(s.individuals);
            float distance = leader.calcDistance(ind);
            if (distance < NEAT.distance_thld)
            {
                s.individuals.Add(ind);
                return;
            }
            else {
                sp.Remove(s);
            }
        }
        species.Add(new Specie(ind));
    }
    
    public void spawn_game(Individual ind1, Individual ind2, Vector2 pos)
    {
        GameObject g = Instantiate(game, pos, Quaternion.identity);
        g.name = "Game " + pos[0] + "," + pos[1];
        Main m = g.GetComponentInChildren<Main>();
        m.position = pos;
        m.ally_ind = ind1;
        m.enemy_ind = ind2;
        m.ally_controller = "evolutive";
        m.enemy_controller = "evolutive";
        m.exec();
    }

    public void spawn_game(Individual ind1, int i, int j)
    {
        Vector2 pos = grid[i,j];
        GameObject g = Instantiate(game, pos, Quaternion.identity);
        g.name = "Game " + i + "," + i;
        Main m = g.GetComponentInChildren<Main>();
        m.position = pos;
        m.coords = new int[2] { i, j };
        m.ally_ind = ind1;
        m.ally_controller = "evolutive";
        m.enemy_controller = "coded";
        m.exec();
        m.gameFinishedEvent += ind1.notifyWin;
    }

    public Arena getArena()
    {
        for (int i = 0; i < grid_size; i++)
        {
            for (int j = 0; j < grid_size; j++)
            {
                if (arena[i, j] == 0)
                {
                    arena[i, j] = 1;
                    return new Arena(grid[i, j], i, j);
                }
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            grid_y = grid_y == grid_size - 1 ? grid_y : grid_y + 1;
            mainCam.transform.position = grid[grid_x, grid_y] + new Vector3(0, 0, -10);
            gameObject.transform.position = grid[grid_x, grid_y] + new Vector3(-9, 5, 0);
            textmesh.text = grid_x.ToString() + "," + grid_y.ToString();
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            grid_y = grid_y == 0 ? grid_y : grid_y - 1;
            mainCam.transform.position = grid[grid_x, grid_y] + new Vector3(0, 0, -10);
            gameObject.transform.position = grid[grid_x, grid_y] + new Vector3(-9, 5, 0);
            textmesh.text = grid_x.ToString() + "," + grid_y.ToString();
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            grid_x = grid_x == 0 ? grid_x : grid_x - 1;
            mainCam.transform.position = grid[grid_x, grid_y] + new Vector3(0, 0, -10);
            gameObject.transform.position = grid[grid_x, grid_y] + new Vector3(-9, 5, 0);
            textmesh.text = grid_x.ToString() + "," + grid_y.ToString();
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            grid_x = grid_x == grid_size - 1 ? grid_x : grid_x + 1;
            mainCam.transform.position = grid[grid_x, grid_y] + new Vector3(0, 0, -10);
            gameObject.transform.position = grid[grid_x, grid_y] + new Vector3(-9, 5, 0);
            textmesh.text = grid_x.ToString() + "," + grid_y.ToString();
        }
        
        if (empty_arena())
        {
            Debug.Log("Start selection");
            foreach (Specie s in species)
            {
                s.adjust_scores();
            }

            //sort species by average_fitness
            species.Sort((x, y) => y.averageFitness.CompareTo(x.averageFitness));

            save_fitness();

            // Metrics

            natural_selection();
            n_gens++;
            
            next_gen();
        }
        
    }

    private bool empty_arena()
    {
        for (int i = 0; i < grid_size; i++)
        {
            for (int j = 0; j < grid_size; j++)
            {
                if (arena[i, j] == 1)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // this function saves an array to a text file
    public void save_fitness()
    {
        int k = 0;
        StreamWriter writer = new StreamWriter("fitness_gen" + n_gens + ".txt");
        foreach (Specie s in species)
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
        Debug.Log("Start next_gen");
        
        foreach (Specie s in species)
        {
            s.mutate();
        }

        foreach (Specie s in species)
        {
            s.score_individuals();
        }

        
    }

    private void natural_selection()
    {
        int P = n;
        List<float> means = new List<float>();
        for (int i = 0; i < species.Count; i++)
        {
            means.Add(Utils.mean(species[i].adj_fitness));
        }
        float S = Utils.sum(means);
        List<Specie> spc = new List<Specie>(species);
        species = new List<Specie>();

        foreach (Specie s in spc)
        {
            int n = (int)(Utils.mean(s.adj_fitness) / S * P);
            if (n > s.individuals.Count * 2)
            {
                n = s.individuals.Count * 2;
            }
            if (n < s.individuals.Count * 0.5f)
            {
                n = (int)(s.individuals.Count * 0.5f);
            }


            if (n > 0 && s.uselessness < NEAT.dropoff)
            {
                List<Individual> offspring = s.getOffspring(n);
                s.individuals = new List<Individual>();
                foreach (Individual i in offspring)
                {
                    speciate(i);
                }
            }
            else
            {
                s.individuals = new List<Individual>();
            }
        }

        // rebuild list of species so there isn't any specie with no individuals
        List<Specie> species_aux = new List<Specie>();

        
        Debug.Log("End of natural selection");
    }
}
