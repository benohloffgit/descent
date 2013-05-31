using System;
using UnityEngine;

public delegate void DropdownDelegate(int id);

public class Dropdown : MonoBehaviour, Focussable {
	public GameObject textPrefab;
	
	private MyGUI myGUI;
	private int containerID;
	private Transform background;
	private ImageButton openButton;
	private TextCC text;
	private int openedBox;
	private string[] options;
	private float border;
	private DropdownDelegate dropdownExecute;
	private DropdownDelegate dropdownSelect;
	private OpenRadioBoxDelegate openDropdown;
	private MyGUI.GUIBackground backgroundRadioBoxLabel;
	private Vector4 uvMapRadioBoxLabel;
		
	private static float DEFAULT_WIDTH = 0.85f;
	
	void Awake() {
		text = GetComponentInChildren<TextCC>();
		openButton = GetComponentInChildren<ImageButton>();
		dropdownSelect = new DropdownDelegate(SelectDropdown);
	}
	
	public void Initialize(MyGUI mG, string[] os, int selectedOption, int cID, Transform backgr, float textMargin, float size, MyGUI.GUIAlignment alignLeftRightCenter, DropdownDelegate dE,
							OpenRadioBoxDelegate oD, int textureIDText, int textureIDButton, Vector4 uvMapDropdownOpenButton) {
		myGUI = mG;
		dropdownExecute = dE;
		options = os;
		openDropdown = oD;
		containerID = cID;
		background = backgr;
		text.Initialize(mG, os[selectedOption], size, cID, textMargin, alignLeftRightCenter, true, textureIDText);

		Vector3 textSize = text.GetSize();
		background.localScale = new Vector3(myGUI.containers[cID].GetSize().x, textSize.y + textMargin*2, 1.0f);
		background.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		background.parent = transform;
		
		TouchDelegate buttonExecute = new TouchDelegate(Open);
		openButton.Initialize(myGUI, buttonExecute, textureIDButton, uvMapDropdownOpenButton, new Vector3(textSize.y, textSize.y, 1.0f));
		MyGUI.Align(openButton.transform.position, openButton.GetSize(), myGUI.containers[cID].GetSize(), openButton.transform, transform, MyGUI.GUIAlignment.Right, 0.025f, MyGUI.GUIAlignment.Center, 0f);
	}
		
	public Vector3 GetSize() {
		return background.renderer.bounds.size;
	}
	
	public Vector3 GetTextSize() {
		return text.GetSize();
	}

	public void SelectDropdown(int id) {
		text.SetText(options[id]);
		Close();
		dropdownExecute(id);
	}
	
	private void Open() {
		openedBox = myGUI.AddContainer(containerID, myGUI.GetSize(), new Vector3(myGUI.GetCenter().x, myGUI.GetCenter().y, myGUI.containers[containerID].transform.position.z-2f), true);
		TouchDelegate dimTouch = new TouchDelegate(DimTouch);
		int dim = 0; // TODO !!!!!!!!!!!!!!!!!!! myGUI.AddDim(openedBox, dimTouch);
		Vector3 openedBoxCenter = myGUI.containers[openedBox].GetCenter();
		int radioContainer = myGUI.AddContainer(openedBox, new Vector3(myGUI.GetSize().x * DEFAULT_WIDTH, myGUI.GetSize().y * 0.5f, 1.0f), new Vector3(openedBoxCenter.x, openedBoxCenter.y + 0.3f, openedBoxCenter.z-2f), false);
		RadioBox rB = myGUI.containers[radioContainer].gameObject.AddComponent("RadioBox") as RadioBox;
		//Callback
		int activeRadioID = openDropdown(radioContainer, dropdownSelect, rB);
		rB.Initialize(activeRadioID);
		myGUI.SetGameInputZLevel(myGUI.dims[dim].transform.position.z);
	}
	
	private void Close() {
		Destroy(myGUI.containers[openedBox].gameObject);
		myGUI.ResetGameInputZLevel();
		myGUI.DeleteGUIInFocus();
	}

	public void LostFocus() {
		Close();
	}
	
	public bool IsBlocking() {
		return false;
	}
	
	public void DimTouch() {
		LostFocus();
	}
}
