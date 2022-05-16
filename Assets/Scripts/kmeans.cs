

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Kmeans
{

    public static void run(List<Individual> individuals, int k)
    {

        const int n_inputs = 10;
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

        float[][] centroids = new float[k][];
        float[][] oldcentroids = new float[k][];

        for (int i = 0; i < k; i++)
        {
            centroids[i] = new float[12];
            oldcentroids[i] = new float[12];
            for (int j = 0; j < 12; j++)
            {
                centroids[i][j] = Random.Range(0.0f, 1.0f);
            }
            Array.Copy(centroids[i], oldcentroids[i], 12);
        }

        float[][] outputs = new float[individuals.Count][];

        for (int i = 0; i < individuals.Count; i++)
        {
            outputs[i] = new float[12];
            for (int j = 0; j < n_inputs; j++)
            {
                float[] res = individuals[i].process(data[j]);
                outputs[i][j] = Utils.Max_index(res);
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
                    float[] output = outputs[i];
                    float[] centroid = centroids[j];
                    float distance = 0.0f;
                    for (int l = 0; l < 12; l++)
                    {
                        distance += Mathf.Pow(output[l] - centroid[l], 2);
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
                float[] centroid = new float[12];
                int count = 0;
                for (int j = 0; j < individuals.Count; j++)
                {
                    if (clusters[j] == i)
                    {
                        for (int l = 0; l < 12; l++)
                        {
                            centroid[l] += outputs[j][l];
                        }
                        count++;
                    }
                }
                for (int l = 0; l < 12; l++)
                {
                    centroid[l] /= count;
                }
                centroids[i] = centroid;
            }

            //calculate distance between old and new centroids

            float acc_distance = 0.0f;
            for (int i = 0; i < k; i++)
            {
                float[] centroid = centroids[i];
                float[] oldcentroid = oldcentroids[i];
                for (int j = 0; j < 12; j++)
                {
                    acc_distance += Mathf.Pow(centroid[j] - oldcentroid[j], 2);
                }
            }

            if (acc_distance < 0.01f)
            {
                break;
            }

            for (int i = 0; i < k; i++)
            {
                Array.Copy(centroids[i], oldcentroids[i], 12);
            }

            it++;
        }

        return;
    }
}