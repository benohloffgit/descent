using System;
using UnityEngine;
using System.Collections;

public class TextInput : MonoBehaviour, Focussable {
	private TextCC text;
	private Transform backgroundOff;
	private Transform backgroundOn;
	private Transform cursor;
	private Transform cursorMesh;
	private MyGUI.Focus focus;
	private Cursor cursorStatus;
	private Button editButton;
	private MeshRenderer editButtonMeshRenderer;
	private BoxCollider editButtonMeshCollider;
	private int cursorPos;
	private string inputString;
	private string cachedInputString;
	private int maxLength;
	private MyGUI myGUI;
	private int containerID;
	private float backgroundLeft;
	private float textMargin;
#if UNITY_ANDROID || UNITY_IPHONE
	private TouchScreenKeyboard mobileKeyboard;
#endif
	private bool isInMobileKeyboardMode;
	private TextInputDelegate textInputUpdated;
	
	public enum Cursor { Off=0, On=1, Waiting=2 }

	void Awake() {
		text = GetComponentInChildren<TextCC>();
		cursor = transform.Find("Cursor");
		cursorMesh = cursor.Find ("Mesh");
		editButton = GetComponentInChildren<Button>();
		editButtonMeshRenderer = editButton.GetComponentInChildren<MeshRenderer>();
		editButtonMeshCollider = editButton.GetComponentInChildren<BoxCollider>();
		focus = MyGUI.Focus.NoFocus;
		isInMobileKeyboardMode = false;
	}

	public void Initialize(MyGUI mG, string t, int cID, Transform backgrOff, Transform backgrOn, float tM, float size, MyGUI.GUIAlignment alignLeftRightCenter,
						int mL, int textureID, Vector4 uvMapCursor, int textureIDText, int textureIDButton, Vector4 uvMapButton, TextInputDelegate tIU) {
		myGUI = mG;
		containerID = cID;
		backgroundOff = backgrOff;
		backgroundOn = backgrOn;
		textInputUpdated = tIU;
		maxLength = mL;
		inputString = t;
		textMargin = tM;
		text.Initialize(mG, t, size, cID, textMargin, alignLeftRightCenter, textureIDText);

		Vector3 textSize = text.GetSize();
		backgroundOff.transform.localScale = new Vector3(myGUI.containers[cID].GetSize().x, textSize.y + textMargin*2, 1.0f);
		backgroundOff.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		backgroundLeft = backgroundOff.transform.position.x - backgroundOff.transform.localScale.x/2.0f;
		backgroundOff.transform.parent = transform;

		backgroundOn.transform.localScale = new Vector3(myGUI.containers[cID].GetSize().x, textSize.y + textMargin*2, 1.0f);
		backgroundOn.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		backgroundOn.transform.parent = transform;
		backgroundOn.renderer.enabled = false;
		
//		SetTextPosition();
		float cursorSizeFactor = text.GetSize().y / cursor.localScale.y;
		cursor.localScale *= cursorSizeFactor;
		cursorPos = inputString.Length;
		SetCursorPosition();
		Mesh3D.SetUVMapping((cursorMesh.GetComponent<MeshFilter>()).mesh, uvMapCursor);
		cursorMesh.renderer.material = myGUI.textureAtlas[textureID];
		cursorMesh.renderer.enabled = false;
		cursorStatus = Cursor.Off;

		TouchDelegate buttonExecute = new TouchDelegate(Edit);
		editButton.Initialize(myGUI, buttonExecute, textureIDButton, uvMapButton, new Vector3(textSize.y, textSize.y, 1.0f));
		MyGUI.Align(editButton.transform.position, editButton.GetSize(), myGUI.containers[cID].GetSize(), editButton.transform, transform, MyGUI.GUIAlignment.Right, 0.025f, MyGUI.GUIAlignment.Center, 0f);
	}
		
	void Update() {
		if (cursorStatus == Cursor.Waiting) {
			StartCoroutine("BlinkCursor");
		}
		
#if UNITY_ANDROID || UNITY_IPHONE
		if (isInMobileKeyboardMode) {
			if (mobileKeyboard.done) {
				text.SetText(mobileKeyboard.text);
				Deselect();
				isInMobileKeyboardMode = false;
			}
		}
#endif
//		if (cursorStatus != Cursor.Off) {
//			if (Input.anyKeyDown) {
//				if (Input.GetKeyDown(KeyCode.Alpha0)) {
//					Debug.Log ("0");
//				}
// there seems to be a bug with Input.inputString, it is always empty
/*				Debug.Log ("string: " + Input.inputString + " " + System.Convert.ToInt32(Input.inputString[0]));
				foreach (char c in Input.inputString) {
				Debug.Log ("char: " + System.Convert.ToInt32(c));
					if (Char.IsDigit(c)) {
						Debug.Log (c);
					}
				}*/
//			}
//		}
	}
	
	void OnGUI() {
		if (cursorStatus != Cursor.Off) {
	        Event e = Event.current;
	        if (e.isKey && e.type == EventType.KeyDown) {
				if (e.keyCode == KeyCode.Backspace && inputString != "" && cursorPos > 0) {
					cursorPos--;
					inputString = inputString.Substring(0, cursorPos) + inputString.Substring(cursorPos + 1);
					MoveCursor ();
				} else if (e.keyCode == KeyCode.Delete && cursorPos < inputString.Length) {
					inputString = inputString.Substring(0, cursorPos) + inputString.Substring(cursorPos + 1);
					MoveCursor ();
				} else if (e.keyCode == KeyCode.Return && inputString != "") {	
					Deselect();
				} else if (e.keyCode == KeyCode.LeftArrow && cursorPos > 0) {
					cursorPos--;
					MoveCursor();
				} else if (e.keyCode == KeyCode.RightArrow && cursorPos < inputString.Length) {	
					cursorPos++;
					MoveCursor();
				} else if (inputString.Length < maxLength && Char.IsDigit(e.character)) {
					inputString += e.character;
					cursorPos++;
					MoveCursor ();
					//Debug.Log("c: " + e.character);
				}
			}
			e.Use();
		}
    }
	
	private void Edit() {
		Select();
	}

	public TextCC GetText() {
		return text;
	}
	
	public Vector3 GetSize() {
		return backgroundOff.renderer.bounds.size;
	}
	
	private void SetTextPosition() {
//		Debug.Log ("backgroundLeft " + backgroundLeft + " text.GetSize().x " + text.GetSize().x);
		text.transform.position = new Vector3(backgroundLeft + (text.GetSize().x/2) + textMargin, backgroundOff.transform.position.y, text.transform.position.z );
	}
	
	private void SetCursorPosition() {
//		Debug.Log (text.textMesh.lastCaret.x);
//		Debug.Log (text.textMesh.mesh.bounds.size +" " +text.textMesh.Width + " " + text.textMesh.minBounds.x + " " + text.textMesh.maxBounds.x + " " +  text.GetSize().x + " " + text.textMesh.CaretMinBounds.x + " " + text.textMesh.CaretMaxBounds.x + " " + text.textMesh.LineWidth);
		cursor.position = new Vector3(text.textMesh.lastCaret.x, text.transform.position.y, cursor.position.z);
//		cursor.position = new Vector3(text.transform.position.x + text.GetSize().x/2.0f, text.transform.position.y, cursor.position.z);
	}
	
	public void Select() {
		if (focus == MyGUI.Focus.NoFocus) {
			focus = MyGUI.Focus.Focus;
			cursorMesh.renderer.enabled = true;
			backgroundOn.renderer.enabled = true;
			backgroundOff.renderer.enabled = false;
			editButtonMeshRenderer.renderer.enabled = false;
			editButtonMeshCollider.enabled = false;
			cursorStatus = Cursor.Waiting;
			myGUI.SetGUIInFocus(this);
			cursorPos = inputString.Length;
			SetCursorPosition();
			cachedInputString = inputString;
#if UNITY_ANDROID || UNITY_IPHONE
			if (myGUI.gameInput.isMobile) {
				isInMobileKeyboardMode = true;
				mobileKeyboard = TouchScreenKeyboard.Open(inputString, TouchScreenKeyboardType.NumberPad, false, false, false, false, "");
			}
#endif
		}
	}
	
	private void Deselect() {
		focus = MyGUI.Focus.NoFocus;
		cursorMesh.renderer.enabled = false;
		backgroundOn.renderer.enabled = false;
		backgroundOff.renderer.enabled = true;
		editButtonMeshRenderer.renderer.enabled = true;
		editButtonMeshCollider.enabled = true;
		cursorStatus = Cursor.Off;
		if (text.GetText() == "") {
			UpdateTextInput(cachedInputString);
			cursorPos = cachedInputString.Length;
			SetCursorPosition();
		}
		inputString = text.GetText();
		textInputUpdated(inputString);
		StopCoroutine("BlinkCursor");
		myGUI.DeleteGUIInFocus();
	}		
	
	private IEnumerator BlinkCursor() {
		cursorStatus = Cursor.On;
		yield return new WaitForSeconds(0.5f);
		if (cursorMesh.renderer.enabled) {
			cursorMesh.renderer.enabled = false;
		} else {
			cursorMesh.renderer.enabled = true;
		}
		cursorStatus = Cursor.Waiting;
	}
	
	public void UpdateTextInput(string newText) {
		text.SetText(newText);
//		SetTextPosition();
	}
	
	private void MoveCursor() {
		UpdateTextInput(inputString.Substring(0, cursorPos));
		SetCursorPosition();
		UpdateTextInput(inputString);
		cursorMesh.renderer.enabled = true;
		cursorStatus = Cursor.On;
		StopCoroutine("BlinkCursor");
		StartCoroutine("BlinkCursor");
	}
	
	public void LostFocus() {
		Deselect();
	}
	
	public bool IsBlocking() {
		return false;
	}
}

