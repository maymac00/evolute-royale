using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlled_behaviour : Brain
{
    // Start is called before the first frame update
    public string team;
    KeyCode squ;
    KeyCode tri;
    KeyCode penta;
    KeyCode dia;
    void Start()
    {
        calcSpawn();
        if(team == "a")
        {
            squ = KeyCode.Q;
            dia = KeyCode.W;
            tri = KeyCode.E;
            penta = KeyCode.R;
        }
        else
        {
            squ = KeyCode.U;
            dia = KeyCode.I;
            tri = KeyCode.O;
            penta = KeyCode.P;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(squ))
        {
            spawnSquare();
        }
        else if (Input.GetKeyUp(dia))
        {
            spawnDiamond();
        }
        else if(Input.GetKeyUp(tri))
        {
            spawnTriangle();
        }
        else if (Input.GetKeyUp(penta))
        {
            spawnPentagon();
        }

    }
}
