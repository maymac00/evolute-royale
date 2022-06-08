using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coded_behaviour : Brain
{
    int[] actions;
    int i;
    // Start is called before the first frame update
    void Start()
    {
        calcSpawn();
        int[] aux = { 0, 0, 1, 0, 0, 4, 2, 3, 2};
        actions = aux;
        i = 0;
        InvokeRepeating("act", 1, (float)(1 / NEAT.game_speed));
    }

    public void act()
    {
        int action = actions[i];
        bool r = false;
        switch (action)
        {
            case 0:
                r = spawnSquare();
                break;
            case 1:
                r = spawnDiamond();
                break;
            case 2:
                r = spawnTriangle();
                break;
            case 3:
                r = spawnPentagon();
                break;
            case 4:
                r = explode();
                break;
        }
        if (r)
        {
            i++;
            if (i >= actions.Length)
            {
                i = 0;
            }
        }
    }
}
