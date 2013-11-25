using System;
using UnityEngine;

public class LabelButton : MonoBehaviour, Focussable {
	
	public int id = -1; // set for id based event handling
	
	private NinePatch ninePatch;
	private Transform mesh;
	private Renderer backgroundRenderer;
//	private Renderer textRenderer;
	private TextCC text;
	private MyGUI myGUI;
	
	public TouchDelegate touchDelegate;
	
	void Awake() {
		text = GetComponentInChildren<TextCC>();
//		textRenderer = text.renderer;
		id = gameObject.GetInstanceID();
	}
		
	public void Initialize(MyGUI mG, TouchDelegate tD, int containerID,
			string text_, float textMargin, float size, MyGUI.GUIAlignment alignLeftRightCenter, int textureIDText,
			int textureIDBackground, Vector4 uvMapBackground,
			Vector3 scale) {
		myGUI = mG;
		touchDelegate = tD;
		
		mesh = transform.Find("NinePatch");
		ninePatch = mesh.GetComponent<NinePatch>();
		ninePatch.Initialize(uvMapBackground);
		backgroundRenderer = mesh.renderer;
		backgroundRenderer.material = mG.textureAtlas[textureIDBackground];
//		Mesh3D.SetUVMapping((mesh.GetComponent<MeshFilter>()).mesh, new Vector2(uvMapBackground.x, uvMapBackground.y), new Vector2(uvMapBackground.z, uvMapBackground.w));

		text.Initialize(mG, text_, size, containerID, textMargin, alignLeftRightCenter, textureIDText);
		text.transform.localScale = scale;
		mesh.localScale = text.GetSize();
	}
	
	public void InitializeF(MyGUI mG, TouchDelegate tD, int containerID,
			float yScale, string text_, float textMargin, float size, MyGUI.GUIAlignment alignLeftRightCenter, int textureIDText,
			int textureIDBackground, Vector4 uvMapBackground,
			Vector3 scale) {
		myGUI = mG;
		touchDelegate = tD;
		mesh = transform.Find("Mesh");
		backgroundRenderer = mesh.renderer;
		backgroundRenderer.material = mG.textureAtlas[textureIDBackground];
		Mesh3D.SetUVMapping((mesh.GetComponent<MeshFilter>()).mesh, new Vector2(uvMapBackground.x, uvMapBackground.y), new Vector2(uvMapBackground.z, uvMapBackground.w));

		text.Initialize(mG, text_, size, containerID, textMargin, alignLeftRightCenter, textureIDText);
		text.transform.localScale = scale;
		mesh.localScale = new Vector3(text.GetSize().y*0.75f*(1f/yScale), text.GetSize().y*0.75f, 1f);
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
//		myGUI.SetGUIInFocus(this);
		myGUI.game.state.PlaySound(7);
//		LostFocus();
		touchDelegate();
	}

	public void Hover() {
		myGUI.SetGUIInFocus(this, transform.position, mesh.localScale);
//		Debug.Log (mesh.lossyScale.x + " " + mesh.lossyScale.y);
//		Debug.Log (GetSize().x + " " +GetSize().y);
	}

	public Vector3 GetSize() {
		return new Vector3(backgroundRenderer.bounds.size.x, backgroundRenderer.bounds.size.y, 1.0f);
	}
	
	public bool IsBlocking() {
		return false;
	}

	public bool IsSameAs(GameObject gO) {
		return gO == gameObject;
	}

	public void LostFocus() {
		myGUI.DeleteGUIInFocus();
	}
}

