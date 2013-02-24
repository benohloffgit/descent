using System;
using UnityEngine;
using System.Collections.Generic;

public delegate void TouchDelegate();
public delegate void TextInputDelegate(string t);
public delegate int OpenRadioBoxDelegate(int containerID, DropdownDelegate dropdownSelect, RadioBox rB);

public class MyGUI : MonoBehaviour {
	public GameObject containerPrefab;
	public GameObject labelPrefab;
	public GameObject labelBitmapPrefab;
	public GameObject dropdownPrefab;
	public GameObject ninePatchPrefab;
	public GameObject quadPrefab;
	public GameObject buttonPrefab;
	public GameObject imagePrefab;
	public GameObject textPrefab;
	public GameObject dimPrefab;
	public GameObject radioPrefab;
	public GameObject textInputPrefab;
	
	public Texture2D[] fontTextures;
	public CCFont[] bitmapFonts;
	public Material[] textureAtlas;
	
	public CCFont activeFont;
	public Material activeTextMaterial;
	
	private Game game;
	public GameInput gameInput;
	
	public Dictionary<int, Container> containers;
	public Dictionary<int, Label> labels;
	public Dictionary<int, LabelCC> labelsCC;
	public Dictionary<int, TextInput> textInputs;
	public Dictionary<int, Dropdown> dropdowns;
	public Dictionary<int, Dim> dims;
	public Dictionary<int, Button> buttons;
	public Dictionary<int, Image> images;
	
	public enum Focus { NoFocus=0, Focus=1 }	
	public enum GUIAlignment { Left=0, Right=1, Center=2, Top=3, Bottom=4 }
	public enum GUIBackground { NinePatch=0, Quad=1, NinePatchWithCollider=2, QuadWithCollider=3 }
	public enum GUIState { Off=0, On=1 }	

//	private NinePatch background;
	private Focussable guiElementInFocus;
	private bool isGUIElementInFocus; // another gui element, not our MyGUI
	
	private float zLevel;
		
	void Awake() {
		containers = new Dictionary<int, Container>();
		labels = new Dictionary<int, Label>();
		labelsCC = new Dictionary<int, LabelCC>();
		textInputs = new Dictionary<int, TextInput>();
		dropdowns = new Dictionary<int, Dropdown>();
		dims = new Dictionary<int, Dim>();
		buttons = new Dictionary<int, Button>();
		images = new Dictionary<int, Image>();
		
//		background = GetComponent<NinePatch>();
		
		zLevel = transform.position.z;
		
		isGUIElementInFocus = false;
	}
	
	void Start() {
//		Game game =  GameObject.Find("/Game(Clone)").GetComponent<Game>();
//		gameInput = game.gameInput;
//		gameInput.RegisterGUI(this);
	}
	
/*	public void Restart() {
	}*/
	
	public void Initialize(Game g, GameInput input) {
		game = g;
		gameInput = input;
		gameInput.RegisterGUI(this);
		activeTextMaterial = GetTextMaterial();
		activeFont = GetBitmapFont();
	}
	
	// as child of this GUI
	public int AddContainer() {
		zLevel -= 2.0f;
		Container c = (GameObject.Instantiate(containerPrefab, new Vector3(transform.position.x, transform.position.y, zLevel), Quaternion.identity) as GameObject).GetComponent<Container>();
		c.Initialize(transform.localScale, true);
		containers.Add(c.gameObject.GetInstanceID(), c);
		c.transform.parent = transform;
		return c.gameObject.GetInstanceID();		
	}

	public int AddContainer(int containerID, Vector3 size, Vector3 pos, TouchDelegate tD, GUIBackground background, int textureIx, Vector4 uvMap, bool isFixedSize) {
		int cID = AddContainer(containerID, size, pos, isFixedSize);
		containers[cID].Initialize(this, containerID, tD, CreateBackground(background, textureIx, uvMap));
		return cID;
	}
	
	// as child of another container, with absolute position and zLevel
	public int AddContainer(int containerID, Vector3 size, Vector3 pos, bool isFixedSize) {
		Container c = (GameObject.Instantiate(containerPrefab, pos, Quaternion.identity) as GameObject).GetComponent<Container>();
		c.Initialize(size, isFixedSize);
		containers.Add(c.gameObject.GetInstanceID(), c);
		c.transform.parent = containers[containerID].transform;
		return c.gameObject.GetInstanceID();		
	}
	
	// as child of another container and absolute position
	public int AddContainer(int containerID, Vector3 size, Vector2 pos, bool isFixedSize) {
		zLevel = containers[containerID].transform.position.z - 2.0f;
		return AddContainer (containerID, size, new Vector3(pos.x, pos.y, zLevel), isFixedSize);
	}
	
	// as child and positioned relative
	public int AddContainer(int containerID, Vector3 scale, bool isFixedSize, GUIAlignment alignLeftRightCenter, float borderLeftRight,
					GUIAlignment alignTopBottomCenter, float borderTopBottom) {		
		Container c = (GameObject.Instantiate(containerPrefab) as GameObject).GetComponent<Container>();
		c.Initialize(scale, isFixedSize);
		containers.Add(c.gameObject.GetInstanceID(), c);
		containers[containerID].AddElement(c.transform, c.GetSize(), alignLeftRightCenter, borderLeftRight, alignTopBottomCenter, borderTopBottom);
		return c.gameObject.GetInstanceID();
	}
	
	public int AddContainerScrollable(int containerID, Vector3 size, Vector2 pos, GUIBackground background, int textureIx, Vector4 uvMap,
			int textureIDBlendTop, Vector4 uvMapBlendTop, int textureIDBlendBottom, Vector4 uvMapBlendBottom) {
		int cID = AddContainer(containerID, size, pos, false);
		containers[cID].InitializeAsScrollable(this, CreateBackground(background, textureIx, uvMap),
			CreateBackground(GUIBackground.Quad, textureIDBlendTop, uvMapBlendTop),
			CreateBackground(GUIBackground.Quad, textureIDBlendBottom, uvMapBlendBottom));
		return cID;
	}

	public int AddButton(int containerID, Vector3 scale, TouchDelegate tD, GUIAlignment alignLeftRightCenter, float borderLeftRight,
					GUIAlignment alignTopBottomCenter, float borderTopBottom, Vector4 uvMap, int textureID) {
		Button b = (GameObject.Instantiate(buttonPrefab) as GameObject).GetComponent<Button>();
		b.Initialize(this, tD, textureID, uvMap, scale);
		buttons.Add(b.gameObject.GetInstanceID(), b);
		containers[containerID].AddElement(b.transform, b.GetSize(), alignLeftRightCenter, borderLeftRight, alignTopBottomCenter, borderTopBottom);
		return b.gameObject.GetInstanceID();
	}	
	
	// scaled by bounding container
	public int AddImage(int containerID, GUIAlignment alignLeftRightCenter, float borderLeftRight,
					GUIAlignment alignTopBottomCenter, float borderTopBottom, Vector4 uvMap, int textureID) {
		Image i = (GameObject.Instantiate(imagePrefab) as GameObject).GetComponent<Image>();
		i.Initialize(this, textureID, uvMap, containers[containerID].GetSize());
		images.Add(i.gameObject.GetInstanceID(), i);
		containers[containerID].AddElement(i.transform, containers[containerID].GetSize(), alignLeftRightCenter, borderLeftRight, alignTopBottomCenter, borderTopBottom);
		return i.gameObject.GetInstanceID();
	}	
	
	// add floating
	public int AddLabel(string text, int containerID, GUIAlignment alignLeftRightCenter, GUIBackground background, float border, float textMargin,
					float size, int textureIx, Vector4 uvMap) {
//		if (text != " ") {
			LabelCC l;
			l = (GameObject.Instantiate(labelBitmapPrefab) as GameObject).GetComponent<LabelCC>();
			l.Initialize(this, text, containerID, CreateBackground(background, textureIx, uvMap), textMargin, size, alignLeftRightCenter);
			labelsCC.Add(l.gameObject.GetInstanceID(), l);
			containers[containerID].AddElement(l.transform, l.GetSize());
			return l.gameObject.GetInstanceID();
/*		} else { // specially for bottom line in preferences 
			Label l;
			l = (GameObject.Instantiate(labelPrefab) as GameObject).GetComponent<Label>();
			l.Initialize(this, text, containerID, CreateBackground(background, textureIx, uvMap), textMargin, size, alignLeftRightCenter);
			labels.Add(l.gameObject.GetInstanceID(), l);
			containers[containerID].AddElement(l.transform, l.GetSize());
			return l.gameObject.GetInstanceID();
		}*/
	}
	
	// add with position
	public int AddLabel(string text, int containerID, Vector3 scale, GUIAlignment alignLeftRightCenter, float borderLeftRight,
					GUIAlignment alignTopBottomCenter, float borderTopBottom, float textMargin, float size, GUIBackground background,
					Vector4 uvMap, int textureID) {
		LabelCC l;
		l = (GameObject.Instantiate(labelBitmapPrefab) as GameObject).GetComponent<LabelCC>();
		l.Initialize(this, text, containerID, CreateBackground(background, textureID, uvMap), textMargin, size, alignLeftRightCenter, scale);
		labelsCC.Add(l.gameObject.GetInstanceID(), l);
		containers[containerID].AddElement(l.transform, l.GetSize(), alignLeftRightCenter, borderLeftRight, alignTopBottomCenter, borderTopBottom);
		return l.gameObject.GetInstanceID();
	}

	// add floating
	public int AddImageLabel(string text, int containerID, GUIAlignment alignLeftRightCenter, GUIBackground background, float border, Vector4 textMargin,
					float size, int textureIx, Vector4 uvMap, int textureIdImage, Vector4 uvMapImage, float scaleImage) {
		LabelCC l;
		l = (GameObject.Instantiate(labelBitmapPrefab) as GameObject).GetComponent<LabelCC>();
		l.Initialize(this, text, containerID, CreateBackground(background, textureIx, uvMap), textMargin, size, alignLeftRightCenter,
			CreateBackground(GUIBackground.Quad, textureIdImage, uvMapImage), scaleImage);
		labelsCC.Add(l.gameObject.GetInstanceID(), l);
		containers[containerID].AddElement(l.transform, l.GetSize());
		return l.gameObject.GetInstanceID();
	}
	
	public int AddTextInput(string text, int containerID, GUIAlignment alignLeftRightCenter, GUIBackground background, float border, float textMargin,
					float size,
					int textureIDBackgr, Vector4 uvMap,
					int textureIDBackgrEdit, Vector4 uvMapEdit,
					int textureIDCursor, Vector4 uvMapCursor,
					int textureIDButton, Vector4 uvMapButton,
					int maxLength, TextInputDelegate textInputUpdated) {
		TextInput tI = (GameObject.Instantiate(textInputPrefab) as GameObject).GetComponent<TextInput>();
		tI.Initialize(this, text, containerID,
				CreateBackground(background, textureIDBackgr, uvMap),
				CreateBackground(background, textureIDBackgrEdit, uvMapEdit),
				textMargin, size, alignLeftRightCenter, maxLength, textureIDCursor, uvMapCursor, textureIDButton, uvMapButton, textInputUpdated);
		textInputs.Add(tI.gameObject.GetInstanceID(), tI);
		containers[containerID].AddElement(tI.transform, tI.GetSize());
		return tI.gameObject.GetInstanceID();
	}
	
	public int AddDropdown(string[] options, int selectedOption, int containerID, GUIAlignment alignLeftRightCenter, GUIBackground background, float border, float textMargin,
							float size, int textureID, Vector4 uvMap, DropdownDelegate dropdownSelect,
							OpenRadioBoxDelegate openDropdown, Vector4 uvMapDropdownOpenButton, int textureIDButton) {
		Dropdown d = (GameObject.Instantiate(dropdownPrefab) as GameObject).GetComponent<Dropdown>();
		d.Initialize(this, options, selectedOption, containerID, CreateBackground(background, textureID, uvMap),
				textMargin, size, alignLeftRightCenter, dropdownSelect, openDropdown, textureIDButton, uvMapDropdownOpenButton);
		dropdowns.Add(d.gameObject.GetInstanceID(), d);
		containers[containerID].AddElement(d.transform, d.GetSize());
		return d.gameObject.GetInstanceID();
	}
	
	public int AddRadio(string text, int containerID, GUIAlignment alignLeftRightCenter, GUIBackground background, float border, float textMargin,
						float size, int textureIx, Vector4 uvMap, DropdownDelegate dropdownSelect, int id, Vector4 uvMapRadioButtonOn, Vector4 uvMapRadioButtonOff
						, RadioBox rB) {
		Radio r = (GameObject.Instantiate(radioPrefab) as GameObject).GetComponent<Radio>();
		r.Initialize(this, text, containerID, CreateBackground(background, textureIx, uvMap), textMargin, size, alignLeftRightCenter, dropdownSelect, id, uvMapRadioButtonOn, uvMapRadioButtonOff, textureIx, rB);
		containers[containerID].AddElement(r.transform, r.GetSize());
		return r.gameObject.GetInstanceID();
	}

	public int AddDim(int containerID, TouchDelegate tD) {
		Dim d = (GameObject.Instantiate(dimPrefab) as GameObject).GetComponent<Dim>();
		d.Initialize(this, tD);
		dims.Add(d.gameObject.GetInstanceID(), d);
		containers[containerID].AddElement(d.transform, d.GetSize(), GUIAlignment.Center, 0f, GUIAlignment.Center, 0f);
		return d.gameObject.GetInstanceID();
	}
	
	public void AddCustomAnimation(Transform t) {
		t.gameObject.AddComponent<CustomAnimation>();
	}
	
	public static void CenterOnScreen(Transform t) {
		// center x,y
		Vector3 pos = t.position;
		pos.x = Camera.main.transform.position.x;
		pos.y = Camera.main.transform.position.y;
		t.position = pos;
	}

	public static void ResizeToScreenSize(Transform t) {
		Vector3 scale = t.localScale;
		scale.x *= Camera.main.aspect;
		t.localScale = scale;
	}

	public Vector3 GetSize() {
		return transform.lossyScale;
		//return background.GetSize();
	}
	
	public Vector3 GetCenter() {
		return transform.position;
		//return background.GetCenter();
	}

	public void SetGameInputZLevel(float maxZ) {
		gameInput.SetGUIRaycastLength(maxZ);
	}
	
	public void ResetGameInputZLevel() {
		gameInput.SetGUIRaycastLength(transform.position.z);
	}
		
	public void SetGUIInFocus(Focussable f) {
		guiElementInFocus = f;
		isGUIElementInFocus = true;
	}
	
	public void DeleteGUIInFocus() {
		isGUIElementInFocus = false;
	}
	
	public void SendTouchDown(GameObject gO, int finger) {
		if (isGUIElementInFocus && guiElementInFocus.IsBlocking()) {
			// do nothing
		} else {
			if (isGUIElementInFocus) {
//				Debug.Log ("hereX " + guiElementInFocus);
				guiElementInFocus.LostFocus();
			}
			if (gO == gameObject) {
			} else {
				gO.SendMessage("Select", finger);
			}
		}
	}
	
	private Transform CreateBackground(GUIBackground background, int textureIx, Vector4 uvMap) {
		if (background == GUIBackground.NinePatch || background == GUIBackground.NinePatchWithCollider) {
			NinePatch nP = (GameObject.Instantiate(ninePatchPrefab) as GameObject).GetComponent<NinePatch>();
			nP.renderer.material = textureAtlas[textureIx];
			nP.Initialize(uvMap);
			if (background == GUIBackground.NinePatchWithCollider) {
				nP.gameObject.AddComponent<BoxCollider>();
			}
			return nP.transform;
		} else {
			Quad q = (GameObject.Instantiate(quadPrefab) as GameObject).GetComponent<Quad>();
			q.renderer.material = textureAtlas[textureIx];
			Mesh3D.SetUVMapping((q.GetComponent<MeshFilter>()).mesh, uvMap);		
			if (background == GUIBackground.QuadWithCollider) {
				q.gameObject.AddComponent<BoxCollider>();
			}
			return q.transform;
		}
	}

	public static void Align(Transform t, Transform parentT, GUIAlignment alignLeftRightCenter,
				float borderLeftRight, GUIAlignment alignTopBottomCenter, float borderTopBottom) {
		MyGUI.Align(parentT.position, t.lossyScale, parentT.lossyScale, t, parentT, alignLeftRightCenter,
				borderLeftRight, alignTopBottomCenter, borderTopBottom);
	}
	
	public static void Align(Vector3 center, Vector3 sizeT, Vector3 sizeParentT, Transform t,
				Transform parentT, GUIAlignment alignLeftRightCenter, float borderLeftRight,
				GUIAlignment alignTopBottomCenter, float borderTopBottom) {
		t.parent = parentT;
		Vector3 reposition = Vector3.zero;
		if (alignLeftRightCenter == MyGUI.GUIAlignment.Left) {
			reposition.x = -sizeParentT.x/2 + sizeT.x/2 + borderLeftRight;
		} else if (alignLeftRightCenter == MyGUI.GUIAlignment.Right) {
			reposition.x = sizeParentT.x/2 - sizeT.x/2 - borderLeftRight;
		} else if (alignLeftRightCenter == MyGUI.GUIAlignment.Center) {
			reposition.x = borderLeftRight;
		}
		if (alignTopBottomCenter == MyGUI.GUIAlignment.Top) {
			reposition.y = sizeParentT.y/2 - sizeT.y/2 - borderTopBottom;
		} else if (alignTopBottomCenter == MyGUI.GUIAlignment.Bottom) {
			reposition.y = -sizeParentT.y/2 + sizeT.y/2 + borderTopBottom;
		} else if (alignTopBottomCenter == MyGUI.GUIAlignment.Center) {
			reposition.y = borderTopBottom;
		}
		t.position = center + reposition;
	}
	
	public static Vector2 RectifyUV(Vector2 uv, float rectify) {
		uv.x += rectify;
		uv.y += rectify;
		return uv;
	}

	private Material GetTextMaterial() {
		Material m = textureAtlas[4];
		switch (game.state.lang) {
			case 0 :				
				m.mainTexture = fontTextures[3];
				break;
			case 1 :				
				m.mainTexture = fontTextures[3];
				break;
			case 3 :				
				m.mainTexture = fontTextures[3];
				break;
			case 4 :				
				m.mainTexture = fontTextures[3];
				break;
			case 5 :				
				m.mainTexture = fontTextures[3];
				break;
			case 6:				
				m.mainTexture = fontTextures[3];
				break;
			case 7:				
				m.mainTexture = fontTextures[3];
				break;
			case 2 :
				m.mainTexture = fontTextures[0];
				break;
			case 8 :
				m.mainTexture = fontTextures[2];
				break;
			case 9 :
				m.mainTexture = fontTextures[1];
				break;
		}
		return m;
	}
	
	private CCFont GetBitmapFont() {
		CCFont f;
		switch (game.state.lang) {
			case 0 :				
				f = bitmapFonts[3];
				break;
			case 1 :				
				f = bitmapFonts[3];
				break;
			case 3 :				
				f = bitmapFonts[3];
				break;
			case 4 :				
				f = bitmapFonts[3];
				break;
			case 5 :				
				f = bitmapFonts[3];
				break;
			case 6:				
				f = bitmapFonts[3];
				break;
			case 7:				
				f = bitmapFonts[3];
				break;
			case 2 :
				f = bitmapFonts[0];
				break;
			case 8 :
				f = bitmapFonts[2];
				break;
			case 9 :
				f = bitmapFonts[1];
				break;
			default :
				f = bitmapFonts[3];
				break;	
		}
		return f;
	}
	
	public static int GetDigitOfNumber(int digit, int number) {
		return (int) Mathf.Floor( (number % (Mathf.Pow(10.0f, digit+1))) / Mathf.Pow(10.0f, digit));
	}

}
