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

    private float range = 3;
    private int power = 5;
    private float attSpeed = 1.5f;
    // -------------------

    private float nextDmgEvent;
    public int direction;
    public LayerMask targetLayer;

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
        Vector2 position = transform.position + new Vector3(0.6f * direction, -1, 0);
        Vector2 towards = transform.right * direction;
        Debug.DrawRay(position, towards * range, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(position, towards, range, targetLayer);
        if (hit.collider != null)
        {
            if (Time.time >= nextDmgEvent)
            {
                nextDmgEvent = Time.time + 1 / attSpeed;
                IUnit src = hit.collider.gameObject.GetComponent<IUnit>();
                src.dealDamage(power);
            }
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
