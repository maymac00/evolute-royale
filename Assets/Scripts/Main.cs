using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public abstract class Brain : MonoBehaviour
{
    public GameObject main;
    public GameObject target;
    public GameObject myTower;
    public GameObject prefabSquare;
    public GameObject prefabTriangle;
    public GameObject prefabPentagon;
    public GameObject prefabDiamond;
    public Vector3 position;

    public int[] units = new int[4];
    public int gold = 0;

    public string log;
    protected Vector3[] spawnPoints = new Vector3[4];
    public int direction;

    public int winner;
    public int diff;

    protected void calcSpawn()
    {
        spawnPoints[0] =  new Vector3(myTower.transform.position.x + direction * 0.5f, -4.264f + position[1], 0);
        spawnPoints[1] =  new Vector3(myTower.transform.position.x + direction * 0.5f, -4.364f + position[1], 0);
        spawnPoints[2] =  new Vector3(myTower.transform.position.x + direction * 0.5f, -4.64f  + position[1], 0);
        spawnPoints[3] =  new Vector3(myTower.transform.position.x + direction * 0.5f, -4.3f   + position[1], 0);
    }
    protected bool spawnSquare()
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

            units[0] += 1;
            return true;
        }
        return false;
    }

    protected bool spawnDiamond()
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
            units[1] += 1;
            return true;
        }
        return false;
    }

    protected bool spawnTriangle()
    {
        if (gold >= 5)
        {
            log += "t";
            gold -= 5;
            GameObject sq = Object.Instantiate(prefabTriangle, spawnPoints[2], Quaternion.identity);
            unitBehabiour src = sq.GetComponent<unitBehabiour>();
            sq.transform.SetParent(gameObject.transform.parent);
            src.tower = target;
            src.direction = direction;
            string s = src.direction == 1 ? "Ally" : "Enemy";
            sq.tag = s;
            sq.layer = src.direction == 1 ? 8 : 9;
            sq.GetComponent<SpriteRenderer>().color = src.direction == 1 ? Color.green : Color.red;
            sq.name = src.direction == 1 ? "Ally" : "Enemy";
            units[2] += 1;
            return true;
        }
        return false;
    }

    protected bool spawnPentagon()
    {
        if (gold >= 5)
        {
            log += "p";
            gold -= 5;
            GameObject sq = Object.Instantiate(prefabPentagon, spawnPoints[3], Quaternion.identity);
            unitBehabiour src = sq.GetComponent<unitBehabiour>();
            sq.transform.SetParent(gameObject.transform.parent);
            src.tower = target;
            src.direction = direction;
            string s = src.direction == 1 ? "Ally" : "Enemy";
            src.targetTag = "tower";
            sq.tag = s;
            sq.layer = src.direction == 1 ? 8 : 9;
            sq.GetComponent<SpriteRenderer>().color = src.direction == 1 ? Color.green : Color.red;
            sq.name = src.direction == 1 ? "Ally" : "Enemy";
            units[3] += 1;
            return true;
        }
        return false;
    }
    protected bool explode() {
        if (gold >= 7) {
            Transform[] children = transform.parent.GetComponentsInChildren<Transform>();
            for (int i = 11; i < children.Length; i++) {
                Debug.Log("");
            }
            return true;
        }
        return false;
    }

    public float[] getValues()
    {
        float[] values = new float[6];
        values[0] = gold;
        for (int i = 0; i < 4; i++)
            values[i + 1] = units[i];
        towerBehabiour tb = myTower.GetComponent<towerBehabiour>();
        values[5] = tb.health;
        return values;
    }
}

interface IBehabiour
{
    void act(float[] inputs = null);

}

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    public string ally_controller;
    public string enemy_controller;
    public bool autoStart = false;
    GameObject brain_ally;
    GameObject brain_enemy;
    public Individual ally_ind;
    public Individual enemy_ind;
    public Vector3 position;
    public int[] coords;
    public Specie g = null;

    public int ally_fitness;
    public int enemy_fitness;


    public int timer = 0;
    public int time_limit = 120;

    public int winner;
    public int diff;

    private void Start()
    {
        if (autoStart)
        {
            exec();
        }
    }
    public void exec()
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

            case "evolutive":
                brain_ally = new GameObject("allyBrain");
                brain_ally.transform.SetParent(gameObject.transform.parent);
                evolutive_behaviour src_ev = brain_ally.AddComponent<evolutive_behaviour>();
                src_ev.target = gameObject.transform.parent.transform.GetChild(1).gameObject;
                src_ev.myTower = gameObject.transform.parent.transform.GetChild(0).gameObject;
                src_ev.prefabSquare = (GameObject)Resources.Load("Prefabs/Square", typeof(GameObject));
                src_ev.prefabTriangle = (GameObject)Resources.Load("Prefabs/Triangle", typeof(GameObject));
                src_ev.prefabPentagon = (GameObject)Resources.Load("Prefabs/Pentagon", typeof(GameObject));
                src_ev.prefabDiamond = (GameObject)Resources.Load("Prefabs/Diamond", typeof(GameObject));
                src_ev.direction = 1;
                InvokeRepeating("evolutive", 1, (float)(1 / NEAT.game_speed));
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

            case "evolutive":
                brain_enemy = new GameObject("enemyBrain");
                brain_enemy.transform.SetParent(gameObject.transform.parent);
                evolutive_behaviour src_ev = brain_enemy.AddComponent<evolutive_behaviour>();
                src_ev.target = gameObject.transform.parent.transform.GetChild(0).gameObject;
                src_ev.myTower = gameObject.transform.parent.transform.GetChild(1).gameObject;
                src_ev.prefabSquare = (GameObject)Resources.Load("Prefabs/Square", typeof(GameObject));
                src_ev.prefabTriangle = (GameObject)Resources.Load("Prefabs/Triangle", typeof(GameObject));
                src_ev.prefabPentagon = (GameObject)Resources.Load("Prefabs/Pentagon", typeof(GameObject));
                src_ev.prefabDiamond = (GameObject)Resources.Load("Prefabs/Diamond", typeof(GameObject));
                src_ev.direction = -1;
                if(ally_controller != "evolutive")
                    InvokeRepeating("evolutive", 1, (float)(1 / NEAT.game_speed));
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
        brain_ally.GetComponent<Brain>().position = position;
        brain_enemy.GetComponent<Brain>().position = position;
        InvokeRepeating("secondUpdate", 1, (float)(1 / NEAT.game_speed));
    }

    void Update()
    {

    }

    void secondUpdate()
    {
        if(timer > time_limit)
        {
            towerBehabiour ta = gameObject.transform.parent.transform.GetChild(0).gameObject.GetComponent<towerBehabiour>();
            towerBehabiour te = gameObject.transform.parent.transform.GetChild(1).gameObject.GetComponent<towerBehabiour>();
            int diff = ta.health - te.health;
            endGame(diff);
        }

        Brain ally = brain_ally.GetComponent<Brain>();
        Brain enemy = brain_enemy.GetComponent<Brain>();
        ally.gold = ally.gold < 15 ? ally.gold + 1 : ally.gold;
        enemy.gold = enemy.gold < 15 ? enemy.gold + 1 : enemy.gold;
        timer++;
        gameObject.transform.parent.transform.GetChild(4).GetComponent<TextMesh>().text = ally.gold.ToString();
        gameObject.transform.parent.transform.GetChild(5).GetComponent<TextMesh>().text = enemy.gold.ToString();
        gameObject.GetComponent<TextMesh>().text = "Timer: " + timer.ToString() + " s";

        ally.log += "_";
        enemy.log += "_";
    }

    void evolutive()
    {

        Brain ally = brain_ally.GetComponent<evolutive_behaviour>();
        if(ally == null)
            ally = brain_ally.GetComponent<Brain>();
        Brain enemy = brain_enemy.GetComponent<evolutive_behaviour>();
        if (enemy == null)
            enemy = brain_ally.GetComponent<Brain>();

        float[] ally_input = new float[12];
        float[] enemy_input = new float[12];

        float[] ally_values = ally.getValues();
        float[] enemy_values = enemy.getValues();


        for (int i = 0; i < 6; i++)
        {
            ally_input[i] = ally_values[i];
            ally_input[6 + i] = enemy_values[i];

            enemy_input[i] = enemy_values[i];
            enemy_input[6 + i] = ally_values[i];
        }

        // Normalization
        if(ally != null)
        {
            ally_input[0] = Utils.normalize(ally_input[0], 0, 15);
            for (int i = 0; i < 4; i++)
                ally_input[i + 1] = Utils.normalize(ally_input[i + 1], 0, 5);
            ally_input[5] = Utils.normalize(ally_input[5], 0, 100);
            ally_input[6] = Utils.normalize(ally_input[0], 0, 15);
            for (int i = 0; i < 4; i++)
                ally_input[i + 7] = Utils.normalize(ally_input[i + 7], 0, 5);
            ally_input[11] = Utils.normalize(ally_input[11], 0, 100);
        }

        if (enemy != null)
        {
            enemy_input[0] = Utils.normalize(enemy_input[0], 0, 15);
            for (int i = 0; i < 4; i++)
                enemy_input[i + 1] = Utils.normalize(enemy_input[i + 1], 0, 5);
            enemy_input[5] = Utils.normalize(enemy_input[5], 0, 100);
            enemy_input[6] = Utils.normalize(enemy_input[0], 0, 15);
            for (int i = 0; i < 4; i++)
                enemy_input[i + 7] = Utils.normalize(enemy_input[i + 7], 0, 5);
            enemy_input[11] = Utils.normalize(enemy_input[11], 0, 100);
        }

        evolutive_behaviour a = null;
        evolutive_behaviour e = null;
        try {
            a =  (evolutive_behaviour)ally;
        } catch { a = null; }

        try{
            e = (evolutive_behaviour)enemy;
        }
        catch { e = null; }

        if (a != null)
        {
            a.act(enemy_input);
        }
        if (e != null)
        {
            e.act(enemy_input);
        }

    }

    public delegate void gameFinishedDelegate(Main p);
    public event gameFinishedDelegate gameFinishedEvent;
    
    public void endGame(int diff)
    {
        //Finish game, set winner etc...
        winner = diff >= 0 ? 1 : 0;
        this.diff = diff;

        ally_fitness = (int)Utils.Max(diff + 150 - timer * 0.5f, 0.0f);
        enemy_fitness = (int)Utils.Max(diff*-1 + 150 - timer * 0.5f, 0.0f);


        CancelInvoke("secondUpdate");
        if(ally_controller == "evolutive" || enemy_controller == "evolutive")
            CancelInvoke("evolutive");   

        Destroy(gameObject.transform.parent.gameObject, 1f);
        gameFinishedEvent(this);
    }
}