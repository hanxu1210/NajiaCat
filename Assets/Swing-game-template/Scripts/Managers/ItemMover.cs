using UnityEngine;
using System.Collections;

public class ItemMover : MonoBehaviour {

	/// <summary>
	/// A very simple movement animation that moves the object up and down.
	/// </summary>

	private float speed = 0.5f;			//movement speed
	private float offset = 0.015f;		//optional position offset

	private Vector3 startingPos;
	private float newTarget;
	private bool canCycle;
	public static bool canDrop;

	
	void Awake () {
		canCycle = true;
		canDrop = false;
		startingPos = transform.localPosition;
		newTarget = startingPos.z + offset;
	}


	void Update () {

		if(canCycle && !canDrop) {
			StartCoroutine(move());
			canCycle = false;
		}

		//if this gem object is dropping, destroy it after a while
		if(transform.localPosition.z > 1.0f)
			Destroy(gameObject);
	}


	/// <summary>
	/// Move this object towards its destination.
	/// </summary>
	IEnumerator move() {
		float t = 0.0f;
		while(t < 1.0f && !canDrop) {
			t += Time.deltaTime * speed;
			transform.localPosition = new Vector3(transform.localPosition.x,
			                                      transform.localPosition.y,
					                              Mathf.SmoothStep(startingPos.z, newTarget, t));
			yield return 0;
		}
		if(transform.localPosition.z <= newTarget)
			StartCoroutine(back());
	}


	/// <summary>
	/// Move this object towards its destination.
	/// </summary>
	IEnumerator back() {
		float t = 0.0f;
		while(t < 1.0f && !canDrop) {
			t += Time.deltaTime * speed;
			transform.localPosition = new Vector3(transform.localPosition.x,
			                                      transform.localPosition.y,
			                               		  Mathf.SmoothStep(newTarget, startingPos.z, t));
			yield return 0;
		}
		if(transform.localPosition.z <= startingPos.z)
			canCycle = true;
	}


	/// <summary>
	/// public function called from other controllers
	/// </summary>
	public void drop() {
		//drop this gem
		gameObject.GetComponent<BoxCollider>().enabled = false;
		gameObject.AddComponent<Rigidbody>();
		GetComponent<Rigidbody>().drag = 2.0f;
	}
}
