using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameoverManager : MonoBehaviour {
	
	///***********************************************************************
	/// Gameover Manager Class. 
	/// Shows the final score on screen and manages ser inputs on it's buttons.
	/// it also saves player score/newScore/bestScore variables.
	///***********************************************************************

	public GameObject scoreText;			//reference to score game object
	public GameObject bestScoreText;		//reference to best score game object

	//NEW
	public GameObject reviveBtn;

	public AudioClip menuTap;					//audio clip
	private bool canTap;						//tap flag
	private float buttonAnimationSpeed = 9.0f;
	
	
	void Awake () {
		canTap = true;
		reviveBtn.SetActive(false);
	}


	void LateUpdate () {	
		
		//Set the new score on gameover screen
		scoreText.GetComponent<TextMesh>().text = GameController.score.ToString();
		bestScoreText.GetComponent<TextMesh>().text = GameController.bestScore.ToString();

		//if our new score is bigger than our saved best score, display it instead of old bestScore!
		if(GameController.score > GameController.bestScore)
			bestScoreText.GetComponent<TextMesh>().text = GameController.score.ToString();

		if(PlayerPrefs.GetInt("availableGem") >= GameController.gemsRequiredToRevive) {
			reviveBtn.SetActive(true);
		}

		if(canTap)
			StartCoroutine(tapManager());
	}
	
	
	///***********************************************************************
	/// Manage user taps on gameover buttons
	///***********************************************************************
	private RaycastHit hitInfo;
	private Ray ray;
	IEnumerator tapManager() {
		if (KeyboardHandler.IsOkButtonDown())
		{
			playSfx(menuTap);										//play audioclip
			saveScore();											//save players best and last score
			clearReviveSettings();
			//StartCoroutine(animateButton(objectHit));				//animate the button
			yield return new WaitForSeconds(0.4f);					//Wait
			//Application.LoadLevel(Application.loadedLevelName); 	//reload level
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
			print(objectHit.name);
			switch(objectHit.name) {

				case "retryButton":
					playSfx(menuTap);										//play audioclip
					saveScore();											//save players best and last score
					clearReviveSettings();
					StartCoroutine(animateButton(objectHit));				//animate the button
					yield return new WaitForSeconds(0.4f);					//Wait
					//Application.LoadLevel(Application.loadedLevelName); 	//reload level
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
					break;

				case "menuButton":
					playSfx(menuTap);
					saveScore();
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(0.5f);
					SceneManager.LoadScene("Menu");
					break;

				case "likeButton":
					playSfx(menuTap);
					StartCoroutine(animateButton(objectHit));
					//your custom solution here
					break;

				case "highscoreButton":
					playSfx(menuTap);
					StartCoroutine(animateButton(objectHit));
					//your custom solution here
					break;

				case "reviveButton":
					playSfx(menuTap);
					StartCoroutine(animateButton(objectHit));
					
					saveScore();
					//save player curret score
					PlayerPrefs.SetInt ("playerReviveScore", GameController.score);
					//deduct from collected gems
					PlayerPrefs.SetInt("availableGem", PlayerPrefs.GetInt("availableGem") - GameController.gemsRequiredToRevive);
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);

					break;
			}	
		}
	}
	

	///***********************************************************************
	/// Save player score
	///***********************************************************************
	void saveScore() {
		//immediately save the last score
		PlayerPrefs.SetInt("lastScore", GameController.score);
		//check if this new score is higher than saved bestScore.
		//if so, save this new score into playerPrefs. otherwise keep the last bestScore intact.
		int lastBestScore;
		lastBestScore = PlayerPrefs.GetInt("bestScore");
		if(GameController.score > lastBestScore)
			PlayerPrefs.SetInt("bestScore", GameController.score);

		//save player's collected gems
		/*
		int playerCurrentGem = PlayerPrefs.GetInt("availableGem");
		if(GameController.collectedGem > 0) {
			playerCurrentGem += GameController.collectedGem;
			PlayerPrefs.SetInt("availableGem", playerCurrentGem);
		}
		*/
		
	}


	/// <summary>
	/// Clears the revive score.
	/// </summary>
	void clearReviveSettings() {
		PlayerPrefs.SetInt("playerReviveScore", 0);
	}

	
	///***********************************************************************
	/// Animate buttons on touch
	///***********************************************************************
	IEnumerator animateButton(GameObject _btn) {
		canTap = false;
		Vector3 startingScale = _btn.transform.localScale;
		Vector3 destinationScale = startingScale * 1.1f;
 
		float t = 0.0f; 
		while (t <= 1.0f) {
			t += Time.deltaTime * buttonAnimationSpeed;
			_btn.transform.localScale = new Vector3(Mathf.SmoothStep(startingScale.x, destinationScale.x, t),
			                                        _btn.transform.localScale.y,
			                                        Mathf.SmoothStep(startingScale.z, destinationScale.z, t));
			yield return 0;
		}
		
		float r = 0.0f; 
		if(_btn.transform.localScale.x >= destinationScale.x) {
			while (r <= 1.0f)  {
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
	
	
	///***********************************************************************
	/// Play audioclip
	///***********************************************************************
	void playSfx(AudioClip _sfx) {
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}

}
