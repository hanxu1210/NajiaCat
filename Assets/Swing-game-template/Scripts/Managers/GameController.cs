using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	/// <summary>
	/// Main game controller.
	/// Handles current score, best score, new platform creation, 
	/// assigning random indexs to objects to have different textures,
	/// and also handles gameover situation.
	/// </summary>

	public GameObject scoreText;			//3d text object that shows the current score
	public GameObject gemText;				//3d text object that shows the collected gems

	public static int score;				//current score
	public static int bestScore;			//best saved score
	public static int availableGem;			//total collected gems
	public static int collectedGem;			//collected gems of this round of play
	
	public GameObject platform;				//normal platform prefab
	public GameObject platformGem;			//gem platform prefab

	public GameObject gameoverPlane;		//gameover plane game object
	public static bool gameover;			//gameover flag
	private bool goFlag;					//gameover plane animation flag

	private bool canCreatePlatform;			//new platform creation flag
	public static int platformCreated;		//total number of created platforms so far (unused)

	public int availableBackgrounds = 3;		//total number of textures we have for game background
	public int availablePlatforms = 2;			//total number of textures we have for platforms (head + body)
	public static int randomBackgroundIndex;	//we choose a different texture each time we run the game
	public static int randomPlatfromIndex;		//we choose a different texture each time we run the game

	//New
	public static int gemsRequiredToRevive = 3;	//players can spend their gems to revive and continue the game.
	public GameObject reviveTextUI;				//The 3d text game object


	void Awake () {

		//PlayerPrefs.DeleteAll();	//incase you want to reset game settings
		Application.targetFrameRate = 60;	//useful for Android and iOS builds

		gameover = false;
		goFlag = false;
		gameoverPlane.SetActive(false);
		canCreatePlatform = true;
		platformCreated = 0;
		collectedGem = 0;

		//if this is a revived game, add previous score to the current score
		if(PlayerPrefs.GetInt ("playerReviveScore") > 0)
			score = PlayerPrefs.GetInt ("playerReviveScore");
		else
			score = 0;

		bestScore = PlayerPrefs.GetInt("bestScore");
		availableGem = PlayerPrefs.GetInt("availableGem");

		randomBackgroundIndex = Random.Range(0, availableBackgrounds);
		randomPlatfromIndex = Random.Range(0, availablePlatforms);

		reviveTextUI.GetComponent<TextMesh>().text = gemsRequiredToRevive.ToString();

		//debug saved data
		//print ("BestScore: " + bestScore + " - " + "availableGem: " + availableGem);
	}
	
	
	void Update() {

		//show player score + collected gems on screen
		if(!gameover) {
			scoreText.GetComponent<TextMesh>().text = score.ToString();
			gemText.GetComponent<TextMesh>().text = collectedGem.ToString();
		} else {
			scoreText.GetComponent<Renderer>().enabled = false;
		}
	}


	/// <summary>
	/// Clones a new platfrom after player performs a successful jump.
	/// </summary>
	public IEnumerator clonePlatform() {

		if(canCreatePlatform) {

			platformCreated++;
			canCreatePlatform = false;

			GameObject newPlatform;
			GameObject platformToClone;

			//with a chance of 1 out of 4, create a platform with gem
			if(Random.value <= 0.75)
				platformToClone = platform;
			else
				platformToClone = platformGem;

			newPlatform = Instantiate(platformToClone, new Vector3(1.5f, 0.5f, -5) , Quaternion.Euler(0, 180, 0) ) as GameObject;
			newPlatform.name = "Platform";
			newPlatform.GetComponent<PlatformMover>().canGoToPosition = true;

			yield return new WaitForSeconds(1.5f);
			canCreatePlatform = true;
		}
	}
	
	
	void LateUpdate () {

		//if gameover happened, play the gameover plan animation just once
		if(gameover && !goFlag) {
			goFlag = true;
			StartCoroutine(processGameover());
		}
	}


	/// <summary>
	/// unhide gameover plane and animate it inside the view
	/// </summary>
	IEnumerator processGameover() {
		gameoverPlane.SetActive(true);
		yield return new WaitForSeconds (0.5f);	//let the player fall completely
		float t = 0;
		while(t < 1) {
			t += Time.deltaTime * 1.4f;
			gameoverPlane.transform.position = new Vector3(gameoverPlane.transform.position.x,
			                                               gameoverPlane.transform.position.y,
			                                               Mathf.SmoothStep(-7, 7, t));
			yield return 0;
		}
	}
	
	
	

}
