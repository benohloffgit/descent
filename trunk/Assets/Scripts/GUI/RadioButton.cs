using System;
using UnityEngine;

public class RadioButton : MonoBehaviour, Focussable {
	public int id = -1; // set for id based event handling
	
	private Vector4 uvMapOn;
	private Vector4 uvMapOff;
	
	private Transform meshOff;
	private Transform meshOn;
	private Renderer myRendererOff;
	private Renderer myRendererOn;
//	private Game g;
	private MyGUI myGUI;
	private MyGUI.GUIState state;
	private Radio radio;
		
	void Awake() {
		meshOff = transform.Find("MeshOff");
		meshOn = transform.Find("MeshOn");
		myRendererOff = meshOff.renderer;
		myRendererOn = meshOn.renderer;
		id = gameObject.GetInstanceID();
	}
		
	public void Initialize(MyGUI mG, Radio r, int textureID, Vector4 uvMapRadioButtonOn, Vector4 uvMapRadioButtonOff, Vector3 scale) {
		myGUI = mG;
		radio = r;
		transform.localScale = scale;
		myRendererOff.material = mG.textureAtlas[textureID];
		myRendererOn.material = mG.textureAtlas[textureID];
		uvMapOn = uvMapRadioButtonOn;
		uvMapOff = uvMapRadioButtonOff;
		Mesh3D.SetUVMapping((meshOff.GetComponent<MeshFilter>()).mesh, new Vector2(uvMapOff.x, uvMapOff.y), new Vector2(uvMapOff.z, uvMapOff.w));
		Mesh3D.SetUVMapping((meshOn.GetComponent<MeshFilter>()).mesh, new Vector2(uvMapOn.x, uvMapOn.y), new Vector2(uvMapOn.z, uvMapOn.w));
		SwitchState(MyGUI.GUIState.Off);
	}
	
	// called from animation
	public void Execute(float scale) {
		meshOn.localScale = new Vector3(scale,scale,scale);
		LostFocus();
		radio.EndSelect();
	}
	
	public Vector3 GetSize() {
		return new Vector3(myRendererOff.bounds.size.x, myRendererOff.bounds.size.y, 1.0f);
	}
	
	public void LostFocus() {
		myGUI.DeleteGUIInFocus();
	}
		
	public void Select() {
		radio.BeginSelect();
		if (state == MyGUI.GUIState.Off) {
			SwitchState(MyGUI.GUIState.On);
		} else {
			SwitchState(MyGUI.GUIState.Off);
		}
		myGUI.SetGUIInFocus(this);
		animation.Play("RadioOn");
		//g.state.PlaySound(7);	
	}
		
	public bool IsBlocking() {
		return true;
	}

	public bool IsSameAs(GameObject gO) {
		return gO == gameObject;
	}

	public void SwitchState(MyGUI.GUIState s) {
		state = s;
		if (s == MyGUI.GUIState.Off) {
			meshOn.renderer.enabled = false;
		} else {
			meshOn.renderer.enabled = true;
		}
	}
}
