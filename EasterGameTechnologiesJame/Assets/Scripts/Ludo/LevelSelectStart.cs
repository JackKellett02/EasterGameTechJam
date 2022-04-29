using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectStart : MonoBehaviour
{

    [SerializeField]
    GameObject[] levelSparks = new GameObject[16];

    [SerializeField]
    GameObject starterPlanet;
    [SerializeField]
    GameObject bulbOne;
    [SerializeField]
    GameObject bulbTwo;
    [SerializeField]
    GameObject bulbThree;

    // Start is called before the first frame update
    void Start()
    {
        bool[] levelArray = GameObject.FindGameObjectWithTag("LevelStorage").GetComponent<ProgressStorage>().GetLevel();
        int levelsComplete = 0;

        for(int i = 0; i < 16; i++)
        {
            if (levelArray[i])
            {
                levelsComplete++;
                levelSparks[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }

        if(levelsComplete >= 1)
        {
            GetComponent<MenuScript>().bulbOneUnlocked = true;
            starterPlanet.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        bulbOne.GetComponent<Image>().color = Color32.Lerp(new Color32(102,102,102, 255), new Color32(255, 255, 255, 255), (levelsComplete - 1) / 5.0f);
        if((levelsComplete - 1) / 5.0f >= 1)
        {
            GetComponent<MenuScript>().bulbTwoUnlocked = true;
        }
        bulbTwo.GetComponent<Image>().color = Color32.Lerp(new Color32(102, 102, 102, 255), new Color32(255, 255, 255, 255), (levelsComplete - 6) / 5.0f);
        if ((levelsComplete - 6) / 5.0f >= 1)
        {
            GetComponent<MenuScript>().bulbThreeUnlocked = true;
        }
        bulbThree.GetComponent<Image>().color = Color32.Lerp(new Color32(102, 102, 102, 255), new Color32(255, 255, 255, 255), (levelsComplete - 11) / 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
