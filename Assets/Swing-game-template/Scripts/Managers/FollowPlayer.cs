using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	/// <summary>
	/// We have a dummy object that follows the player all the time, 
	/// so we can draw a line (rope texture) between the hook (pivot) and the player.
	/// </summary>

	internal bool canFollowPlayer;		//flag
	private GameObject player;			//player game object


	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		canFollowPlayer = true;
	}


	void Update () {
		if(canFollowPlayer) {
			transform.position = player.transform.position;
		}
	}
}
