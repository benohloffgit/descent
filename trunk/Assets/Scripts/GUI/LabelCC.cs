using System;
using UnityEngine;

public class LabelCC : MonoBehaviour {
	public Transform background;
	public Renderer myRenderer;
	private TextCC text;
	private MyGUI myGUI;
//	private int containerID;
	private Transform labelImage;
	
	void Awake() {
		text = GetComponentInChildren<TextCC>();
		myRenderer = GetComponentInChildren<Renderer>();
	}
	
	public TextCC GetText() {
		return text;
	}
	
	public void SetText(string t) {
		text.SetText(t);
	}
	
	public Vector3 GetSize() {
		return new Vector3(myRenderer.bounds.size.x, myRenderer.bounds.size.y, 1.0f);
//		return text.GetSize();
	}
	
	public void Initialize(MyGUI mG, string t, int cID, float textMargin, float size, MyGUI.GUIAlignment alignLeftRightCenter,
			int textureID) {
		myGUI = mG;
		text.Initialize(mG, t, size, cID, textMargin, alignLeftRightCenter, textureID);
	}
	
	public void Initialize(MyGUI mG, string t, int cID, Transform backgr, float textMargin, float size, MyGUI.GUIAlignment alignLeftRightCenter,
			Vector3 scale, int textureID) {
		myGUI = mG;
		background = backgr;
		text.Initialize(mG, t, size, cID, textMargin, alignLeftRightCenter, textureID);
		text.transform.localScale = scale;
//		Vector3 textSize = text.GetSize();
//		background.parent = null;
//		background.localScale = new Vector3(textSize.x + textMargin*2, textSize.y + textMargin*2, 1.0f);
		background.localScale = text.GetSize();
		background.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		background.parent = transform;
	}
	
	public void Initialize(MyGUI mG, string t, int cID, Transform backgr, float textMargin, float size, MyGUI.GUIAlignment alignLeftRightCenter,
			int textureID) {
		myGUI = mG;
		background = backgr;
		text.Initialize(mG, t, size, cID, textMargin, alignLeftRightCenter, textureID);

		Vector3 textSize = text.GetSize();
		background.localScale = new Vector3(myGUI.containers[cID].GetSize().x, textSize.y + textMargin*2, 1.0f);
		background.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		background.parent = transform;
	}
	
	// initialize as image label
	public void Initialize(MyGUI mG, string t, int cID, Transform backgr, Vector4 textMargin, float size, MyGUI.GUIAlignment alignLeftRightCenter,
			Transform imageT, float scaleImage, int textureID) {
		myGUI = mG;
		background = backgr;
		labelImage = imageT;
		text.Initialize(mG, t, size, cID, textMargin, alignLeftRightCenter, textureID);
		
		Vector3 textSize = text.GetSize();
		background.localScale = new Vector3(myGUI.containers[cID].GetSize().x, textSize.y + textMargin.z + textMargin.w, 1.0f);
		background.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		background.parent = transform;
		
		labelImage.localScale *= scaleImage;
		labelImage.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1.0f);
		MyGUI.Align(labelImage.position, labelImage.lossyScale, background.lossyScale, labelImage, transform, MyGUI.GUIAlignment.Left, 0.01f, MyGUI.GUIAlignment.Top, 0.01f);
		labelImage.parent = transform;
	}
	
}


