using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [SerializeField]
    int currentPlanet = 0;

    [SerializeField]
    SpringDynamics bulbHolder;
    [SerializeField]
    SpringDynamics starterPlanet;
    [SerializeField]
    SpringDynamics bulbOne;
    [SerializeField]
    SpringDynamics bulbTwo;
    [SerializeField]
    SpringDynamics bulbThree;

    [SerializeField]
    GameObject stopInput;

    public bool bulbOneUnlocked = false;
    public bool bulbTwoUnlocked = false;
    public bool bulbThreeUnlocked = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveLeft()
    {
        if(currentPlanet != 0)
        {
            bulbHolder.AddOffset(new Vector2(890.0f, 0.0f));
            
            if(currentPlanet == 1)
            {
                starterPlanet.SwitchSize();
                bulbOne.SwitchSize();
            }
            else if(currentPlanet == 2)
            {
                bulbTwo.SwitchSize();
                bulbOne.SwitchSize();
            }
            else if(currentPlanet == 3)
            {
                bulbThree.SwitchSize();
                bulbTwo.SwitchSize();
            }

            currentPlanet -= 1;
            GameObject.FindGameObjectWithTag("LevelStorage").GetComponent<ProgressStorage>().SetBulb(currentPlanet);
        }
    }

    public void MoveRight()
    {
        if(currentPlanet != 3)
        {
            bool pathUnlocked = false;

            if (currentPlanet == 0 && bulbOneUnlocked)
            {
                starterPlanet.SwitchSize();
                bulbOne.SwitchSize();
                pathUnlocked = true;
            }
            else if (currentPlanet == 1 && bulbTwoUnlocked)
            {
                bulbTwo.SwitchSize();
                bulbOne.SwitchSize();
                pathUnlocked = true;
            }
            else if (currentPlanet == 2 && bulbThreeUnlocked)
            {
                bulbThree.SwitchSize();
                bulbTwo.SwitchSize();
                pathUnlocked = true;
            }

            if (!pathUnlocked) { return; }

            bulbHolder.AddOffset(new Vector2(-890.0f, 0.0f));
            currentPlanet += 1;
            GameObject.FindGameObjectWithTag("LevelStorage").GetComponent<ProgressStorage>().SetBulb(currentPlanet);
        }
    }

    public void LoadScene(int sparkNumber)
    {
        stopInput.SetActive(true);
        GameObject.FindGameObjectWithTag("LevelStorage").GetComponent<ProgressStorage>().SetLevel(0);
        StartCoroutine(SlideWaiter(sparkNumber));
    }

    IEnumerator SlideWaiter(int sparkNumber)
    {
        GameObject.FindGameObjectWithTag("CircleWipe").GetComponent<CircleWipe>().WipeIn(1);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sparkNumber);
    }
}
