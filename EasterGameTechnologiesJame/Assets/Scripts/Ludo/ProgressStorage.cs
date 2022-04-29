using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressStorage : MonoBehaviour
{
    static public bool[] levelProgress = new bool[16];
    static public int bulbNumber = 0;

    public void SetLevel(int levelNumber)
    {
        levelProgress[levelNumber] = true;
    }

    public bool[] GetLevel()
    {
        return levelProgress;
    }

    public void SetBulb(int bulb)
    {
        bulbNumber = bulb;
    }

    public int GetBulb()
    {
        return bulbNumber;
    }
}
