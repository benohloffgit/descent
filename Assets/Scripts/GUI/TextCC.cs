using System;
using UnityEngine;

public class TextCC : MonoBehaviour {
	public CCText textMesh; // http://catlikecoding.com/unity/documentation/
	private Renderer myRenderer;
	private MyGUI myGUI;
	private MyGUI.GUIAlignment alignLeftRightCenter;
	private Vector4 textMargin;
	private int containerID;
	private bool noBreak;
	
	private static float MAX_OVERSIZE = 0.725f;
	
	void Awake() {
		textMesh = GetComponent<CCText>();
		myRenderer = GetComponent<Renderer>();
	}

	public void Initialize(MyGUI mG, string text, float size, int cID, Vector4 tM, MyGUI.GUIAlignment align) {
		Initialize (mG, text, size, cID, tM, align, false);
	}
	
	public void Initialize(MyGUI mG, string text, float size, int cID, Vector4 tM, MyGUI.GUIAlignment align, bool noBr) {
		myGUI = mG;
		textMargin = tM;
		containerID = cID;
		alignLeftRightCenter = align;
		noBreak = noBr;
		transform.localScale *= size;
		textMesh.renderer.material = mG.activeTextMaterial;
		textMesh.Font = mG.activeFont;
		SetText(text);
	}
	
	public void Initialize(MyGUI mG, string text, float size, int cID, float tM, MyGUI.GUIAlignment align, bool noBr) {
		Initialize(mG, text, size, cID, new Vector4(tM, tM, tM, tM), align, noBr);
	}
	
	public void Initialize(MyGUI mG, string text, float size, int cID, float tM, MyGUI.GUIAlignment align) {
		Initialize(mG, text, size, cID, new Vector4(tM, tM, tM, tM), align);
	}
	
	public Vector3 GetSize() {
		return myRenderer.bounds.size;
	}
	
	public void SetText(string t) {
		textMesh.Bounding = CCText.BoundingMode.Margin;
		textMesh.Text = t;
		// shorten string if oversized
		float textWidth = textMesh.lastCaret.x;
		if (textMesh.LineCount > 1) {
			textWidth += (textMesh.LineCount-1)*myGUI.containers[containerID].GetSize().x;
		}
		float oversize = textWidth / myGUI.containers[containerID].GetSize().x;
//		float oversize = GetSize().x / myGUI.containers[containerID].GetSize().x;
		if (noBreak && oversize > MAX_OVERSIZE) {
//		if (!t.Contains("\n") && oversize > MAX_OVERSIZE) {
			ShortenText(oversize);
		}
		Vector3 containerPos = myGUI.containers[containerID].transform.position;
		if (alignLeftRightCenter == MyGUI.GUIAlignment.Left) {
			transform.position = new Vector3(containerPos.x - (myGUI.containers[containerID].GetSize().x/2.0f - GetSize().x/2.0f) + textMargin.x, transform.position.y, transform.position.z);
			textMesh.Alignment = CCText.AlignmentMode.Left;
		} else if (alignLeftRightCenter == MyGUI.GUIAlignment.Right) {
			transform.position = new Vector3(containerPos.x + (myGUI.containers[containerID].GetSize().x/2.0f - GetSize().x/2.0f) - textMargin.y, transform.position.y, transform.position.z);
			textMesh.Alignment = CCText.AlignmentMode.Right;
		}
		textMesh.Bounding = CCText.BoundingMode.Caret;
		//Debug.Log ("size " + myGUI.containers[containerID].GetSize().x + " textsize " + GetSize().x);
	}
	
	public string GetText() {
		return textMesh.Text;
	}
	
	private void ShortenText(float oversize) {
		int newLength = Mathf.RoundToInt( (textMesh.Text.Length / oversize) * MAX_OVERSIZE );
//		Debug.Log ("lineWidth " + textMesh.LineWidth + " newLength " + newLength + " old length " + textMesh.Text.Length + " " + oversize);
		textMesh.Text = textMesh.Text.Substring(0,newLength) + "...";
	}
}


