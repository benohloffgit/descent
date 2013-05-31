using System;
using UnityEngine;

public class LabelButton : MonoBehaviour, Focussable {
	
	public int id = -1; // set for id based event handling
		
	private Transform mesh;
	private Renderer backgroundRenderer;
	private Renderer textRenderer;
	private TextCC text;
	private MyGUI myGUI;
	
	public TouchDelegate touchDelegate;
	
	void Awake() {
		mesh = transform.Find("Mesh");
		text = GetComponentInChildren<TextCC>();
		backgroundRenderer = mesh.renderer;
		textRenderer = text.renderer;
		id = gameObject.GetInstanceID();
	}
		
	public void Initialize(MyGUI mG, TouchDelegate tD, int containerID,
			string text_, float textMargin, float size, MyGUI.GUIAlignment alignLeftRightCenter, int textureIDText,
			int textureIDBackground, Vector4 uvMapBackground,
			Vector3 scale) {
		myGUI = mG;
		touchDelegate = tD;
		backgroundRenderer.material = mG.textureAtlas[textureIDBackground];
		Mesh3D.SetUVMapping((mesh.GetComponent<MeshFilter>()).mesh, new Vector2(uvMapBackground.x, uvMapBackground.y), new Vector2(uvMapBackground.z, uvMapBackground.w));

		text.Initialize(mG, text_, size, containerID, textMargin, alignLeftRightCenter, textureIDText);
		text.transform.localScale = scale;
		mesh.localScale = text.GetSize();
	}

	public TextCC GetText() {
		return text;
	}
	
	public void SetText(string t) {
		text.SetText(t);
	}

/*	public void SetScale(float scale) {
		mesh.localScale = new Vector3(scale,scale,scale);
		LostFocus();
		touchDelegate();
	}*/
	
	public void Select() {
		myGUI.SetGUIInFocus(this);
		myGUI.game.state.PlaySound(7);
		LostFocus();
		touchDelegate();
	}

	public Vector3 GetSize() {
		return new Vector3(backgroundRenderer.bounds.size.x, backgroundRenderer.bounds.size.y, 1.0f);
	}
	
	public bool IsBlocking() {
		return true;
	}
	
	public void LostFocus() {
		myGUI.DeleteGUIInFocus();
	}
}

