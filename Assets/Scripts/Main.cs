using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

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
            src.controller = this;
            sq.GetComponent<SpriteRenderer>().color = src.direction == 1 ? Color.green : Color.red;
            sq.transform.SetParent(gameObject.transform.parent);
            src.tower = target;
            src.direction = direction;
            string s = src.direction == 1 ? "Ally" : "Enemy";
            sq.tag = s;
            sq.layer = src.direction == 1 ? 8 : 9;
            sq.GetComponent<SpriteRenderer>().color = src.direction == 1 ? Color.green : Color.red;
            sq.name = src.direction == 1 ? "Ally" : "Enemy";
            src.unit = "s";

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
            src.controller = this;
            sq.transform.SetParent(gameObject.transform.parent);
            src.tower = target;
            src.direction = direction;
            string s = src.direction == 1 ? "Ally" : "Enemy";
            sq.tag = s;
            sq.layer = src.direction == 1 ? 8 : 9;
            sq.GetComponent<SpriteRenderer>().color = src.direction == 1 ? Color.green : Color.red;
            sq.name = src.direction == 1 ? "Ally" : "Enemy";
            units[1] += 1;
            src.unit = "d";
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
            src.controller = this;
            sq.transform.SetParent(gameObject.transform.parent);
            src.tower = target;
            src.direction = direction;
            string s = src.direction == 1 ? "Ally" : "Enemy";
            sq.tag = s;
            sq.layer = src.direction == 1 ? 8 : 9;
            sq.GetComponent<SpriteRenderer>().color = src.direction == 1 ? Color.green : Color.red;
            sq.name = src.direction == 1 ? "Ally" : "Enemy";
            units[2] += 1;
            src.unit = "t";
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
            src.controller = this;
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
            src.unit = "p";
            return true;
        }
        return false;
    }
    protected bool explode() {
        if (gold >= 7) {
            unitBehabiour[] children = transform.parent.GetComponentsInChildren<unitBehabiour>();
            for (int i = 0; i < children.Length; i++) {
                children[i].explode();
            }
            gold -= 7;
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
    int act(float[] inputs = null);

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

    public int ally_violations = 0;
    public int enemy_violations = 0;

    public int speed;

    public int index;

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
                src_ev.ind = ally_ind;
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
                Enviroment.speed = this.speed;
                brain_ally = new GameObject("allyBrain");
                brain_ally.transform.SetParent(gameObject.transform.parent);
                src_ev = brain_ally.AddComponent<evolutive_behaviour>();
                src_ev.ind = new Individual(12, 5);
                src_ev.target = gameObject.transform.parent.transform.GetChild(1).gameObject;
                src_ev.myTower = gameObject.transform.parent.transform.GetChild(0).gameObject;
                src_ev.prefabSquare = (GameObject)Resources.Load("Prefabs/Square", typeof(GameObject));
                src_ev.prefabTriangle = (GameObject)Resources.Load("Prefabs/Triangle", typeof(GameObject));
                src_ev.prefabPentagon = (GameObject)Resources.Load("Prefabs/Pentagon", typeof(GameObject));
                src_ev.prefabDiamond = (GameObject)Resources.Load("Prefabs/Diamond", typeof(GameObject));
                src_ev.direction = 1;
                InvokeRepeating("evolutive", 1, (float)(1 / NEAT.game_speed));                
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
                
            case "evaluator":
                brain_enemy = new GameObject("enemyBrain");
                brain_enemy.transform.SetParent(gameObject.transform.parent);
                src_cod = brain_enemy.AddComponent<coded_behaviour>();
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
                src_ev.ind = enemy_ind;                
                src_ev.target = gameObject.transform.parent.transform.GetChild(0).gameObject;
                src_ev.myTower = gameObject.transform.parent.transform.GetChild(1).gameObject;
                src_ev.prefabSquare = (GameObject)Resources.Load("Prefabs/Square", typeof(GameObject));
                src_ev.prefabTriangle = (GameObject)Resources.Load("Prefabs/Triangle", typeof(GameObject));
                src_ev.prefabPentagon = (GameObject)Resources.Load("Prefabs/Pentagon", typeof(GameObject));
                src_ev.prefabDiamond = (GameObject)Resources.Load("Prefabs/Diamond", typeof(GameObject));
                src_ev.direction = -1;
                if (ally_controller != "evolutive")
                    InvokeRepeating("evolutive", 1, (float)(1 / NEAT.game_speed));
                break;

            default:
                Enviroment.speed = this.speed;
                brain_enemy = new GameObject("enemyBrain");
                brain_enemy.transform.SetParent(gameObject.transform.parent);
                src_ev = brain_enemy.AddComponent<evolutive_behaviour>();
                src_ev.ind = IndividualFactory.buildFromFile("Champions/"+ enemy_controller);
                src_ev.target = gameObject.transform.parent.transform.GetChild(0).gameObject;
                src_ev.myTower = gameObject.transform.parent.transform.GetChild(1).gameObject;
                src_ev.prefabSquare = (GameObject)Resources.Load("Prefabs/Square", typeof(GameObject));
                src_ev.prefabTriangle = (GameObject)Resources.Load("Prefabs/Triangle", typeof(GameObject));
                src_ev.prefabPentagon = (GameObject)Resources.Load("Prefabs/Pentagon", typeof(GameObject));
                src_ev.prefabDiamond = (GameObject)Resources.Load("Prefabs/Diamond", typeof(GameObject));
                src_ev.direction = -1;
                if (ally_controller != "evolutive")
                    InvokeRepeating("evolutive", 1, (float)(1 / NEAT.game_speed));
                break;
        }
        brain_ally.GetComponent<Brain>().position = position;
        brain_enemy.GetComponent<Brain>().position = position;
        InvokeRepeating("secondUpdate", 1, (float)(1 / NEAT.game_speed));
    }

    void secondUpdate()
    {
        if(timer > time_limit)
        {
            towerBehabiour ta = gameObject.transform.parent.transform.GetChild(0).gameObject.GetComponent<towerBehabiour>();
            towerBehabiour te = gameObject.transform.parent.transform.GetChild(1).gameObject.GetComponent<towerBehabiour>();
            int diff = (ta.health - te.health);
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

        if (timer > 15) {
            if(ally_input[0] == 1 && enemy_input[0] == 1)
            {
                List<int> r = ally_ind.log
                .GroupBy(c => c)
                .Select(group => group.Count())
                .ToList();
                List<int> l = new List<int>();
                if (enemy_controller == "evolutive") {
                    l = enemy_ind.log
                    .GroupBy(c => c)
                    .Select(group => group.Count())
                    .ToList();
                }
                

                if(r.Count == l.Count)
                {
                    endGame(0);
                }
            }
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
            ally_violations += a.act(ally_input);
        }
        if (e != null)
        {
            enemy_violations += e.act(enemy_input);
        }
        /*
        if (a != null)
        {
            ally_violations += a.act(new float[] { ally_input[0], ally_input[1], ally_input[2], ally_input[3], ally_input[4], ally_input[6], ally_input[7], ally_input[8], ally_input[9], ally_input[10] });
        }
        if (e != null)
        {
            enemy_violations += e.act(new float[] { enemy_input[0], enemy_input[1], enemy_input[2], enemy_input[3], enemy_input[4], enemy_input[6], enemy_input[7], enemy_input[8], enemy_input[9], enemy_input[10] });
        }
        */
    }
    
    public void endGame(int diff)
    {
        this.diff = diff;

        winner = diff > 0 ? -1 : 1;  // -1 = ally, 1 = enemy
        winner = diff == 0 ? 0 : winner;
        
        int penalty(int x)
        {
            if(!NEAT.penalty)
                return 0;
            float var;
            if (Generation.n_gen < 25.0f)
            {
                var = (400 / (1 + Mathf.Pow((float)Math.E, -(1.0f / (25.0f - (float)Generation.n_gen)) * (float)x))) - 200;
            }
            else
            {
                var = 5 * x;
            }
            return (int)var;
        }

        if(enemy_controller == "evaluator")
        {
            Evaluator.fitness_progress[index] = 100 + Mathf.Abs(diff) - penalty(enemy_violations);
            CancelInvoke("secondUpdate");
            if (ally_controller == "evolutive" || enemy_controller == "evolutive")
                CancelInvoke("evolutive");

            Enviroment.arena[coords[0], coords[1]] = 0;
            //gameFinishedEvent(this);
            Destroy(gameObject.transform.parent.gameObject, 1f);
            return;
        }
        
        
        if (!Tournament.playing)
        {
            // Generation games
            // Dynamic penalty
            ally_ind.violations = ally_violations;
            
            if (winner == -1)
            {
                // Ally wins
                List<int> r = ally_ind.log
                .GroupBy(c => c)
                .Select(group => group.Count())
                .ToList();
                ally_ind.wins += 1;
                int rwd = (int)(100+Mathf.Abs(diff) - penalty(enemy_violations) + 10 * (r.Count > 2 ? r.Count : 0));

                if (enemy_controller == "coded")
                    rwd += 100;
                ally_ind.fitness += Math.Max(rwd, 0);
            }
            else if (winner == 1)
            {
                if (enemy_controller == "evolutive")
                {
                    // Enemy wins
                    List<int> r = enemy_ind.log
                    .GroupBy(c => c)
                    .Select(group => group.Count())
                    .ToList();
                    enemy_ind.wins += 1;
                    enemy_ind.violations = enemy_violations;
                    int rwd = (int)(100 + Mathf.Abs(diff) - penalty(enemy_violations) + 10 * (r.Count > 2 ? r.Count : 0));
                    enemy_ind.fitness += Math.Max(rwd, 0);
                }
                   
            }
            else
            {
                // Tie

                List<int> r = ally_ind.log
                .GroupBy(c => c)
                .Select(group => group.Count())
                .ToList();
                ally_ind.fitness += Math.Max(50- penalty(enemy_violations) + 20 * (r.Count > 2 ? r.Count:0), 0) ;
                if (enemy_controller == "evolutive")
                {
                    r = enemy_ind.log
                    .GroupBy(c => c)
                    .Select(group => group.Count())
                    .ToList();
                    enemy_ind.violations = enemy_violations;
                    enemy_ind.fitness += Math.Max(50 - penalty(enemy_violations) + 20 * (r.Count > 2 ? r.Count : 0), 0);
                }
                    
            }
            
            Individual.gen.games_played++;
        }
        else
        {
            // Tournament games  

            if (winner == -1)
            {
                if (ally_ind.specie != null)
                {
                    List<int> r = ally_ind.log
                    .GroupBy(c => c)
                    .Select(group => group.Count())
                    .ToList();
                    // Ally wins
                    ally_ind.violations = ally_violations;
                    int rwd = (int)(100 + Mathf.Abs(diff) - penalty(enemy_violations) + 10 * (r.Count > 2 ? r.Count : 0));
                    if (enemy_controller == "coded")
                        rwd += 100;
                    ally_ind.specie.tournament_wins += Math.Max(rwd, 0);
                }
            }
            else if (winner == 1)
            {
                if (enemy_controller == "evolutive") {
                    if (enemy_ind.specie != null)
                    {
                        List<int> r = enemy_ind.log
                        .GroupBy(c => c)
                        .Select(group => group.Count())
                        .ToList();
                        // Enemy wins
                        enemy_ind.violations = enemy_violations;
                        int rwd = (int)(100 + Mathf.Abs(diff) - penalty(enemy_violations) + 10 * (r.Count > 2 ? r.Count : 0));
                        enemy_ind.specie.tournament_wins += Math.Max(rwd, 0);
                    } 
                }
            }
            else
            {
                // Tie
                List<int> r = ally_ind.log
                   .GroupBy(c => c)
                   .Select(group => group.Count())
                   .ToList();
                if (ally_ind.specie != null)
                    ally_ind.specie.tournament_wins += Math.Max(50 - penalty(enemy_violations) + 10 * (r.Count > 2 ? r.Count : 0), 0);

                if (enemy_controller == "evolutive")
                {
                    r = enemy_ind.log
                    .GroupBy(c => c)
                    .Select(group => group.Count())
                    .ToList();
                    if (enemy_ind.specie != null)
                        enemy_ind.specie.tournament_wins += Math.Max(50 - penalty(enemy_violations) + 20 * (r.Count > 2 ? r.Count : 0), 0);
                }
            }

            Individual.tour.games_played++;
        }
        
        if (Tournament.playing)
            Individual.tour.games_played++;
        else
            Individual.gen.games_played++;

        CancelInvoke("secondUpdate");
        if(ally_controller == "evolutive" || enemy_controller == "evolutive")
            CancelInvoke("evolutive");

        Enviroment.arena[coords[0], coords[1]] = 0;
        //gameFinishedEvent(this);
        Destroy(gameObject.transform.parent.gameObject, 1f);
    }
}