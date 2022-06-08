

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Kmeans
{

    public static void run(List<Specie> niche, List<Individual> individuals, int k)
    {

        const int n_inputs = 50;
        const int iterations = 50;

        float[][] data = new float[n_inputs][];
        for (int i = 0; i < n_inputs; i++)
        {
            data[i] = new float[12];
            for (int j = 0; j < 12; j++)
            {
                data[i][j] = Random.Range(0.0f, 1.0f);
            }
        }

        float[][][] centroids = new float[k][][];
        float[][][] oldcentroids = new float[k][][];

        if (niche.Count > 0) {
            for (int i = 0; i < k; i++)
            {
                centroids[i] = new float[n_inputs][];
                oldcentroids[i] = new float[n_inputs][];
                if (i < k)
                {
                    Individual ind = IndividualFactory.buildIndividual(12, 6, new List<ConnectionGene>(niche[0].adn.Values));
                    for (int j = 0; j < n_inputs; j++)
                    {
                        float[] res = individuals[i].process(data[j]);
                        int action = Utils.Max_index(res);
                        if(action == 0)
                            centroids[i][j] = new float[] {0,0};
                        else if (action == 1)
                            centroids[i][j] = new float[] { 1, 0 };
                        else if (action == 2)
                            centroids[i][j] = new float[] { -1,  };
                        else if (action == 3)
                            centroids[i][j] = new float[] { 0, 1 };
                        else if (action == 4)
                            centroids[i][j] = new float[] { 0, -1 };
                        else if (action == 5)
                            centroids[i][j] = new float[] { 0, 0};

                    }
                }
                else
                {
                    for (int j = 0; j < n_inputs; j++)
                    {
                        centroids[i][j] = Range.choice(new List<float[]>() { new float[] { 0, 0 }, new float[] { 1, 0 }, new float[] { -1, 0 }, new float[] { 0, 1 }, new float[] { 0, -1 } });
                    }
                }
                Array.Copy(centroids[i], oldcentroids[i], n_inputs);
            }
        }
        else
        {
            for (int i = 0; i < k; i++)
            {
                centroids[i] = new float[n_inputs][];
                oldcentroids[i] = new float[n_inputs][];
                
                for (int j = 0; j < n_inputs; j++)
                {
                    centroids[i][j] = Range.choice(new List<float[]>() { new float[] { 0, 0 }, new float[] { 1, 0 }, new float[] { -1, 0 }, new float[] { 0, 1 }, new float[] { 0, -1 } });
                }

                Array.Copy(centroids[i], oldcentroids[i], n_inputs);
            }
        }


        float[][][]outputs = new float[individuals.Count][][];

        for (int i = 0; i < individuals.Count; i++)
        {
            outputs[i] = new float[n_inputs][];
            for (int j = 0; j < n_inputs; j++)
            {
                float[] res = individuals[i].process(data[j]);
                int action = Utils.Max_index(res);
                if (action == 0)
                    outputs[i][j] = new float[] {0, 0};
                else if (action == 1)
                    outputs[i][j] = new float[] { 1, 0 };
                else if (action == 2)
                    outputs[i][j] = new float[] { -1, 0};
                else if (action == 3)
                    outputs[i][j] = new float[] { 0, 1 };
                else if (action == 4)
                    outputs[i][j] = new float[] { 0, -1 };
                else if (action == 5)
                    outputs[i][j] = new float[] { 0, 0 };
            }
        }

        int[] clusters = new int[individuals.Count];
        int it = 0;
        while (it < iterations)
        {
            // Evaluate the input data for each individual and assign it to a cluster

            for (int i = 0; i < individuals.Count; i++)
            {
                float[] distances = new float[k];
                for (int j = 0; j < k; j++)
                {
                    float[][] output = outputs[i];
                    float[][] centroid = centroids[j];
                    float distance = 0.0f;
                    for (int l = 0; l < n_inputs; l++)
                    {
                        for (int q = 0; q < 2; q++)
                        {
                            distance += Mathf.Pow(output[l][q] - centroid[l][q], 2);
                        }
                    }
                    distances[j] = distance;
                }
                int index = 0;
                float min = distances[0];
                for (int j = 1; j < k; j++)
                {
                    if (distances[j] < min)
                    {
                        min = distances[j];
                        index = j;
                    }
                }
                clusters[i] = index;
            }

            // Update the centroids

            for (int i = 0; i < k; i++)
            {
                float[][] centroid = new float[n_inputs][];
                int count = 0;
                for (int j = 0; j < n_inputs; j++)
                {
                    centroid[j] = new float[] { 0, 0 };
                    if (clusters[j] == i)
                    {
                        for (int l = 0; l < individuals.Count; l++)
                        {
                            
                            for (int q = 0; q < 2; q++)
                            {
                                centroid[j][q] += outputs[l][j][q];
                            }
                        }
                        count++;
                    }
                }
                for (int l = 0; l < n_inputs; l++)
                {
                    for (int q = 0; q < 2; q++)
                    {
                        centroid[l][q] /= count;
                    }
                }
                centroids[i] = centroid;
            }

            //calculate distance between old and new centroids

            float acc_distance = 0.0f;
            for (int i = 0; i < k; i++)
            {
                float[][] centroid = centroids[i];
                float[][] oldcentroid = oldcentroids[i];
                for (int j = 0; j < n_inputs; j++)
                {
                    for (int q = 0; q < 2; q++)
                    {
                        acc_distance += Mathf.Pow(centroid[j][q] - oldcentroid[j][q], 2);
                    }
                }
            }

            if (acc_distance < 0.01f)
            {
                break;
            }

            for (int i = 0; i < k; i++)
            {
                Array.Copy(centroids[i], oldcentroids[i], n_inputs);
            }

            it++;
        }


        
        int init_species = niche.Count;
        if (init_species > 0)
        {
            for(int i = 0; i < k; i++)
            {
                if (i < init_species)
                {
                    for (int j = 0; j < individuals.Count; j++)
                    {
                        if (clusters[j] == i)
                        {
                            niche[i].individuals.Add(individuals[j]);
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < individuals.Count; j++)
                    {
                        if (clusters[j] == i)
                        { 
                            if(niche.Count < i)
                            {
                                niche.Add(new Specie(individuals[j]));
                            }
                            else
                            {
                                niche[i].individuals.Add(individuals[j]);
                                individuals[j].specie = niche[i];
                            }
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < individuals.Count; j++) {
                    if (clusters[j] == i)
                    {
                        if (niche.Count <= i )
                        {
                            niche.Add(new Specie(individuals[j]));
                        }
                        else
                        {
                            niche[i].individuals.Add(individuals[j]);
                            individuals[j].specie = niche[i];
                        }
                    }
                }
            }
        }



        

        for (int i = 0; i < individuals.Count; i++)
        {
            // individuals[i].specie(clusters[i]);
        }


        return;
    }
}