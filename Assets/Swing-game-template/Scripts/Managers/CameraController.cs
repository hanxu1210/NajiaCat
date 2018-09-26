using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	/// <summary>
	/// Main camera controller. It makes the camera follow player when player moves on platforms.
	/// it also limits camera movement ranges.
	/// </summary>

	private GameObject player;					//reference to player game object
	private float playerPosLimit = -2.5f;		//if player position override this value, camera start to follow
												//the player object


	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}


	void LateUpdate () {

		//follow players position
		followPlayer ();

		//Position limiter
		if(transform.position.x < -8.0f)
			transform.position = new Vector3(-8.0f, transform.position.y, transform.position.z);
	}


	//move the camera based on current player's position
	void followPlayer() {
		if (player.transform.position.x <= playerPosLimit) {
			transform.position = new Vector3(player.transform.position.x - playerPosLimit,
			                                 transform.position.y,
			                                 transform.position.z);
		}
	}
}
