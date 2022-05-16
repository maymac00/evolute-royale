using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class evolutive_behaviour : Brain, IBehabiour
    {

        public Individual ind;

        // Use this for initialization
        private void Start()
        {
            calcSpawn();
        }
        public int act(float[] inputs = null)
        {
            float[] output = ind.process(inputs);

            int action = Utils.Max_index(output);

            switch (action)
            {
                case 0:
                    ind.log += "_";
                    return 0;
                case 1:
                    bool b = spawnSquare();
                    ind.log += "s";
                    if (!b)
                        return 1;
                    else
                        return 0;
                case 2:
                    b = spawnDiamond();
                    ind.log += "d";
                    if (!b)
                        return 1;
                    else
                        return 0;
                case 3:
                    b = spawnTriangle();
                    ind.log += "t";
                    if (!b)
                        return 1;
                    else
                        return 0;
                case 4:
                    b = spawnPentagon();
                    ind.log += "p";
                    if (!b)
                        return 1;
                    else
                        return 0;
                case 5:
                    b = explode();
                    ind.log += "e";
                    if (!b)
                        return 1;
                    else
                        return 0;

                default:
                    return 0;
            }
        }

    }
}