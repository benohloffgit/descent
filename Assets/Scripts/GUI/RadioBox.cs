using System;
using UnityEngine;
using System.Collections.Generic;

public class RadioBox : MonoBehaviour {
	private MyGUI myGUI;
	private int containerID;
	private int id;
	private int activeRadioID;
	private Dictionary<int, RadioButton> radioButtons = new Dictionary<int, RadioButton>();
	
	public void Initialize(int aR) {
		activeRadioID = aR;
		radioButtons[activeRadioID].SwitchState(MyGUI.GUIState.On);
	}
	
	public void DeselectActive(int newActiveID) {
//		if (activeRadioID != newActiveID) {
			radioButtons[activeRadioID].SwitchState(MyGUI.GUIState.Off);
//		}
	}
	
	public void RegisterRadioButton(int radioID, RadioButton rB) {
		radioButtons.Add(radioID, rB);	
	}
}
