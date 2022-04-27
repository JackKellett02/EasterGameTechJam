using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusObject : MonoBehaviour
{
    public GameObject parentRoom;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Camera.main.GetComponent<CameraController>().AddFocus(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Camera.main.GetComponent<CameraController>().RemoveFocus(this);
        }
    }

    private void OnDisable()
    {
        if (Camera.main)
        {
            Camera.main.GetComponent<CameraController>().RemoveFocus(this);
        }
    }

    private void OnDrawGizmos()
    {
        if (GetComponent<CircleCollider2D>())
        {
            Gizmos.DrawWireSphere(transform.position, GetComponent<CircleCollider2D>().radius);
        }
        else
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y, 0));
        }
    }
}
