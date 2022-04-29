using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleWipe : MonoBehaviour
{
    float wipeTimer = 0.0f;
    float startTime = 1.0f;
    float wipeSize = 2300;
    float wipeDirection = 1;
    
    [SerializeField]
    bool changeDirection = false;
    [SerializeField]
    bool wiping = false;
    [SerializeField]
    float wipeHoldTime = 0.5f;
    
    RectTransform wipeRect;

    private void Start()
    {
        wipeRect = GetComponent<RectTransform>();
    }

    public void WipeIn(float wipeTime)
    {
        wipeTimer = 0;
        startTime = wipeTime;
        wiping = true;
        wipeDirection = 1.0f;
    }

    public void WipeOut(float wipeTime)
    {
        wipeTimer = 0;
        startTime = wipeTime;
        wiping = true;
        wipeDirection = -1.0f;
    }

    public void WipeInAndOut(float wipeTime)
    {
        wipeTimer = 0;
        startTime = wipeTime;
        wiping = true;
        wipeDirection = 1.0f;
        changeDirection = true;
    }

    private void Update()
    {
        if (wiping)
        {
            wipeTimer += Time.deltaTime * wipeDirection;
            wipeSize = Mathf.Lerp(2300, 0, wipeTimer / startTime);
            wipeRect.sizeDelta = new Vector2(wipeSize, wipeSize);

            if(wipeTimer >= startTime + wipeHoldTime || wipeTimer <= 0 - wipeHoldTime)
            {
                if (changeDirection)
                {
                    wipeDirection *= -1;
                    changeDirection = false;
                }
                else
                {
                    wiping = false;
                }
            }
        }
    }
}
