using System;
using UnityEngine;

public class Text : MonoBehaviour {
	private TextMesh textMesh;
	private Renderer myRenderer;
	private MyGUI myGUI;
	private MyGUI.GUIAlignment alignLeftRightCenter;
	private Vector4 textMargin;
	private int containerID;
	
	private static float MAX_OVERSIZE = 0.7f;
	
	void Awake() {
		textMesh = GetComponent<TextMesh>();
		myRenderer = GetComponent<Renderer>();
	}

	public void Initialize(MyGUI mG, string text, float size, int cID, Vector4 tM, MyGUI.GUIAlignment align) {
		myGUI = mG;
		textMargin = tM;
		containerID = cID;
		alignLeftRightCenter = align;
		transform.localScale *= size;
		SetText(text);
	}
	
	public void Initialize(MyGUI mG, string text, float size, int cID, float tM, MyGUI.GUIAlignment align) {
		Initialize(mG, text, size, cID, new Vector4(tM, tM, tM, tM), align);
	}
	
/*	public void SetProperties(string text, TextAlignment alignment, float size) {
		textMesh.text = text;
		textMesh.alignment = alignment;
		transform.localScale *= size;
	}*/
	
	public Vector3 GetSize() {
		return myRenderer.bounds.size;
	}
	
/*	void OnDrawGizmos() {
    	Gizmos.color = Color.white;
//		Debug.Log (renderer.bounds.size.x + " " + renderer.bounds.size.y);
		Gizmos.DrawWireCube(myRenderer.bounds.center, myRenderer.bounds.size);
	}*/
	
	public void SetText(string t) {
		textMesh.text = t;
		// shorten string if oversized
		float oversize = GetSize().x / myGUI.containers[containerID].GetSize().x;
		if (!t.Contains("\n") && oversize > MAX_OVERSIZE) {
			ShortenText(oversize);
		}
		Vector3 containerPos = myGUI.containers[containerID].transform.position;
		if (alignLeftRightCenter == MyGUI.GUIAlignment.Left) {
			transform.position = new Vector3(containerPos.x - (myGUI.containers[containerID].GetSize().x/2.0f - GetSize().x/2.0f) + textMargin.x, transform.position.y, transform.position.z);
			textMesh.alignment = TextAlignment.Left;
		} else if (alignLeftRightCenter == MyGUI.GUIAlignment.Right) {
			transform.position = new Vector3(containerPos.x + (myGUI.containers[containerID].GetSize().x/2.0f - GetSize().x/2.0f) - textMargin.y, transform.position.y, transform.position.z);
			textMesh.alignment = TextAlignment.Right;
		}
		//Debug.Log ("size " + myGUI.containers[containerID].GetSize().x + " textsize " + GetSize().x);
	}
	
	public string GetText() {
		return textMesh.text;
	}
	
	private void ShortenText(float oversize) {
		int newLength = Mathf.RoundToInt( (textMesh.text.Length / oversize) * MAX_OVERSIZE );
//		Debug.Log ("newLength " + newLength);
		textMesh.text = textMesh.text.Substring (0,newLength) + "...";
	}
}

