using UnityEngine;
using System.Collections;

public class PlatformMover : MonoBehaviour {

	/// <summary>
	/// This class handles game difficulty by changing platforms sizes and 
	/// increasing the distance from current player position.
	/// </summary>

	public GameObject childHead;	//head game object
	public GameObject childBody;	//body game object
	public GameObject itemInside;	//just for platforms with gem. leave empty for normal platforms

	private float speed = 3.0f;		//movement speed when this platfrom should move near other platforms

	internal bool canGoToPosition = false;		//flag for move animation
	internal Vector2 targetPosition;			//destination of the platform after successful jump of the player

	private float startPosition = 15.0f;		//start outside of game view

	private float targetWidth = 0.5f; 			//must be a multiply of 0.5f
	internal int widthModifier = 1;				//can be 1, 2 or 3


	void Start() {
		//hardness of the game
		targetPosition = new Vector2(Random.Range (0.25f, 2.5f) + (GameController.platformCreated * 0.05f),
		                             Random.Range (-4.5f, -5.0f));

		widthModifier = Random.Range (1, 4);
		//widthModifier = 1;	//for cheat or debug

		//if this is a platfrom with gem, make sure to correct the scale for it's child (gem object)
		//after we set a different width than the default value.
		if(gameObject.tag == "platform-gem" && itemInside != null && widthModifier > 1) {
			Vector3 startingScale = itemInside.transform.localScale;
			itemInside.transform.localScale = new Vector3(startingScale.x / widthModifier,
			                                              itemInside.transform.localScale.y,
			                                              itemInside.transform.localScale.z);
		}

		//move the platform inside game view
		if (canGoToPosition) {
			childHead.GetComponent<Renderer>().material.SetTextureScale ("_MainTex", new Vector2 (widthModifier, 1));
			childBody.GetComponent<Renderer>().material.SetTextureScale ("_MainTex", new Vector2 (widthModifier, 12));
			targetWidth *= widthModifier;
			transform.localScale = new Vector3 (targetWidth, transform.localScale.y, transform.localScale.z);
			StartCoroutine(goToPosition(targetPosition));
		}
	}
	

	void Update () {

		//used to move all the platforms in the scene at once
		if(PlayerManager.platformGlobalShift) {
			transform.position -= new Vector3(Time.deltaTime * speed, 0,0);
		}

		//destroy the platfrom object, if its far from the view
		if(transform.localPosition.x < -7.5f)
			Destroy(gameObject);
	}


	/// <summary>
	/// Move this platfrom to it's starting point inside game view
	/// </summary>
	public IEnumerator goToPosition(Vector2 _pos) {
		float t = 0;
		while (t < 1) {
			t += Time.deltaTime * 3.0f;
			transform.position = new Vector3(Mathf.SmoothStep(startPosition, targetPosition.x, t), transform.position.y, targetPosition.y);
			yield return 0;
		}
	}
}
