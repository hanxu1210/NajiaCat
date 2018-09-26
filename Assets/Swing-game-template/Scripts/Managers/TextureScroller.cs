using UnityEngine;
using System.Collections;

public class TextureScroller : MonoBehaviour {

	///***********************************************************************
	/// This script will scroll the background texture of the main game.
	/// It also carefully sync the speed with GameController's global speed to 
	/// avoid undesired effects, like when objects are sliding on the background.
	///***********************************************************************

	private float offset;
	private float damper = 0.1f;

	[Range(1, 4)]
	public float coef = 2.0f;
	
	void Update () {

		if(PlayerManager.platformGlobalShift) {
			offset +=  damper * Time.deltaTime * coef * (GetComponent<Renderer>().material.mainTextureScale.x / 1.5f);
			GetComponent<Renderer>().material.SetTextureOffset ("_MainTex", new Vector2(offset, 0));
		}
	}
}
