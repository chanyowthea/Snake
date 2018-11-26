using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomUtil : TSingleton<RandomUtil>
{
    System.Random _random = new System.Random();

    public int Next()
    {
        return _random.Next();
    }

    public int Next(int maxValue)
    {
        return _random.Next(maxValue);
    }

    public int Next(int minValue, int maxValue)
    {
        return _random.Next(minValue, maxValue);
    }
}
