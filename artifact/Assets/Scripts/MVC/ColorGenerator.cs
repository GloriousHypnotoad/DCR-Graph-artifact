using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{
    public Dictionary<string, UnityEngine.Color> GenerateColors(HashSet<string> activities)
    {
        Dictionary<string, UnityEngine.Color> colorMap = new Dictionary<string, UnityEngine.Color>();
        int counter = UnityEngine.Random.Range(0, 4);

        foreach (var activity in activities)
        {
            UnityEngine.Color color;
            switch (counter)
            {
                case 0:
                    color = new UnityEngine.Color(0, 1, RandomRange(0.5f, 1));
                    break;
                case 1:
                    color = new UnityEngine.Color(0, RandomRange(0, 0.5f), 1);
                    break;
                case 2:
                    color = new UnityEngine.Color(0, RandomRange(0.5f, 1), 1);
                    break;
                case 3:
                    color = new UnityEngine.Color(RandomRange(0, 0.5f), 0, 1);
                    break;
                default:
                    throw new InvalidOperationException("Invalid counter state");
            }

            colorMap.Add(activity, color);
            counter = (counter + 1) % 4;
        }

        return colorMap;
    }

    private float RandomRange(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
}
