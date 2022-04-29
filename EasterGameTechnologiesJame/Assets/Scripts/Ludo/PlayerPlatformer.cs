using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.IK;
using UnityEngine;

public class PlayerPlatformer : MonoBehaviour {
	float inputX;
	bool grounded;
	bool gliding;
	bool canInput = true;
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
	[SerializeField]
	float airBrakeMultiplier = 1.0f;

	// Start is called before the first frame update
	void Start() {
		RB = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update() {
		//Handling Jump Input Buffer timer
		jumpTimer -= Time.deltaTime;

		if (canInput) {
			inputX = Input.GetAxisRaw("Horizontal");

			if (Input.GetButtonDown("Jump")) {
				jumpTimer = jumpBufferValue;
			}

			if (Input.GetButton("Fire3") && RB.velocity.y < 0.5f) {
				gliding = true;
			} else {
				gliding = false;
			}
		} else {
			inputX = 0;
			gliding = false;
		}

		HandleAnimationTransitions();
	}

	private void FixedUpdate() {

		//Setting player horizontal velocity, only add up to max speed value, but allow player to keep faster velocities
		float maxSpeed = speed * Time.deltaTime;
		if (grounded) {
			RB.velocity -= RB.velocity * new Vector2(0.9f, 0);
		} else if (gliding) {
			RB.velocity = new Vector2(RB.velocity.x, RB.velocity.y * 0.8f);
		}
		//If Player moves left
		if (inputX < 0) {
			//If they're moving faster than max left, slow back down a little
			//Otherwise, move max left
			if (RB.velocity.x < -maxSpeed) {
				RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, -maxSpeed, 0.02f), RB.velocity.y);
			} else {
				RB.velocity += new Vector2(-maxSpeed - RB.velocity.x, 0);
			}
		}
		//If Player moves right
		else if (inputX > 0) {
			//If they're moving faster than max right, slow back down a little
			//Otherwise, move max right
			if (RB.velocity.x > maxSpeed) {
				RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, maxSpeed, 0.02f), RB.velocity.y);
			} else {
				RB.velocity += new Vector2(maxSpeed - RB.velocity.x, 0);
			}
		} else {
			if (RB.velocity.x > maxSpeed) {
				RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, maxSpeed, 0.7f * airBrakeMultiplier), RB.velocity.y);
			} else if (RB.velocity.x < -maxSpeed) {
				RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, -maxSpeed, 0.7f * airBrakeMultiplier), RB.velocity.y);
			} else {
				RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, 0, 0.2f * airBrakeMultiplier), RB.velocity.y);
			}

		}

		//If there's a jump stored in the jump buffer and the player is grounded, set upwards velocity to jump height.
		if (jumpTimer > 0.0f && grounded == true) {
			RB.velocity = new Vector2(RB.velocity.x, jumpHeight);
			jumpTimer = 0.0f;
		}

		//Change the player sprite's X size based on Y velocity.
		playerSprite.localScale = new Vector3(Mathf.Lerp(1.0f, squashStretchValue, Mathf.Abs(RB.velocity.y * 0.1f) - squashStretchCutoff), 1, 1);

		//Set 'previous velocity' of frame to be used in camera shake next frame
		previousVelocity = RB.velocity;
	}

	public void PlayerDeathToggleOn(Vector2 deathDirection) {
		airBrakeMultiplier = 0.0f;
		GetComponent<BoxCollider2D>().enabled = false;
		RB.gravityScale = 0;
		RB.drag = 4.0f;
		RB.velocity = deathDirection;
		canInput = false;
	}

	public void PlayerDeathToggleOff() {
		airBrakeMultiplier = 1.0f;
		GetComponent<BoxCollider2D>().enabled = true;
		RB.gravityScale = 1.5f;
		RB.drag = 0.0f;
		RB.velocity = Vector2.zero;
		canInput = true;
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.tag == "Ground") {
			grounded = true;
			if (previousVelocity.magnitude * fallShakeMultiplier > 0.4f) {
				Camera.main.GetComponent<CameraController>().CameraShake(previousVelocity.magnitude * fallShakeMultiplier);
			}

		}
	}

	private void OnCollisionExit2D(Collision2D collision) {
		if (collision.gameObject.tag == "Ground") {
			grounded = false;
		}
	}

	#region Animation Code.
	//Animation Variables.
	[SerializeField]
	private Animator Animator;

	private float lastInput = 0.0f;

	private void HandleAnimationTransitions() {
		//Handle sprite rotation
		if (inputX < 0) {
			//Rotate the sprite left.
			Animator.gameObject.transform.right = Vector3.left;
		}

		if (inputX > 0) {
			//Rotate the sprite right.
			Animator.gameObject.transform.right = Vector3.right;
		}

		//If no other animator should play then player is idle.
		Animator.SetBool("JumpUp", false);
		Animator.SetBool("JumpDown", false);
		Animator.SetBool("JumpPeak", false);
		Animator.SetBool("StartRunning", false);
		Animator.SetBool("StopRunning", false);
		Animator.SetBool("StartGliding", false);
		Animator.SetBool("StopGliding", false);
		Animator.SetBool("JumpLand", false);
		Animator.SetBool("Death", false);
		Animator.SetBool("Idle", true);
		if (inputX <= -1.0f || inputX >= 1.0f && grounded) {
			//Player is running.
			Animator.SetBool("JumpUp", false);
			Animator.SetBool("JumpDown", false);
			Animator.SetBool("JumpPeak", false);
			Animator.SetBool("StartRunning", true);
			Animator.SetBool("StopRunning", false);
			Animator.SetBool("StartGliding", false);
			Animator.SetBool("StopGliding", false);
			Animator.SetBool("JumpLand", false);
			Animator.SetBool("Death", false);
			Animator.SetBool("Idle", false);
			lastInput = inputX;
		}
		if (RB.velocity.y > 0.1f && !grounded) {
			//Player is moving up.
			Animator.SetBool("JumpUp", true);
			Animator.SetBool("JumpDown", false);
			Animator.SetBool("JumpPeak", false);
			Animator.SetBool("StartRunning", false);
			Animator.SetBool("StopRunning", false);
			Animator.SetBool("StartGliding", false);
			Animator.SetBool("StopGliding", false);
			Animator.SetBool("JumpLand", false);
			Animator.SetBool("Death", false);
			Animator.SetBool("Idle", false);
		}
		if (RB.velocity.y > -0.1f && RB.velocity.y < 0.1f && !grounded) {
			//Player is peaking.
			Animator.SetBool("JumpUp", false);
			Animator.SetBool("JumpDown", false);
			Animator.SetBool("JumpPeak", true);
			Animator.SetBool("StartRunning", false);
			Animator.SetBool("StopRunning", false);
			Animator.SetBool("StartGliding", false);
			Animator.SetBool("StopGliding", false);
			Animator.SetBool("JumpLand", false);
			Animator.SetBool("Death", false);
			Animator.SetBool("Idle", false);
		}
		if (RB.velocity.y < -0.1f && !grounded) {
			//Player is moving down.
			Animator.SetBool("JumpUp", false);
			Animator.SetBool("JumpDown", true);
			Animator.SetBool("JumpPeak", false);
			Animator.SetBool("StartRunning", false);
			Animator.SetBool("StopRunning", false);
			Animator.SetBool("StartGliding", false);
			Animator.SetBool("StopGliding", false);
			Animator.SetBool("JumpLand", false);
			Animator.SetBool("Death", false);
			Animator.SetBool("Idle", false);
		}
	}
	#endregion
}
