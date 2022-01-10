using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class towerBehabiour : MonoBehaviour, IUnit
{
    public int health;
    public GameObject healthPrefab;
    public int team;
    public GameObject main;
    Main src;

    // Start is called before the first frame update
    void Start()
    {
        GameObject healthbar = Instantiate(healthPrefab, gameObject.transform);
        healthbar.GetComponent<healthBar>().unit = gameObject;
        healthbar.transform.localScale = new Vector3(0.05f, 0.05f/6, 0.05f);

        src = main.GetComponent<Main>();
    }

    // Update is called once per frame
    void Update()
    {
        //am i dead
        if (health <= 0)
        { 
            src.endGame(team * -1);
            Destroy(this.gameObject);
        }
    }

    public void dealDamage(int p)
    {
        health-= p;
        healthBar lifes = this.gameObject.transform.GetChild(0).GetComponent<healthBar>();
        lifes.update();
    }

    public string getHealth()
    {
        return health.ToString();
    }
}
