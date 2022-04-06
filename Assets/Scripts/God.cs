using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class God : MonoBehaviour
{
    private TextMesh textmesh;
    public Camera mainCam;
    public GameObject game;
    
    public Vector3[,] grid;
    public int grid_size = 8;
    public int grid_x = 0;
    public int grid_y = 0;
    public int speed;
    public float[,] fitnesses;

    // Start is called before the first frame update
    void Start()
    {
        NEAT.game_speed = speed;
        fitnesses = new float[grid_size, grid_size];

        textmesh = GetComponent<TextMesh>();
        
        grid = new Vector3[grid_size, grid_size];
        Vector3 h_vector = new Vector3(25.0f, 0, 0);
        Vector3 v_vector = new Vector3(0, 15.0f, 0);
        for (int i = 0; i < grid_size; i++) {
            for (int j = 0; j < grid_size; j++)
            {
                grid[i, j] = new Vector3(0.0f, 0.0f, 0.0f) + i * v_vector + j * h_vector;
                fitnesses[i, j] = float.MinValue;
            }
        }

        fitnesses = new float[grid_size, grid_size];

        for (int i = 0; i < grid_size; i++)
        {
            for (int j = 0; j < grid_size; j++)
            {
                fitness(new Individual(12, 5), i, j, fitnesses);
            }
        }

    }

    void spawn_game(Individual ind1, Individual ind2, Vector2 pos)
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
    void test_2_inds()
    {
        Individual ind1 = new Individual(12, 5);
        Individual ind2 = new Individual(12, 5);

        GameObject g = Instantiate(game, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        Main m = g.GetComponentInChildren<Main>();
        m.ally_ind = ind1;
        m.enemy_ind = ind2;
        m.ally_controller = "evolutive";
        m.enemy_controller = "evolutive";
        m.exec();
    }

    float fitness(Individual ind, int i, int j, float[,] fitnesses)
    {
        GameObject g = Instantiate(game, grid[i, j], Quaternion.identity);
        g.name = "Game " + i + "," + j;
        Main m = g.GetComponentInChildren<Main>();
        m.position = grid[i, j];
        m.coords = new int[] { i, j };
        m.ally_ind = ind;
        m.ally_controller = "evolutive";
        m.enemy_controller = "coded";
        m.god = this;
        m.exec();
        return 0.0f;
    }
    
    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            grid_y = grid_y == grid_size-1 ? grid_y : grid_y + 1;
            mainCam.transform.position = grid[grid_x, grid_y] + new Vector3(0,0,-10);
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
            grid_x = grid_x == grid_size-1 ? grid_x : grid_x + 1;
            mainCam.transform.position = grid[grid_x, grid_y] + new Vector3(0, 0, -10);
            gameObject.transform.position = grid[grid_x, grid_y] + new Vector3(-9, 5, 0);
            textmesh.text = grid_x.ToString() + "," + grid_y.ToString();
        }

        bool b = true;
        for (int i = 0; i < grid_size; i++)
        {
            for (int j = 0; j < grid_size; j++)
            {
                if (fitnesses[i, j] == float.MaxValue)
                {
                    b = false;
                    break;
                }
            }
        }
        if (b)
        {

            save_array(fitnesses, "fitnesses.txt");
            Destroy(this);
        }

    }

    // this function saves an array to a text file
    public void save_array(float[,] array, string filename)
    {
        StreamWriter writer = new StreamWriter(filename);
        for (int i = 0; i < grid_size; i++)
        {
            for (int j = 0; j < grid_size; j++)
            {
                writer.Write(array[i, j] + " ");
            }
            writer.WriteLine();
        }
        writer.Close();
    }
}
