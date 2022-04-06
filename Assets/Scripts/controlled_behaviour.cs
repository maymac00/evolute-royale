using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlled_behaviour : Brain, IBehabiour
{
    // Start is called before the first frame update
    public string team;
    KeyCode squ;
    KeyCode tri;
    KeyCode penta;
    KeyCode dia;
    KeyCode exp;
    void Start()
    {
        calcSpawn();
        if(team == "a")
        {
            squ = KeyCode.Q;
            dia = KeyCode.W;
            tri = KeyCode.E;
            penta = KeyCode.R;
            exp = KeyCode.T;
            
        }
        else
        {
            squ = KeyCode.Y;
            dia = KeyCode.U;
            tri = KeyCode.I;
            penta = KeyCode.O;
            exp = KeyCode.P;
        }
    }

    // Update is called once per frame
    void Update(){
        act();
    }
    

    public void act(float[] inputs = null)
    {
        if (Input.GetKeyUp(squ))
        {
            spawnSquare();
        }
        else if (Input.GetKeyUp(dia))
        {
            spawnDiamond();
        }
        else if (Input.GetKeyUp(tri))
        {
            spawnTriangle();
        }
        else if (Input.GetKeyUp(penta))
        {
            spawnPentagon();
        }
        else if (Input.GetKeyUp(exp)) {
            explode();
        }

    }
}
