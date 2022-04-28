using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformer : MonoBehaviour
{
    float inputX;
    bool grounded;
    bool gliding;
    float glidingHorizontal;
    float jumpTimer;
    Vector2 previousVelocity = Vector2.zero;
    Rigidbody2D RB;

    [SerializeField]
    float jumpBufferValue = 0.15f;
    [SerializeField]
    float speed = 5;
    [SerializeField]
    float jumpHeight = 5;
    [SerializeField]
    float squashStretchCutoff = 0.5f;
    [SerializeField]
    float squashStretchValue = 0.6f;
    [SerializeField]
    Transform playerSprite = null;
    [SerializeField]
    float fallShakeMultiplier = 0.025f;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        //Handling Jump Input Buffer values
        jumpTimer -= Time.deltaTime;
        if (Input.GetButtonDown("Jump"))
        {
            jumpTimer = jumpBufferValue;
        }

        if (Input.GetButton("Fire3") && RB.velocity.y < 0.5f)
        {
            gliding = true;
        }
        else
        {
            gliding = false;
        }
        
    }

    private void FixedUpdate()
    {
        
        //Setting player horizontal velocity, only add up to max speed value, but allow player to keep faster velocities
        float maxSpeed = speed * Time.deltaTime;
        if (grounded)
        {
            RB.velocity -= RB.velocity * new Vector2(0.9f, 0);
        }
        else if (gliding)
        {
            RB.velocity = new Vector2(RB.velocity.x, RB.velocity.y * 0.8f);
        }
        //If Player moves left
        if (inputX < 0)
        {
            //If they're moving faster than max left, slow back down a little
            //Otherwise, move max left
            if(RB.velocity.x < -maxSpeed)
            {
                RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, -maxSpeed, 0.02f), RB.velocity.y);
            }
            else
            {
                RB.velocity += new Vector2(-maxSpeed - RB.velocity.x, 0);
            }
        }
        //If Player moves right
        else if (inputX > 0)
        {
            //If they're moving faster than max right, slow back down a little
            //Otherwise, move max right
            if (RB.velocity.x > maxSpeed)
            {
                RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, maxSpeed, 0.02f), RB.velocity.y);
            }
            else
            {
                RB.velocity += new Vector2(maxSpeed - RB.velocity.x, 0);
            }
        }
        else
        {
            if (RB.velocity.x > maxSpeed)
            {
                RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, maxSpeed, 0.7f), RB.velocity.y);
            }
            else if (RB.velocity.x < -maxSpeed)
            {
                RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, -maxSpeed, 0.7f), RB.velocity.y);
            }
            else
            {
                RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, 0, 0.2f), RB.velocity.y);
            }
            
        }

        //If there's a jump stored in the jump buffer and the player is grounded, set upwards velocity to jump height.
        if (jumpTimer > 0.0f && grounded == true)
        {
            RB.velocity = new Vector2(RB.velocity.x, jumpHeight);
            jumpTimer = 0.0f;
        }

        //Change the player sprite's X size based on Y velocity.
        playerSprite.localScale = new Vector3(Mathf.Lerp(1.0f, squashStretchValue, Mathf.Abs(RB.velocity.y * 0.1f) - squashStretchCutoff), 1, 1);

        //Set 'previous velocity' of frame to be used in camera shake next frame
        previousVelocity = RB.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            grounded = true;
            if(previousVelocity.magnitude * fallShakeMultiplier > 0.4f)
            {
                Camera.main.GetComponent<CameraController>().CameraShake(previousVelocity.magnitude * fallShakeMultiplier);
            }

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = false;
        }
    }
}
