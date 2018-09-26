using UnityEngine;
using System.Collections;

public class TextureRandomizer : MonoBehaviour {

	public enum types { background, platform }		//available types for this object (used to change object's texture)
	public types type = types.background;			//selected type

	public Texture2D[] availableTextures;			//available textures to choose from

	void Start () {

		if(availableTextures == null)
			return;

		if(type == types.background) 
			GetComponent<Renderer>().material.mainTexture = availableTextures[GameController.randomBackgroundIndex];
		else if(type == types.platform) 
			GetComponent<Renderer>().material.mainTexture = availableTextures[GameController.randomPlatfromIndex];
	}

}
