using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConnectionGene : IComparable
{

    public int input;
    public int output;
    public float w;
    public bool enable;
    public int innovation;
    

    public ConnectionGene(int _input, int _output, float _w=0.0f, bool _enable=true)
    {
        if (_w == 0.0f)
            _w = Random.Range(-2.0f, 2.0f);
        input = _input;
        output = _output;
        w = _w;
        enable = _enable;

        NEAT.check_innovation(this);
    }

    public static bool operator ==(ConnectionGene lcg, ConnectionGene rcg) {
        return lcg.input == rcg.input && lcg.output == rcg.output;
    }

    public static bool operator !=(ConnectionGene lcg, ConnectionGene rcg)
    {
        return lcg.input != rcg.input || lcg.output != rcg.output;
    }

    public static bool operator < (ConnectionGene lcg, ConnectionGene rcg)
    {
        return lcg.innovation < rcg.innovation;
    }

    public static bool operator >(ConnectionGene lcg, ConnectionGene rcg)
    {
        return lcg.innovation > rcg.innovation;
    }

    public override int GetHashCode()
    {
        int h = output ^ input;
        int hl = output ^ input;
        return h;
    }

    public int CompareTo(object obj)
    {
        return innovation < ((ConnectionGene)obj).innovation ? -1 : 1;
    }
}
