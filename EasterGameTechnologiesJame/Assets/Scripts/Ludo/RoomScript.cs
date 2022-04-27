using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    [Range(0.1f,5.0f)]
    public float cameraMultiplier = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Camera.main.GetComponent<CameraController>().SwitchRooms(this.gameObject);
        }
    }
}
