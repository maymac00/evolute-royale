using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Range
{
    public static List<int> getRange(int s, int e)
    {
        List<int> l = new List<int>();
        for (int i = s; i < e; i++)
        {
            l.Add(i);
        }
        return l;
    }

    public static T choice<T>(List<T> l)
    {
        int n = l.Count;
        int r = Mathf.RoundToInt(Random.Range(0, n));
        return l[r];
    }
}

public static class Permutations
{
    public static List<List<int>> permute(List<int> arr)
    {
        List<List<int>> l = new List<List<int>>();
        for (int i = 0;
            i < (int)Mathf.Pow(arr.Count, 2); i++)
        {
            List<int> aux = convert_To_Len_th_base(i, arr, (int)Mathf.Pow(arr.Count,2));
            if(aux.Count > 0)
            {
                l.Add(aux);
            }
        }
        return l;
    }

    private static List<int> convert_To_Len_th_base(int n, List<int> arr,
                                   int len)
    {
        List<int> l = new List<int>();

        for (int i = 0; i < 2; i++)
        {
            l.Add(arr[n % len]);
            n /= len;
        }
        if(l[0] != l[1])
        {
            return l;
        }
        return new List<int>();
    }

}

public static class Utils
{
    public static int Max(List<int> l)
    {
        int res = l[0];
        foreach(int it in l)
        {
            if(it > res)
            {
                res = it;
            }
        }
        return res;
    }

    public static float Max(float[] l)
    {
        float res = l[0];
        foreach (float it in l)
        {
            if (it > res)
            {
                res = it;
            }
        }
        return res;
    }

    public static int Min(List<int> l)
    {
        int res = l[0];
        foreach (int it in l)
        {
            if (it < res)
            {
                res = it;
            }
        }
        return res;
    }

    public static float Min(float[] l)
    {
        float res = l[0];
        foreach (float it in l)
        {
            if (it < res)
            {
                res = it;
            }
        }
        return res;
    }

    public static int Min_index(float[] l)
    {
        int res = 0;
        for (int i = 1; i < l.Length; i++) {
            if (l[i] < l[res])
            {
                res = i;
            }
        }
        return res;
    }

    public static int Max_index(float[] l)
    {
        int res = 0;
        for (int i = 1; i < l.Length; i++)
        {
            if (l[i] > l[res])
            {
                res = i;
            }
        }
        return res;
    }
    public static T Max<T>(T lhs, T rhs)
    {
        return Comparer<T>.Default.Compare(lhs, rhs) > 0 ? lhs : rhs;
    }

    public static float normalize(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }

    public static List<Individual> insert_sorted(List<Individual> l, Individual i)
    {
        int n = l.Count;
        for (int j = 0; j < n; j++)
        {
            if (l[j].fitness > i.fitness)
            {
                l.Insert(j, i);
                return l;
            }
        }
        l.Add(i);
        return l;
    }

    public static float sum(float[] l)
    {
        float res = 0;
        foreach (float it in l)
        {
            res += it;
        }
        return res;
    }

    public static float mean(float[] l)
    {
        return sum(l) / l.Length;
    }

    public static float sum(List<float> l)
    {
        float res = 0;
        foreach (float it in l)
        {
            res += it;
        }
        return res;
    }

    public static float mean(List<float> l)
    {
        return sum(l) / l.Count;
    }
}