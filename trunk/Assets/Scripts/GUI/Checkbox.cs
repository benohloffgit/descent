using System;
using UnityEngine;

public delegate void CheckboxDelegate(MyGUI.GUIState selectState);
	
public class Checkbox : MonoBehaviour {
	private MyGUI myGUI;
	private int containerID;
	
	private Transform background;
	private Transform checkMark;
	
	private MyGUI.GUIState selectState;
	
	public CheckboxDelegate checkboxDelegate;
	
	void Awake() {
		background = transform;
		checkMark = transform.Find("Check Mark");
	}

	public void Initialize(MyGUI myGUI_, CheckboxDelegate checkboxDelegate_, Vector3 size, MyGUI.GUIState selectState_,
		Vector4 textureBackgroundScaleOffset, int textureIDBackground, Vector4 textureCheckMarkScaleOffset, int textureIDCheckMark
		) {
		myGUI = myGUI_;
		checkboxDelegate = checkboxDelegate_;
		SetSelectState(selectState_);
		background.renderer.material = myGUI.textureAtlas[textureIDBackground];
		background.renderer.material.mainTextureOffset = new Vector2(textureBackgroundScaleOffset.x, textureBackgroundScaleOffset.y);
		background.renderer.material.mainTextureScale = new Vector2(textureBackgroundScaleOffset.z, textureBackgroundScaleOffset.w);
		checkMark.renderer.material = myGUI.textureAtlas[textureIDCheckMark];
		checkMark.renderer.material.mainTextureOffset = new Vector2(textureCheckMarkScaleOffset.x, textureCheckMarkScaleOffset.y);
		checkMark.renderer.material.mainTextureScale = new Vector2(textureCheckMarkScaleOffset.z, textureCheckMarkScaleOffset.w);
		
		background.localScale = size;
	}	

	public void Select() {
		if (selectState == MyGUI.GUIState.On) {
			SetSelectState(MyGUI.GUIState.Off);
		} else {
			SetSelectState(MyGUI.GUIState.On);
		}
		checkboxDelegate(selectState);
	}
	
	private void SetSelectState(MyGUI.GUIState selectState_) {
		selectState = selectState_;
		if (selectState == MyGUI.GUIState.On) {
			checkMark.renderer.enabled = true;
		} else {
			checkMark.renderer.enabled = false;
		}
	}

	public Vector3 GetSize() {
		return transform.lossyScale;
	}

}

