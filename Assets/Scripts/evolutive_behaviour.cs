using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class evolutive_behaviour : Brain, IBehabiour
    {

        Individual ind = new Individual(12, 5);

        // Use this for initialization
        private void Start()
        {
            calcSpawn();
            for (int i = 0; i < 20; i++)
                ind.force_mutate();

        }
        public new void act(float[] inputs = null)
        {
            float[] output = ind.process(inputs);

            int action = Utils.Max_index(output);

            switch (action)
            {
                case 0:
                    break;
                case 1:
                    spawnSquare();
                    break;
                case 2:
                    spawnDiamond();
                    break;
                case 3:
                    spawnTriangle();
                    break;
                case 4:
                    spawnPentagon();
                    break;
            }
        }

    }
}