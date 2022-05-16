using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enviroment
{
    
    static public int grid_size = 14;
    static public int grid_x = 0;
    static public int grid_y = 0;

    static public int speed = 10;
    static readonly public int N_INDS = 40;

    static public Vector3[,] grid = new Vector3[grid_size, grid_size];
    static public byte[,] arena = new byte[grid_size, grid_size];

    static public int n_gens = 100;

    static public Arena getArena()
    {
        for (int i = 0; i < Enviroment.grid_size; i++)
        {
            for (int j = 0; j < Enviroment.grid_size; j++)
            {
                if (Enviroment.arena[i, j] == 0)
                {
                    Enviroment.arena[i, j] = 1;
                    return new Arena(Enviroment.grid[i, j], i, j);
                }
            }
        }
        return null;
    }

    static public bool empty_arena()
    {
        for (int i = 0; i < Enviroment.grid_size; i++)
        {
            for (int j = 0; j < Enviroment.grid_size; j++)
            {
                if (Enviroment.arena[i, j] == 1)
                {
                    return false;
                }
            }
        }
        return true;
    }

    static public void grid_setup()
    {
        Vector3 h_vector = new Vector3(25.0f, 0, 0);
        Vector3 v_vector = new Vector3(0, 15.0f, 0);
        for (int i = 0; i < grid_size; i++)
        {
            for (int j = 0; j < grid_size; j++)
            {
                grid[i, j] = new Vector3(0.0f, 0.0f, 0.0f) + i * v_vector + j * h_vector;
                arena[i, j] = 0;
            }
        }
    }
}

public class God : MonoBehaviour
{
    public static GameObject generation_prefab;
    public static GameObject game_prefab = null;

    public static string[] names = null;
    // Start is called before the first frame update

    public void getInitialPopulations() {
        //First gen start
        List<Individual> population = new List<Individual>();
        List<Individual> parasite = new List<Individual>();
        for (int i = 0; i < Enviroment.N_INDS; i++)
        {
            population.Add(new Individual(NEAT.N_INPUTS, NEAT.N_OUTPUTS));
            parasite.Add(new Individual(NEAT.N_INPUTS, NEAT.N_OUTPUTS));
            for (int j = 0; j < 10; j++)
            {
                population[i].mutate();
                parasite[i].mutate();
            }
        }
        Generation.host = population;
        Generation.parasite = parasite;
        GameObject go = Instantiate(generation_prefab);
        go.name = "Generation 0";
        Generation.n_gen = 0;
    }
    public static string getName()
    {
        if (names == null)
            return "Testing";
        return names[Random.Range(0, names.Length)];
    }
    void Start()
    {
        names = new string[20000];
        string[] lines = System.IO.File.ReadAllLines("names.csv");

        for (int i = 0; i < names.Length; i++)
        {
            names[i] = lines[i];
        }

        game_prefab = (GameObject)Resources.Load("Prefabs/Game");
        generation_prefab = (GameObject)Resources.Load("Prefabs/Generation");
        NEAT.game_speed = Enviroment.speed;
        Enviroment.grid_setup();
        getInitialPopulations();
    }

    public static void spawn_game(Individual ind1, int i, int j)
    {
        Vector2 pos = Enviroment.grid[i, j];
        GameObject g = Instantiate(game_prefab, pos, Quaternion.identity);
        g.name = "Game " + i + "," + i;
        Main m = g.GetComponentInChildren<Main>();
        m.position = pos;
        m.coords = new int[2] { i, j };
        m.ally_ind = ind1;
        m.ally_controller = "evolutive";
        m.enemy_controller = "coded";
        m.exec();
    }

    public static void spawn_game(Individual ind1, Individual ind2, Vector2 pos, int i, int j)
    {
        GameObject g = Instantiate(game_prefab, pos, Quaternion.identity);
        g.name = "Game " + pos[0] + "," + pos[1];
        Main m = g.GetComponentInChildren<Main>();
        m.position = pos;
        m.ally_ind = ind1;
        m.enemy_ind = ind2;
        m.ally_controller = "evolutive";
        m.enemy_controller = "evolutive";
        m.coords = new int[2] { i, j };
        m.exec();
    }
}
