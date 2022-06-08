using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evaluator : MonoBehaviour
{

    public string individual;
    public Individual ind;
    public static float[] fitness_progress;

    public float[][] tests_12 = new float[][]
    {
        //             g    s    d    t    p    h    g    s    d    t    p    h
        new float[12] {0.0f,0.0f,0.0f,0.0f,0.0f,1.0f,0.0f,0.0f,0.0f,0.0f,0.0f,1.0f},
        new float[12] {1.0f,0.0f,0.0f,0.0f,0.0f,1.0f,0.0f,0.0f,0.0f,0.0f,0.0f,1.0f},
        new float[12] {1.0f,0.0f,0.0f,0.0f,0.0f,1.0f,0.0f,0.0f,0.0f,1.0f,0.0f,1.0f},
        new float[12] {1.0f,0.0f,0.0f,0.0f,0.0f,1.0f,0.0f,0.0f,0.0f,0.5f,0.0f,1.0f},
        new float[12] {1.0f,0.0f,0.0f,0.0f,0.0f,1.0f,0.0f,0.0f,0.0f,1.0f,0.5f,1.0f},
        new float[12] {1.0f,0.0f,0.0f,0.5f,0.0f,1.0f,0.0f,0.0f,1.0f,0.0f,0.0f,1.0f},
        new float[12] {0.5f,0.0f,0.0f,0.0f,0.0f,1.0f,0.0f,0.0f,0.0f,0.0f,0.0f,1.0f},
        new float[12] {0.5f,0.5f,0.0f,0.0f,0.0f,1.0f,0.0f,0.5f,0.0f,0.0f,0.0f,1.0f},
        new float[12] {0.5f,0.0f,0.5f,0.0f,0.0f,1.0f,0.0f,0.0f,0.0f,0.0f,0.0f,1.0f},
        new float[12] {0.5f,0.0f,0.5f,0.0f,0.0f,1.0f,0.3f,0.0f,0.0f,0.0f,0.0f,1.0f},
        new float[12] {0.5f,0.0f,0.5f,0.0f,0.0f,1.0f,0.7f,0.0f,0.0f,0.0f,0.0f,1.0f},
        new float[12] {0.5f,0.0f,0.0f,0.6f,0.0f,1.0f,1.0f,0.0f,0.0f,0.0f,0.0f,1.0f},
    };

    // Start is called before the first frame update
    void Start()
    {
        // ind = IndividualFactory.buildFromFile(individual);
        Enviroment.grid_setup();
        NEAT.game_speed = Enviroment.speed;
        evaluate_fitness_progress("Champions/Ronda 12/");
    }

    void Update()
    {
        if (Enviroment.empty_arena())
        {
            Debug.Log("Evaluation finished");
        }
    }

    public void evaluate1ind()
    {
        ind = IndividualFactory.buildFromFile(individual);

        float[] test_output = new float[tests_12.Length];
        for (int i = 0; i < tests_12.Length; i++)
        {
            test_output[i] = Utils.Max_index(ind.process(tests_12[i]));
        }
    }

    public void evaluate_fitness_progress(string path)
    {
        // Read files from path
        string[] files = System.IO.Directory.GetFiles(path);
        int nb_files = files.Length;

        // Create fitness progress
        fitness_progress = new float[nb_files];

        // Evaluate each file
        for (int i = 0; i < nb_files; i++)
        {
            // Check that file extension is .json
            if (files[i].Substring(files[i].Length - 5) == ".json")
            {
                ind = IndividualFactory.buildFromFile(files[i]);
                StartCoroutine(ind.evaluate(i));
            }

        }
    }
}
