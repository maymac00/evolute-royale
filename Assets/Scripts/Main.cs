using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Brain : MonoBehaviour
{
    public GameObject main;
    public GameObject target;
    public GameObject myTower;
    public GameObject prefabSquare;
    public GameObject prefabTriangle;
    public GameObject prefabPentagon;
    public GameObject prefabDiamond;
    public int gold = 0;

    public string log;
    protected Vector3[] spawnPoints = new Vector3[4];
    public int direction;

    protected void calcSpawn()
    {
        spawnPoints[0] = new Vector3(myTower.transform.position.x + direction * 0.5f, -4.264f, 0);
        spawnPoints[1] = new Vector3(myTower.transform.position.x + direction * 0.5f, -4.364f, 0);
        spawnPoints[2] = new Vector3(myTower.transform.position.x + direction * 0.5f, -4.64f, 0);
        spawnPoints[3] = new Vector3(myTower.transform.position.x + direction * 0.5f, -4.3f, 0);
    }
    protected void spawnSquare()
    {
        if (gold >= 3) {
            log +="s";
            gold -= 3;
            GameObject sq = Object.Instantiate(prefabSquare, spawnPoints[0], Quaternion.identity);
            unitBehabiour src = sq.GetComponent<unitBehabiour>();
            sq.GetComponent<SpriteRenderer>().color = src.direction == 1 ? Color.green : Color.red;
            sq.transform.SetParent(gameObject.transform.parent);
            src.tower = target;
            src.direction = direction;
            string s = src.direction == 1 ? "Ally" : "Enemy";
            sq.tag = s;
            sq.layer = src.direction == 1 ? 8 : 9;
            sq.GetComponent<SpriteRenderer>().color = src.direction == 1 ? Color.green : Color.red;
            sq.name = src.direction == 1 ? "Ally" : "Enemy";
        }
    }

    protected void spawnDiamond()
    {
        if (gold >= 3)
        {
            log += "d";
            gold -= 3;
            GameObject sq = Object.Instantiate(prefabDiamond, spawnPoints[1], Quaternion.identity);
            unitBehabiour src = sq.GetComponent<unitBehabiour>();
            sq.transform.SetParent(gameObject.transform.parent);
            src.tower = target;
            src.direction = direction;
            string s = src.direction == 1 ? "Ally" : "Enemy";
            sq.tag = s;
            sq.layer = src.direction == 1 ? 8 : 9;
            sq.GetComponent<SpriteRenderer>().color = src.direction == 1 ? Color.green : Color.red;
            sq.name = src.direction == 1 ? "Ally" : "Enemy";
        }
    }

    protected void spawnTriangle()
    {
        if (gold >= 5)
        {
            log += "t";
            gold -= 5;
            GameObject sq = Object.Instantiate(prefabTriangle, spawnPoints[2], Quaternion.identity);
            unitBehabiour src = sq.GetComponent<unitBehabiour>();
            sq.transform.SetParent(gameObject.transform.parent);
            sq.transform.parent = gameObject.transform.parent;
            src.tower = target;
            src.direction = direction;
            string s = src.direction == 1 ? "Ally" : "Enemy";
            sq.tag = s;
            sq.layer = src.direction == 1 ? 8 : 9;
            sq.GetComponent<SpriteRenderer>().color = src.direction == 1 ? Color.green : Color.red;
            sq.name = src.direction == 1 ? "Ally" : "Enemy";
        }
    }

    protected void spawnPentagon()
    {
        if (gold >= 5)
        {
            log += "p";
            gold -= 5;
            GameObject sq = Object.Instantiate(prefabPentagon, spawnPoints[3], Quaternion.identity);
            unitBehabiour src = sq.GetComponent<unitBehabiour>();
            sq.transform.SetParent(gameObject.transform.parent);
            sq.transform.parent = gameObject.transform.parent;
            src.tower = target;
            src.direction = direction;
            string s = src.direction == 1 ? "Ally" : "Enemy";
            src.targetTag = "tower";
            sq.tag = s;
            sq.layer = src.direction == 1 ? 8 : 9;
            sq.GetComponent<SpriteRenderer>().color = src.direction == 1 ? Color.green : Color.red;
            sq.name = src.direction == 1 ? "Ally" : "Enemy";
        }
    }
    protected void explode() {
        if (gold >= 7) {
            Transform[] children = transform.parent.GetComponentsInChildren<Transform>();
            for (int i = 11; i < children.Length; i++) {
                Debug.Log("");
            }
        }
    }
    
}


public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    public string ally_controller;
    public string enemy_controller;
    GameObject brain_ally;
    GameObject brain_enemy;
    int timer = 0;

    void Start()
    {
        switch (ally_controller)
        {
            case "coded":
                brain_ally = new GameObject("allyBrain");
                brain_ally.transform.SetParent(gameObject.transform.parent);
                coded_behaviour src_cod = brain_ally.AddComponent<coded_behaviour>();
                src_cod.target = gameObject.transform.parent.transform.GetChild(1).gameObject;
                src_cod.myTower = gameObject.transform.parent.transform.GetChild(0).gameObject;
                src_cod.prefabSquare = (GameObject)Resources.Load("Prefabs/Square", typeof(GameObject));
                src_cod.prefabTriangle = (GameObject)Resources.Load("Prefabs/Triangle", typeof(GameObject));
                src_cod.prefabPentagon = (GameObject)Resources.Load("Prefabs/Pentagon", typeof(GameObject));
                src_cod.prefabDiamond = (GameObject)Resources.Load("Prefabs/Diamond", typeof(GameObject));
                src_cod.direction = 1;
                break;

            case "controlled":
                brain_ally = new GameObject("allyBrain");
                brain_ally.transform.SetParent(gameObject.transform.parent);
                controlled_behaviour src_cont = brain_ally.AddComponent<controlled_behaviour>();
                src_cont.target = gameObject.transform.parent.transform.GetChild(1).gameObject;
                src_cont.myTower = gameObject.transform.parent.transform.GetChild(0).gameObject;
                src_cont.prefabSquare = (GameObject)Resources.Load("Prefabs/Square", typeof(GameObject));
                src_cont.prefabTriangle = (GameObject)Resources.Load("Prefabs/Triangle", typeof(GameObject));
                src_cont.prefabPentagon = (GameObject)Resources.Load("Prefabs/Pentagon", typeof(GameObject));
                src_cont.prefabDiamond = (GameObject)Resources.Load("Prefabs/Diamond", typeof(GameObject));
                src_cont.team = "a";
                src_cont.direction = 1;
                break;
            default:
                brain_ally = new GameObject("allyBrain");
                brain_ally.transform.SetParent(gameObject.transform.parent);
                controlled_behaviour src = brain_ally.AddComponent<controlled_behaviour>();
                src.target = gameObject.transform.parent.transform.GetChild(1).gameObject;
                src.myTower = gameObject.transform.parent.transform.GetChild(0).gameObject;
                src.prefabSquare = (GameObject)Resources.Load("Prefabs/Square", typeof(GameObject));
                src.prefabTriangle = (GameObject)Resources.Load("Prefabs/Triangle", typeof(GameObject));
                src.prefabPentagon = (GameObject)Resources.Load("Prefabs/Pentagon", typeof(GameObject));
                src.prefabDiamond = (GameObject)Resources.Load("Prefabs/Diamond", typeof(GameObject));
                src.direction = 1;
                break;
        }

        switch (enemy_controller)
        {
            case "controlled":
                brain_enemy = new GameObject("enemyBrain");
                brain_enemy.transform.SetParent(gameObject.transform.parent);
                controlled_behaviour src_cont = brain_enemy.AddComponent<controlled_behaviour>();
                src_cont.target = gameObject.transform.parent.transform.GetChild(0).gameObject;
                src_cont.myTower = gameObject.transform.parent.transform.GetChild(1).gameObject;
                src_cont.prefabSquare = (GameObject)Resources.Load("Prefabs/Square", typeof(GameObject));
                src_cont.prefabTriangle = (GameObject)Resources.Load("Prefabs/Triangle", typeof(GameObject));
                src_cont.prefabPentagon = (GameObject)Resources.Load("Prefabs/Pentagon", typeof(GameObject));
                src_cont.prefabDiamond = (GameObject)Resources.Load("Prefabs/Diamond", typeof(GameObject));
                src_cont.direction = -1;
                src_cont.team = "e";
                break;

            case "coded":
                brain_enemy = new GameObject("enemyBrain");
                brain_enemy.transform.SetParent(gameObject.transform.parent);
                coded_behaviour src_cod = brain_enemy.AddComponent<coded_behaviour>();
                src_cod.target = gameObject.transform.parent.transform.GetChild(0).gameObject;
                src_cod.myTower = gameObject.transform.parent.transform.GetChild(1).gameObject;
                src_cod.prefabSquare = (GameObject)Resources.Load("Prefabs/Square", typeof(GameObject));
                src_cod.prefabTriangle = (GameObject)Resources.Load("Prefabs/Triangle", typeof(GameObject));
                src_cod.prefabPentagon = (GameObject)Resources.Load("Prefabs/Pentagon", typeof(GameObject));
                src_cod.prefabDiamond = (GameObject)Resources.Load("Prefabs/Diamond", typeof(GameObject));
                src_cod.direction = -1;
                break;
            default:
                brain_enemy = new GameObject("enemyBrain");
                brain_enemy.transform.SetParent(gameObject.transform.parent);
                coded_behaviour src = brain_enemy.AddComponent<coded_behaviour>();
                src.target = gameObject.transform.parent.transform.GetChild(0).gameObject;
                src.myTower = gameObject.transform.parent.transform.GetChild(1).gameObject;
                src.prefabSquare = (GameObject)Resources.Load("Prefabs/Square", typeof(GameObject));
                src.prefabTriangle = (GameObject)Resources.Load("Prefabs/Triangle", typeof(GameObject));
                src.prefabPentagon = (GameObject)Resources.Load("Prefabs/Pentagon", typeof(GameObject));
                src.prefabDiamond = (GameObject)Resources.Load("Prefabs/Diamond", typeof(GameObject));
                src.direction = -1;
                break;
        }

        InvokeRepeating("secondUpdate", 1, 1);
    }

    void Update()
    {
        
    }

    void secondUpdate()
    {
        Brain ally = brain_ally.GetComponent<Brain>();
        Brain enemy = brain_enemy.GetComponent<Brain>();
        ally.gold += 1;
        enemy.gold += 1;
        timer++;
        gameObject.transform.parent.transform.GetChild(4).GetComponent<TextMesh>().text = ally.gold.ToString();
        gameObject.transform.parent.transform.GetChild(5).GetComponent<TextMesh>().text = enemy.gold.ToString();
        gameObject.GetComponent<TextMesh>().text = "Timer: " + timer.ToString() + " s";

        ally.log += "_";
        enemy.log += "_";
    }

    public void endGame(int winner)
    {
        //Finish game, set winner etc...
        Destroy(gameObject.transform.parent.gameObject, 3);
        CancelInvoke("secondUpdate");
    }
}