using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tournament : MonoBehaviour
{
    public static List<Individual> hall_of_fame = new List<Individual>();
    public static List<Individual> parasite_champions = new List<Individual>();
    public static List<Individual> host_champions = new List<Individual>();
    public List<Individual> participants = new List<Individual>();

    public int games_played = 0;
    public int target_games;

    public static bool playing = false;
    // Start is called before the first frame update
    void Start()
    {
        Individual.tour = this;
        //Play all other 
        participants.AddRange(host_champions);
        participants.AddRange(parasite_champions);

        List<Individual> hall = new List<Individual>();
        if (hall_of_fame.Count > 0) {
            for (int i = 0; i < Mathf.Min(hall_of_fame.Count, 8); i++)
            {
                hall.Add(Range.choice(hall_of_fame));
            }
        }

        playing = true;
        for (int i = 0; i < participants.Count; i++)
        {
            for (int j = i + 1; j < participants.Count; j++)
            { 
                StartCoroutine(participants[i].fight(participants[j]));
            }
        }

        foreach(Individual p in participants)
        {
            foreach (Individual h in hall)
            {
                StartCoroutine(p.fight(h));
            }
            StartCoroutine(p.fight());
        }
        
        

        target_games = ((participants.Count - 1) * participants.Count / 2);
        target_games += hall.Count*participants.Count; // vs hall
        target_games += participants.Count; // vs coded
    }

    // Update is called once per frame
    void Update()
    {
        if (Enviroment.empty_arena() && games_played >= target_games)
        {
            playing = false;
            // posar a null la especie del campió i tractar-ho en el main
            Individual champion = Range.choice(host_champions);
            foreach(Individual ind in participants)
            {
                if(champion.specie != null && ind.specie != null)
                {
                    if (ind.specie.tournament_wins > champion.specie.tournament_wins) {
                        champion = ind;
                    }
                }
            }

            Individual copy = (Individual)champion.Clone();
            copy.fitness = champion.specie.tournament_wins;
            hall_of_fame.Add(copy);
            champion.save("Individual-"+Generation.n_gen+"-"+champion.specie.tournament_wins+"_"+ (target_games/participants.Count) + "-"+champion.genome.Count+".json");
            

            if (Generation.n_gen < Enviroment.n_gens)
            {
                GameObject go = Instantiate((GameObject)Resources.Load("Prefabs/Generation"));
                go.name = "Generation " + Generation.n_gen;
                Generation.n_gen++;

                Generation.natural_selection();
            }
            else
            {
                // god.fes_algo
            }
            Destroy(this.gameObject);
        }
    }
}
