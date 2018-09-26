using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {

	/// <summary>
	/// 
	/// </summary>

	public GameObject collisionParticle;		//smoke particle system used after each successful jump
	public float playerMoveSpeed = 1.5f;		//player speed on platforms (when pulling the rope)
	private float startPosition = -2.0f;		//Important: starting position of the player

	public GameObject nearestHook;				//hook we use to draw the rope
	public GameObject playerBody;				//reference to child game object that renders player's texture
	public GameObject gameController;			//reference to main game controller

	public static bool isSwinging;				//has jump procedure begun (swinging forward)?
	public static bool swingBack;				//is player swinging back on the rope?

	public static bool platformGlobalShift;		//flag used to move all platforms at once
	public static bool canStartTheGame;			//global flag to stop the game at the first run

	//new slide image
	public Material[] playerStatus;				//different player status like walk, slide or idle.

	//Audioclips
	public AudioClip fallSfx;
	public AudioClip walkSfx;
	public AudioClip hitSfx;
	public AudioClip landSfx;
	public AudioClip jumpSfx;

	//****************************
	// line (Rope) settings
	private float startWidth = 0.06f;			//rope starting width
	private float endWidth = 0.06f;				//rope ending width
	public Material ropeMat;					//rope material(texture)
	private LineRenderer line;					//Line renderer component
	private GameObject target;					//target object for starting point of line renderer
	public GameObject dummyTarget;				//dummy object which always follows player's position
	//*****************************


	void Awake () {

		//cache important game objects
		//nearestHook = GameObject.FindGameObjectWithTag("hook");
		//dummyTarget = GameObject.FindGameObjectWithTag("dummyTarget");
		//gameController = GameObject.FindGameObjectWithTag("GameController");

		//init all variables
		isSwinging = false;
		swingBack = false;
		target = playerBody;
		GetComponent<Rigidbody>().drag = 100;
		platformGlobalShift = false;
		canStartTheGame = false;

		playerBody.GetComponent<Renderer>().material = playerStatus[0];
		playerBody.GetComponent<AnimatedAtlas>().enabled = false;		//disable player walk animation
		playerBody.transform.eulerAngles = new Vector3(0, 180, 0);		//keep player object straight up

		line = this.gameObject.GetComponent<LineRenderer>();
		line.GetComponent<Renderer>().enabled = false;
		//line.SetWidth(startWidth, endWidth);
		line.startWidth = startWidth;
		line.endWidth = endWidth;
		//line.SetVertexCount(2);
		line.positionCount = 2;
		line.SetPosition(0, target.transform.position + new Vector3(0, 0, -0.35f));
		line.SetPosition(1, nearestHook.transform.position);
	}


	IEnumerator Start () {

		//clone the first platform
		StartCoroutine(gameController.GetComponent<GameController>().clonePlatform());

		//wait for the platform to arrive
		yield return new WaitForSeconds(0.35f);

		//start the game
		canStartTheGame = true;
		line.GetComponent<Renderer>().enabled = true;
	}
	

	void Update () {

		//no input is allowed of we are in the middle of our jump
		if(isSwinging || GameController.gameover || !canStartTheGame)
			return;

		if(!Input.anyKey && Input.touches.Length == 0 && !isSwinging) {
			if(transform.position.x > -7.8f) {
				setMovement();
				setLine(1);
			} else {
				//if player pulled the rope for too long, make it fall from the other side
				transform.parent = null;
				gameObject.GetComponent<BoxCollider>().enabled = false;
				GetComponent<Rigidbody>().isKinematic = false;
				GetComponent<Rigidbody>().useGravity = true;
				setLine(0);
				GetComponent<Rigidbody>().drag = 0;
				GetComponent<Rigidbody>().AddForce(new Vector3(-80, 0, 0));
				GameController.gameover = true;
				playSfx(fallSfx);
			}
		}

		//get player input (mouse + touch + keyboard)
		if(Input.GetKeyDown (KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
			isSwinging = true;
			StartCoroutine(initSwing());
		}

	}


	void LateUpdate() {
		//fast reset the game for debug
		if(Input.GetKeyDown(KeyCode.R))
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}


	//move the player on the platform
	void setMovement() {
		transform.position -= new Vector3(Time.deltaTime * playerMoveSpeed, 0, 0);
		playerBody.GetComponent<AnimatedAtlas>().enabled = true;
		playerBody.transform.eulerAngles = new Vector3(0, 150, 0);	//fake struggle when pulling the rope
		playSfx(walkSfx);
	}
	
	//render a line between player and the hook
	void setLine(int _state) {

		if(_state == 0) {
			line.GetComponent<Renderer>().enabled = false;
			return;
		}

		line.SetPosition(0, target.transform.position + new Vector3(0, 0, -0.35f));
		line.SetPosition(1, nearestHook.transform.position);	
	}


	//main swing (jump) routine
	IEnumerator initSwing() {

		playSfx(jumpSfx);

		//remove player physics properties and make it a child of the hook
		//so it can rotate with the hook.
		GetComponent<Rigidbody>().drag = 0;
		transform.parent = nearestHook.transform;
		gameObject.GetComponent<BoxCollider>().enabled = false;
		gameObject.GetComponent<Rigidbody>().isKinematic = true;
		gameObject.GetComponent<Rigidbody>().useGravity = false;

		//stop player's walk animation
		playerBody.GetComponent<AnimatedAtlas>().enabled = false;
		playerBody.transform.eulerAngles = new Vector3(0, 180, 0);
		playerBody.GetComponent<Renderer>().material = playerStatus[1];

		float t = 0.0f;
		while(t < 1) {
			t += Time.deltaTime * 0.9f;
			setLine(1);
			nearestHook.transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.SmoothStep(0, -205, t), 0));
			gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
			yield return 0;
		}


		//when player is swinging back, we enable collider, so we could land on platforms
		float v = 0.0f;
		if(t >= 1) {
			swingBack = true;

			gameObject.GetComponent<BoxCollider>().enabled = true;
			gameObject.GetComponent<Rigidbody>().isKinematic = false;

			while(v < 1 && swingBack) {
				v += Time.deltaTime * 0.9f;
				setLine(1);
				nearestHook.transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.SmoothStep(-205, -30, v), 0));
				gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
				yield return 0;
			}
		}

		if(v >= 1) {
			//print ("Line swing ended!");
			setLine(0);
			yield break;
		}

	}
	
	//different collision senarios
	void OnCollisionEnter(Collision c) {

		//we can land on platfrom heads
		if(c.gameObject.tag == "platformHead") {

			//disable the collider again and make the player a child of no one
			gameObject.GetComponent<BoxCollider>().enabled = false;
			GameObject tmpParticle = Instantiate(collisionParticle, 
			                                     c.contacts[0].point + new Vector3(0, 1.2f, 0), 
			                                     Quaternion.Euler(0, 0, 0)) as GameObject;
			tmpParticle.name = "Particle";
			playSfx(landSfx);
			GameController.score++;	//add one to acore

			//change the texture to normal walk status
			playerBody.GetComponent<Renderer>().material = playerStatus[0];

			//let the rope continue its swing animation (change the target from player to dummy)
			target = dummyTarget;
			dummyTarget.GetComponent<FollowPlayer>().canFollowPlayer = false;
			transform.parent = null;

			//make the player a rigidbody with a very high drag value (to fix it in it's position)
			gameObject.GetComponent<Rigidbody>().isKinematic = false;
			gameObject.GetComponent<Rigidbody>().useGravity = true;
			GetComponent<Rigidbody>().drag = 100;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

			StartCoroutine(createNewPlatform(c.gameObject.transform.parent.gameObject));
		}

		//we should lose and fall if we collide with platfromBody
		if(c.gameObject.tag == "platformBody") {

			//remove physics properties
			gameObject.GetComponent<BoxCollider>().enabled = false;
			GameObject tmpParticle = Instantiate(collisionParticle, 
			                                     c.contacts[0].point + new Vector3(0, 1.2f, 0), 
			                                     Quaternion.Euler(0, 0, 0)) as GameObject;
			tmpParticle.name = "Particle";
			playSfx(hitSfx);

			//change the texture to normal walk status
			playerBody.GetComponent<Renderer>().material = playerStatus[0];

			target = dummyTarget;
			dummyTarget.GetComponent<FollowPlayer>().canFollowPlayer = false;
			transform.parent = null;
			gameObject.GetComponent<Rigidbody>().isKinematic = false;
			gameObject.GetComponent<Rigidbody>().useGravity = true;
			GetComponent<Rigidbody>().drag = 0;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

			//game over
			print ("Game Over");
			GameController.gameover = true;
		}

		//if we fall and hit the ground object, the game is over
		if(c.gameObject.tag == "ground") {
			gameObject.GetComponent<BoxCollider>().enabled = false;
			dummyTarget.GetComponent<FollowPlayer>().canFollowPlayer = false;
			target = dummyTarget;
			transform.parent = null;
			gameObject.GetComponent<Rigidbody>().isKinematic = false;
			gameObject.GetComponent<Rigidbody>().useGravity = true;
			GetComponent<Rigidbody>().drag = 1000;

			//game over
			print ("Game Over");
			GameController.gameover = true;
		}
	}


	//create new platform 
	IEnumerator createNewPlatform(GameObject platform) {

		//first of all, check the result of our last jump.

		//if we jumped and there is a gem nearby, but we didn't collide with it, let it drop
		GameObject gemNearby = GameObject.FindGameObjectWithTag("gem");
		if(gemNearby != null) {
			ItemMover.canDrop = true;
			gemNearby.GetComponent<ItemMover>().drop();
		}

		//stop and let things cool down
		yield return new WaitForSeconds (0.35f);

		//make the player a child of this platform (we want the player to move with it)
		transform.parent = platform.transform;
		GetComponent<Rigidbody>().drag = 0;
		gameObject.GetComponent<BoxCollider>().enabled = false;
		gameObject.GetComponent<Rigidbody>().isKinematic = true;
		gameObject.GetComponent<Rigidbody>().useGravity = false;

		//move the new platform near old platform
		float platformPosX = platform.transform.position.x;
		float platformPosZ = platform.transform.position.z;
		float t = 0.0f;
		float flickOffset = 0.05f;
		float dest = (-1.5f + (platform.GetComponent<PlatformMover>().widthModifier * 0.25f) - flickOffset);
		while(t < 1 && platform.transform.position.x > dest) {
			t += Time.deltaTime * 2.1f;
			platform.transform.position = new Vector3(Mathf.Lerp(platformPosX, dest, t), 
			                                          0.5f, 
			                                          Mathf.Lerp(platformPosZ, -4, t));
			yield return 0;
		}

		//shift all platforms to the left
		while(t >= 1) {
			platformGlobalShift = true;

			//print ("cloning new platform...");
			StartCoroutine(gameController.GetComponent<GameController>().clonePlatform());

			float newStartPos = (-1.5f - (platform.GetComponent<PlatformMover>().widthModifier * 0.25f));
			if(platform.transform.position.x <= newStartPos) {

				platformGlobalShift = false;
				platform.transform.position = new Vector3(newStartPos, 
				                                          platform.transform.position.y, 
				                                          platform.transform.position.z);
				//move player to starting position if it has offset
				if(Mathf.Abs(transform.position.x - startPosition) > 0.2f) {
					print ("Position adjustment needed.");
					playerBody.GetComponent<AnimatedAtlas>().enabled = true;
					float currentXpos = transform.position.x;
					float t2 = 0;
					while(t2 < 1) {
						t2 += Time.deltaTime * 3.0f;
						transform.position = new Vector3(Mathf.SmoothStep(currentXpos, startPosition, t2),
						                                 transform.position.y,
						                                 transform.position.z);
						yield return 0;
					}
				}

				playerBody.GetComponent<AnimatedAtlas>().enabled = false;
				yield return new WaitForSeconds (0.35f);

				//re-init all variables we changed before
				isSwinging = false;
				swingBack = false;
				transform.parent = null;
				target = playerBody;
				GetComponent<Rigidbody>().drag = 100;
				//line.renderer.enabled = true;
				nearestHook.transform.eulerAngles = new Vector3(0, 0, 0);
				dummyTarget.GetComponent<FollowPlayer>().canFollowPlayer = true;
				//end re-init

				yield return new WaitForSeconds (0.15f);
				line.GetComponent<Renderer>().enabled = true;
				playerBody.GetComponent<AnimatedAtlas>().enabled = true;

				yield break;
			}
			yield return 0;
		}
	}


	///***********************************************************************
	/// Play audioclip
	///***********************************************************************
	void playSfx(AudioClip _sfx) {
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}

}
