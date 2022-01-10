using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthBar : MonoBehaviour
{
    public GameObject unit;
    private TextMesh textmesh;
    // Use this for initialization
    void Start()
    {
        textmesh = GetComponent<TextMesh>();
        textmesh.text = unit.GetComponent<IUnit>().getHealth();
        textmesh.transform.position += new Vector3(0, unit.GetComponent<SpriteRenderer>().bounds.size.y/2 + 0.5f,0);
    }

    public void update()
    {
        textmesh.text = unit.GetComponent<IUnit>().getHealth();
    }
}
