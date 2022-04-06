using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IUnit
{
    void dealDamage(int p);
    string getHealth();
}
public class unitBehabiour : MonoBehaviour, IUnit
{
    // Start is called before the first frame update
    public GameObject tower;
    public GameObject healthPrefab;
    public Brain controller;


    //STATS
    public int health;
    public int MoveSpeed;
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
        healthBar lifes = this.gameObject.transform.GetChild(0).GetComponent<healthBar>();
        lifes.update();
    }

    public string getHealth()
    {
        return health.ToString();
    }
}
