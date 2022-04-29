using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetToCheckpointScript : MonoBehaviour {

	GameObject player = null;

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag == "Player") {

			player = collision.gameObject;

			//Call player's death toggle
			collision.GetComponent<PlayerPlatformer>().PlayerDeathToggleOn(transform.up * 4);

			//Move the player back to the last saved checkpoint.
			StartCoroutine("WaitForReset");

			//Fire death event.
			//Play a hurt sound.
			StartCoroutine(AudioManagerScript.PlaySoundEffect("GlassBreak Reverse", Camera.main.gameObject.transform.position));
		}
	}

	IEnumerator WaitForReset()
    {
		GameObject.FindGameObjectWithTag("CircleWipe").GetComponent<CircleWipe>().WipeInAndOut(0.8f);
		yield return new WaitForSeconds(0.8f);
		CheckpointManagerScript.ResetToCheckpoint();
		player.GetComponent<PlayerPlatformer>().PlayerDeathToggleOff();
	}
}
