using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform playerObject;
    [SerializeField]
    private float followSpeed;
    [SerializeField]
    private bool clampBool = true;
    [SerializeField]
    private GameObject currentRoom;
    [SerializeField]
    private float cameraSizeMultiplier = 1.0f;

    [SerializeField]
    private float maxDistance = 1.0f;
    [SerializeField]
    private float traumaRate = 0.005f;
    [SerializeField]
    private float shakeFrequency = 5.0f;

    private Vector3 targetPosition;
    private float originalCameraSize;
    private Vector3 cameraOffset;
    private float trauma = 0;
    private List<FocusObject> focusList = new List<FocusObject>();

    // Start is called before the first frame update
    void Start()
    {
        originalCameraSize = Camera.main.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        //Various Dev Tools
        if (Input.GetKey(KeyCode.Q))
        {
            followSpeed -= 0.1f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            followSpeed += 0.1f * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            CameraShake(0.4f);
        }
    }

    private void FixedUpdate()
    {
        //Set camera back to normal from camerashake before it applies calculations
        transform.position -= cameraOffset;

        //Handling camera size smoothly with a lerp
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, originalCameraSize * cameraSizeMultiplier, followSpeed);

        //Keeping camera target within constraints of room, with the size of the camera kept in mind.
        //May not function perfectly if camera bounds are bigger than room.
        float orthoSize = Camera.main.orthographicSize;

        //Finding target position of camera.
        //If there are no focus objects it will be the position of the player.
        //Otherwise, find player percentage distance between max focus radius and centre of focus. Then, lerp camera towards focus by that percentage.
        //If there are multiple foci, get all end positions and average between them.
        targetPosition = playerObject.position;
        Vector3 newTargetPosition = Vector3.zero;
        int focusCount = 0;
        foreach (FocusObject focus in focusList)
        {
            if (focus)
            {
                if (focus.parentRoom == currentRoom)
                {
                    if (focus.GetComponent<CircleCollider2D>())
                    {
                        newTargetPosition += Vector3.Lerp(playerObject.position, focus.transform.position, Mathf.InverseLerp(focus.GetComponent<CircleCollider2D>().radius, 0, Vector3.Distance(playerObject.position, focus.transform.position)));
                    }
                    else
                    {
                        newTargetPosition += Vector3.Lerp(playerObject.position, focus.transform.position, Mathf.InverseLerp(focus.GetComponent<BoxCollider2D>().size.magnitude, 0, Vector3.Distance(playerObject.position, focus.transform.position)));
                    }
                    focusCount++;
                }
            }
        }
        if(focusCount > 0)
        {
            targetPosition = newTargetPosition / focusCount;
        }

        Bounds roomBounds = currentRoom.GetComponent<BoxCollider2D>().bounds;

        //Upper and lower bounds checks
        if (targetPosition.y - orthoSize < roomBounds.center.y - roomBounds.extents.y)
        {
            targetPosition = new Vector3(targetPosition.x, roomBounds.center.y - roomBounds.extents.y + orthoSize, -10.0f);
        }
        else if(targetPosition.y + orthoSize > roomBounds.center.y + roomBounds.extents.y)
        {
            targetPosition = new Vector3(targetPosition.x, roomBounds.center.y + roomBounds.extents.y - orthoSize, -10.0f);
        }

        //Horizontal bounds checks
        //If the camera bounds are outside of the room bounds, move the camera target back into the room + the difference between current and target camera size so it doesn't "dip out" of the room while sizing upwards.
        if (targetPosition.x - orthoSize * Screen.width / Screen.height < roomBounds.center.x - roomBounds.extents.x)
        {
            targetPosition = new Vector3(roomBounds.center.x - roomBounds.extents.x + orthoSize * Screen.width / Screen.height + Mathf.Clamp((originalCameraSize * cameraSizeMultiplier - orthoSize) * 2, -3.0f, 50.0f), targetPosition.y, -10.0f);
            
        }
        else if (targetPosition.x + orthoSize * Screen.width / Screen.height > roomBounds.center.x + roomBounds.extents.x)
        {
            targetPosition = new Vector3(roomBounds.center.x + roomBounds.extents.x - orthoSize * Screen.width / Screen.height - Mathf.Clamp((originalCameraSize * cameraSizeMultiplier - orthoSize) * 2, -3.0f, 50.0f), targetPosition.y, -10.0f);
        }

        //Camera follows target position smoothly with a lerp
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10.0f);

        CameraShake(0.0f);
    }

    public void SwitchRooms(GameObject newRoom)
    {
        currentRoom = newRoom;
        cameraSizeMultiplier = currentRoom.GetComponent<RoomScript>().cameraMultiplier;
    }

    public void AddFocus(FocusObject newFocus)
    {
        focusList.Add(newFocus);
    }

    public void RemoveFocus(FocusObject scrappedFocus)
    {
        focusList.Remove(scrappedFocus);
    }

    public void CameraShake(float hitValue)
    {
        trauma += hitValue;
        trauma = Mathf.Clamp(trauma, 0.0f, 1.0f);

        float shake = trauma * trauma;
        if(trauma <= 0)
        {
            cameraOffset = Vector2.zero;
        }
        else
        {
            cameraOffset = new Vector2(maxDistance * cameraSizeMultiplier * shake * (Mathf.PerlinNoise(Time.realtimeSinceStartup * shakeFrequency, Time.realtimeSinceStartup * shakeFrequency) * 2 - 1),
                                       maxDistance * cameraSizeMultiplier * shake * (Mathf.PerlinNoise(Time.realtimeSinceStartup * shakeFrequency, Time.realtimeSinceStartup * shakeFrequency + 10) * 2 - 1));
        }
        transform.position += cameraOffset;
        
        trauma -= traumaRate;
        trauma = Mathf.Clamp(trauma, 0.0f, 1.0f);
    }

    public void FollowSpeedChange(Slider sliderValue)
    {
        followSpeed = sliderValue.value;
    }
}
