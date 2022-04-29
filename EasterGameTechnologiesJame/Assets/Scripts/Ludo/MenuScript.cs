using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            currentPlanet -= 1;
            
            if(currentPlanet == 0)
            {
                
            }
            else if(currentPlanet == 1)
            {
                bulbOne.SwitchSize();
                bulbTwo.SwitchSize();
            }
            else if(currentPlanet == 2)
            {
                bulbThree.SwitchSize();
                bulbTwo.SwitchSize();
            }
        }
    }

    public void MoveRight()
    {
        if(currentPlanet != 3)
        {
            bulbHolder.AddOffset(new Vector2(-890.0f, 0.0f));
            currentPlanet += 1;
        }
    }
}
