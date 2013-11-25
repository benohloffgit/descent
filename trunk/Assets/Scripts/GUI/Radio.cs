using System;
using UnityEngine;

public class Radio : MonoBehaviour {
	private MyGUI myGUI;
//	private int containerID;
	private Transform background;
	private RadioButton button;
	private TextCC text;
	private DropdownDelegate dropdownSelect;
	private int radioID;
	private RadioBox radioBox;
	
	void Awake() {
		text = GetComponentInChildren<TextCC>();
		button = GetComponentInChildren<RadioButton>();
	}

	public void Initialize(MyGUI mG, string t, int cID, Transform backgr, float textMargin, float size, MyGUI.GUIAlignment alignLeftRightCenter,
				DropdownDelegate dS, int rID, Vector4 uvMapRadioButtonOn, Vector4 uvMapRadioButtonOff, int textureIDText, int textureIDButton, RadioBox rB) {
		myGUI = mG;
		dropdownSelect = dS;
		radioID = rID;
		radioBox = rB;
//		containerID = cID;
		background = backgr;
		text.Initialize(mG, t, size, cID, textMargin, alignLeftRightCenter, true, textureIDText);

		Vector3 textSize = text.GetSize();
		background.transform.localScale = new Vector3(myGUI.containers[cID].GetSize().x, textSize.y + textMargin*2, 1.0f);
		background.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		background.transform.parent = transform;

		button.Initialize(myGUI, this, textureIDButton, uvMapRadioButtonOn, uvMapRadioButtonOff, new Vector3(textSize.y, textSize.y, 1.0f));
		MyGUI.Align(button.transform.position, button.GetSize(), myGUI.containers[cID].GetSize(), button.transform, transform, MyGUI.GUIAlignment.Right, 0.025f, MyGUI.GUIAlignment.Center, 0f);
		
		radioBox.RegisterRadioButton(radioID, button);
	}	
			
	public Vector3 GetSize() {
		return background.renderer.bounds.size;
	}

	public void EndSelect() {
		dropdownSelect(radioID);
	}
	
	public void BeginSelect() {
		radioBox.DeselectActive(radioID);
	}
}
