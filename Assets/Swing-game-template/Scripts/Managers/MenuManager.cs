using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
	
	///*************************************************************************///
	/// Main Menu Controller.
	/// This class handles all touch events on buttons, and also updates the 
	/// player status on screen.
	///*************************************************************************///
	
	private float buttonAnimationSpeed = 9;		//speed on animation effect when tapped on button
	private bool canTap = true;					//flag to prevent double tap
	public AudioClip tapSfx;					//tap sound for buttons click
	
	//Reference to GameObjects
	public GameObject bestScore;				//UI 3d text object
	public GameObject gemsLabel;


	//*****************************************************************************
	// Init. Updates the 3d texts with saved values fetched from playerprefs.
	//*****************************************************************************
	void Awake (){
		bestScore.GetComponent<TextMesh>().text = PlayerPrefs.GetInt("bestScore").ToString();
		gemsLabel.GetComponent<TextMesh>().text = PlayerPrefs.GetInt("availableGem").ToString();
	}


	//*****************************************************************************
	// FSM
	//*****************************************************************************
	void Update (){	
		if(canTap) {
			StartCoroutine(tapManager());
		}
	}


	//*****************************************************************************
	// This function monitors player touches on menu buttons.
	// detects both touch and clicks and can be used with editor, handheld device and 
	// every other platforms at once.
	//*****************************************************************************
	private RaycastHit hitInfo;
	private Ray ray;
	IEnumerator tapManager (){
		if (Input.GetKeyDown (KeyCode.JoystickButton0))
		{
			playSfx(tapSfx);							//play touch sound
			//StartCoroutine(animateButton(objectHit));	//touch animation effect
			PlayerPrefs.SetInt("playerReviveScore", 0);	//there is no revive score if we are starting a new game
			yield return new WaitForSeconds(.3f);		//Wait for the animation to end
			SceneManager.LoadScene("Game");				//Load the next scene
		}

		//Mouse of touch?
		if(	Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)  
			ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
		else if(Input.GetMouseButtonUp(0))
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		else
			yield break;
		
		if (Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.transform.gameObject;
			switch(objectHit.name) {
				
			case "btnPlay":
				playSfx(tapSfx);							//play touch sound
				StartCoroutine(animateButton(objectHit));	//touch animation effect
				PlayerPrefs.SetInt("playerReviveScore", 0);	//there is no revive score if we are starting a new game
				yield return new WaitForSeconds(1.0f);		//Wait for the animation to end
				SceneManager.LoadScene("Game");				//Load the next scene
				break;

			case "btnExit":
				playSfx(tapSfx);
				StartCoroutine(animateButton(objectHit));
				yield return new WaitForSeconds(1.0f);
				Application.Quit();
				break;	
			}	
		}
	}

	
	//*****************************************************************************
	// This function animates a button by modifying it's scales on x-y plane.
	// can be used on any element to simulate the tap effect.
	//*****************************************************************************
	IEnumerator animateButton ( GameObject _btn  ){
		canTap = false;
		Vector3 startingScale = _btn.transform.localScale;	//initial scale	
		Vector3 destinationScale = startingScale * 1.1f;	//target scale
		
		//Scale up
		float t = 0.0f; 
		while (t <= 1.0f) {
			t += Time.deltaTime * buttonAnimationSpeed;
			_btn.transform.localScale = new Vector3(Mathf.SmoothStep(startingScale.x, destinationScale.x, t),
			                                        _btn.transform.localScale.y,
			                                        Mathf.SmoothStep(startingScale.z, destinationScale.z, t));
			yield return 0;
		}
		
		//Scale down
		float r = 0.0f; 
		if(_btn.transform.localScale.x >= destinationScale.x) {
			while (r <= 1.0f) {
				r += Time.deltaTime * buttonAnimationSpeed;
				_btn.transform.localScale = new Vector3(Mathf.SmoothStep(destinationScale.x, startingScale.x, r),
				                                        _btn.transform.localScale.y,
				                                        Mathf.SmoothStep(destinationScale.z, startingScale.z, r));
				yield return 0;
			}
		}
		
		if(r >= 1)
			canTap = true;
	}


	//*****************************************************************************
	// Play sound clips
	//*****************************************************************************
	void playSfx ( AudioClip _clip  ){
		GetComponent<AudioSource>().clip = _clip;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}
	
}