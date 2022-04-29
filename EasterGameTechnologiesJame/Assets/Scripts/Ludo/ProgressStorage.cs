using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressStorage : MonoBehaviour
{
    static public bool[] levelProgress = new bool[16];

    public void SetLevel(int levelNumber)
    {
        levelProgress[levelNumber] = true;
    }

    public bool[] GetLevel()
    {
        return levelProgress;
    }
}
