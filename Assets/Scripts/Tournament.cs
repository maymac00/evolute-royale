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
        //Play all other champions
        //TODO: play vs hardcoded behaviour
        participants.AddRange(host_champions);
        participants.AddRange(parasite_champions);

        if (hall_of_fame.Count > 0) {
            for (int i = 0; i < Mathf.Min(hall_of_fame.Count, 8); i++)
            {
                participants.Add(Range.choice(hall_of_fame));
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

        target_games = ((participants.Count - 1) * participants.Count / 2);
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

            int n = (participants.Count-1);
            hall_of_fame.Add((Individual)champion.Clone());
            champion.save("Individual-"+Generation.n_gen+"-"+champion.specie.tournament_wins+"_"+n+"-"+champion.genome.Count+".json");
            

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
