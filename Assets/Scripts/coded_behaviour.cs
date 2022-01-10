using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coded_behaviour : Brain
{
    // Start is called before the first frame update
    void Start()
    {
        calcSpawn();
        spawnPentagon();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("holaa");
    }
}
