using UnityEngine;
using System.Collections;

public class TriggerDetector : MonoBehaviour {

	/// <summary>
	/// trigger detector for gem objects.
	/// we use a different collider on player to detect collision with gems.
	/// refer to player object's structure to inspect this. "PlayerBody" collider and tag are used
	/// solely for detecting collision with gems.
	/// </summary>

	public AudioClip itemCollect;
	
	void OnTriggerEnter(Collider c) {
		
		if(gameObject.tag == "gem" && c.gameObject.tag == "PlayerBody") {
			playSfx(itemCollect);
			GameController.collectedGem++;
			PlayerPrefs.SetInt("availableGem", PlayerPrefs.GetInt("availableGem") + 1);
			GetComponent<Renderer>().enabled = false;
			GetComponent<BoxCollider>().enabled = false;
			Destroy(gameObject, 1);
		}
	}

	void playSfx(AudioClip _sfx) {
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}

}
