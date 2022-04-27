using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaytestPickup : MonoBehaviour
{
    [SerializeField]
    private GameObject particleEffect;
    [SerializeField]
    private bool movingParticle = false;
    [SerializeField]
    private Vector3 targetVector = Vector3.zero;
    [SerializeField]
    private Vector3 targetVector2 = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        targetVector = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (movingParticle)
        {
            transform.position = Vector3.Lerp(transform.position, targetVector, Time.deltaTime);
            if(Vector3.Distance(transform.position,targetVector) < 1.0f)
            {
                Vector3 tempVector = targetVector;
                targetVector = targetVector2;
                targetVector2 = tempVector;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject particle = Instantiate(particleEffect);
        particle.transform.position = transform.position;
        gameObject.SetActive(false);
    }
}
