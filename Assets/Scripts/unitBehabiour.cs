using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IUnit
{
    public void dealDamage(int p);
    string getHealth();
}
public class unitBehabiour : MonoBehaviour, IUnit
{
    // Start is called before the first frame update
    public GameObject tower;
    public GameObject healthPrefab;
    public Brain controller;
    public string unit = "";

    //STATS
    public int health;
    public float MoveSpeed;
    public float range;
    public int power;
    public float attSpeed;
    // -------------------

    private float nextDmgEvent;
    public int direction;
    public string targetTag = null;
    public LayerMask targetLayer;
    void Start()
    {
        string s = direction == 1 ? "Enemy" : "Ally";
        targetTag = s;
        targetLayer = LayerMask.GetMask(s);

        GameObject healthbar = Instantiate(healthPrefab, gameObject.transform);
        healthbar.GetComponent<healthBar>().unit = gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        //am i dead
        if(health <= 0)
        {
            if (unit == "s")
                controller.units[0]--;
            if (unit == "d")
                controller.units[1]--;
            if (unit == "t")
                controller.units[2]--;
            if (unit == "p")
                controller.units[3]--;
            
            Destroy(gameObject);
        }

        Vector2 position = transform.position + new Vector3(0.6f*direction, 0, 0);
        Vector2 towards = transform.right * direction;
        Debug.DrawRay(position, towards* range, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(position, towards, range, targetLayer);
        if (hit.collider != null)
        {
            if (targetTag != null)
            {
                if (Time.time >= nextDmgEvent)
                {
                    nextDmgEvent = Time.time + 1 / (attSpeed*NEAT.game_speed);
                    IUnit src = hit.collider.gameObject.GetComponent<IUnit>();
                    src.dealDamage(power);
                }
            }
            
        }
        else
        {
            transform.position += transform.right * direction * MoveSpeed * NEAT.game_speed * Time.deltaTime;
            nextDmgEvent = Time.time + 1/ (attSpeed * NEAT.game_speed);
        }
    }

    //FUNCTIONS
    void IUnit.dealDamage(int p)
    {
        health -= p;
        try
        {
            healthBar lifes = this.gameObject.transform.GetComponentInChildren<healthBar>();
            lifes.update();
        }
        catch
        {

        }
        
    }
    public void explode()
    {
        health -= 35;
        try
        {
            healthBar lifes = this.gameObject.transform.GetComponentInChildren<healthBar>();
            lifes.update();
        }
        catch
        {

        }
    }
    public string getHealth()
    {
        return health.ToString();
    }
}
