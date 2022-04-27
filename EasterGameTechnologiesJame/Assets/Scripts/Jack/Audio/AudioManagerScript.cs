using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AudioManagerScript : MonoBehaviour {
	#region Variables to assign via the unity inspector (SerializeFields).
	[SerializeField]
	[Range(1, 150)]
	private int audioSourcePoolSize = 20;

	[SerializeField]
	private GameObject audioSourcePrefab = null;

	//[SerializeField]
	//private List<AudioFile> damagePlayerFiles = new List<AudioFile>();
	#endregion

	#region Private Variable Declarations.
	private Vector3 cameraPos;
	private static GameObject musicAudioSource = null;
	private static ObjectPool audioSourcePool;
	private static bool transitionMusic = false;
	private static float transitionTime = 0.0f;
	private static AudioFile newMusic = null;
	private static bool newMusicShouldLoop = false;
	private static float maxVolume = 0.0f;
	private float timer = 0.0f;

	#endregion

	#region Private Functions.
	// Start is called before the first frame update
	void Start() {
		//Create the audio source pool.
		audioSourcePool = new ObjectPool(this.gameObject.transform, audioSourcePrefab, audioSourcePoolSize);

		//Create an audio source specifically for music.
		musicAudioSource = Instantiate(audioSourcePrefab, this.gameObject.transform);
		musicAudioSource.GetComponent<AudioSource>().clip = null;

		//Reset static variables.
		transitionMusic = false;
		transitionTime = 0.0f;
		newMusic = null;
		newMusicShouldLoop = false;
		maxVolume = 0.0f;

		//Subscribe listeners to correct events.
		//BaseEnemy.DamagePlayer += DamagePlayerListener;
	}

	// Update is called once per frame
	void Update() {
		//Move the music audio source infront of the main camera.
		cameraPos = new Vector3(0.0f, 0.0f, 0.0f);
		GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
		if (cam) {
			cameraPos = cam.transform.position;
		}
		musicAudioSource.transform.position = new Vector3(cameraPos.x, cameraPos.y, cameraPos.z);

		//Transition music to new audio file if appropriate.
		if (transitionMusic && newMusic != null) {
			PlayNewMusic();
		}
	}

	private void PlayNewMusic() {
		//Get the music audio source object.
		AudioSource audioSource = musicAudioSource.GetComponent<AudioSource>();

		//If the music should immediately start playing skip transition.
		if (transitionTime <= 0 || audioSource.clip == null) {
			InstantPlayMusic(audioSource);
		} else {
			//Transition to the new music.
			TransitionMusic(audioSource);
		}
	}

	private void InstantPlayMusic(AudioSource audioSource) {
		//Stop any previous music from playing.
		audioSource.Stop();

		//Make sure transition music is set to false so music isn't messed up.
		transitionMusic = false;
		timer = 0.0f;

		//Find the audio clip with the specified name.
		AudioClip audioClip = newMusic.audioClip;
		float volume = newMusic.volume;

		//Set the music volume to the correct value.
		audioSource.volume = volume;

		if (audioClip != null) {
			//Pass audio clip values to the audio source and play the clip.
			audioSource.clip = audioClip;
			audioSource.loop = newMusicShouldLoop;
			audioSource.Play();
		} else {
			Debug.LogError("Invalid music file passed into the audio manager.");
		}
	}

	private void TransitionMusic(AudioSource audioSource) {
		//Variables for transition.
		float volumeIncrement = 0.0f;

		if (timer <= (transitionTime / 2.0f)) {
			//Calculate how much the volume will need to decrement each frame.
			volumeIncrement = maxVolume / (transitionTime / 2);

			//Decrease volume of music audio source.
			audioSource.volume -= volumeIncrement * Time.deltaTime;

			//If the next frame is gonna be past the midpoint.
			if (timer + Time.deltaTime > (transitionTime / 2.0f)) {
				//Set audio source file to new music file.
				audioSource.Stop();
				audioSource.volume = 0.0f;
				audioSource.clip = newMusic.audioClip;
				audioSource.Play();
				audioSource.loop = newMusicShouldLoop;
				maxVolume = newMusic.volume;
			}
		} else {
			//Calculate how much the volume will need to increment each frame.
			volumeIncrement = maxVolume / (transitionTime / 2);

			//Increment volume of audio source.
			audioSource.volume += volumeIncrement * Time.deltaTime;
		}

		Debug.Log("Current volume: " + audioSource.volume + " Timer Value: " + timer);

		//Increment timer.
		timer += Time.deltaTime;

		if (timer >= transitionTime) {
			//Transition complete Ensure variables are reset and correct..
			audioSource.volume = maxVolume;
			transitionMusic = false;
			timer = 0.0f;
		}
	}

	private void OnDisable() {
		//Unsubscribe listeners from events.
		//BaseEnemy.DamagePlayer -= DamagePlayerListener;
	}

	/// <summary>
	/// Player an audio file that has been given the passed in name.
	/// Also can give the audio source a position to play the sound from.
	/// </summary>
	/// <param name="name"></param>
	private IEnumerator PlaySoundEffect(Vector3 a_position, List<AudioFile> listOfSFX) {
		//Check list is valid.
		if (listOfSFX.Count <= 0) {
			//Early out.
			Debug.LogError("Sound Effect list needs to be populated in audio manager.");
			yield return new WaitForEndOfFrame();
		} else {

			//Choose a random sfx.
			int randomIndex = Random.Range(0, listOfSFX.Count - 1);

			//Find the audio clip with the specified name.
			AudioClip audioClip = listOfSFX[randomIndex].audioClip;
			float volume = listOfSFX[randomIndex].volume;

			//Get an audio source object.
			GameObject audioSoureGameObject = audioSourcePool.SpawnObject();
			AudioSource audioSource = audioSoureGameObject.GetComponent<AudioSource>();

			//Change the volume to the correctly volume for the clip.
			audioSource.volume = volume;

			//Move it to the specified location.
			audioSoureGameObject.transform.position = a_position;

			//Get the length of the audio clip.
			float clipLength = 0.5f;
			if (audioClip != null) {
				//Pass audio clip values to the audio source and play the clip.
				clipLength = audioClip.length;
				audioSource.clip = audioClip;
				audioSource.loop = false;
				audioSource.Play();
			} else {
				Debug.LogError("Audio Manager has not been populated with sound effects!!!");
			}

			//Pause execution for length of the audio.
			yield return new WaitForSeconds(clipLength);

			//Deactivate the audio Source.
			audioSource.Stop();
			audioSoureGameObject.SetActive(false);
		}
	}
	#endregion

	#region Event Listeners.

	//private void DamageEnemyListener(object sender, EventArgs e) {
	//	StartCoroutine(PlaySoundEffect(cameraPos, damageEnemyFiles));
	//}


	#endregion

	#region Public Access Functions (Getters and Setters).


	public static void PlayMusicClip(AudioFile audioFile, float a_transitionTime, bool a_loop) {
		//Get the music audio source object.
		AudioSource audioSource = musicAudioSource.GetComponent<AudioSource>();
		newMusic = audioFile;
		transitionTime = a_transitionTime;
		transitionMusic = true;
		newMusicShouldLoop = a_loop;

		//Set the music volume to the correct value.
		newMusic = audioFile;
		maxVolume = audioSource.volume;
	}
	#endregion
}
