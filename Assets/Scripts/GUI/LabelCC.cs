using System;
using UnityEngine;

public class LabelCC : MonoBehaviour {
	public Transform background;
	private TextCC text;
	private MyGUI myGUI;
//	private int containerID;
	private Transform labelImage;
	
	void Awake() {
		text = GetComponentInChildren<TextCC>();
	}
	
	public TextCC GetText() {
		return text;
	}
	
	public Vector3 GetSize() {
		return background.renderer.bounds.size;
	}

	public void Initialize(MyGUI mG, string t, int cID, Transform backgr, float textMargin, float size, MyGUI.GUIAlignment alignLeftRightCenter,
			Vector3 scale) {
		myGUI = mG;
//		containerID = cID;
		background = backgr;
		text.Initialize(mG, t, size, cID, textMargin, alignLeftRightCenter);

//		Vector3 textSize = text.GetSize();
		background.transform.localScale = scale;
		background.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		background.transform.parent = transform;
	}
	
	public void Initialize(MyGUI mG, string t, int cID, Transform backgr, float textMargin, float size, MyGUI.GUIAlignment alignLeftRightCenter) {
		myGUI = mG;
//		containerID = cID;
		background = backgr;
		text.Initialize(mG, t, size, cID, textMargin, alignLeftRightCenter);

		Vector3 textSize = text.GetSize();
		background.transform.localScale = new Vector3(myGUI.containers[cID].GetSize().x, textSize.y + textMargin*2, 1.0f);
		background.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		background.transform.parent = transform;
	}
	
	// initialize as image label
	public void Initialize(MyGUI mG, string t, int cID, Transform backgr, Vector4 textMargin, float size, MyGUI.GUIAlignment alignLeftRightCenter,
			Transform imageT, float scaleImage) {
		myGUI = mG;
//		containerID = cID;
		background = backgr;
		labelImage = imageT;
		text.Initialize(mG, t, size, cID, textMargin, alignLeftRightCenter);
		
		Vector3 textSize = text.GetSize();
//		transform.localScale = new Vector3(myGUI.containers[cID].GetSize().x, textSize.y + textMargin.z + textMargin.w, 1.0f);
//		background.localScale = transform.localScale;
		background.localScale = new Vector3(myGUI.containers[cID].GetSize().x, textSize.y + textMargin.z + textMargin.w, 1.0f);
		background.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		background.parent = transform;
		
		labelImage.localScale *= scaleImage;
		labelImage.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1.0f);
		MyGUI.Align(labelImage.position, labelImage.lossyScale, background.lossyScale, labelImage, transform, MyGUI.GUIAlignment.Left, 0.01f, MyGUI.GUIAlignment.Top, 0.01f);
		labelImage.parent = transform;
	}
	
}


