using System;
using UnityEngine;
using System.Collections.Generic;

public delegate void TouchDelegate();
public delegate void TextInputDelegate(string t);
public delegate int OpenRadioBoxDelegate(int containerID, DropdownDelegate dropdownSelect, RadioBox rB);

public class MyGUI : MonoBehaviour {
	public GameObject containerPrefab;
//	public GameObject labelPrefab;
	public GameObject labelBitmapPrefab;
	public GameObject dropdownPrefab;
	public GameObject ninePatchPrefab;
	public GameObject quadPrefab;
	public GameObject imageButtonPrefab;
	public GameObject labelButtonPrefab;
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
	
	public Game game;
	public GameInput gameInput;
	public Camera guiCamera;
	
	public Dictionary<int, Container> containers;
//	public Dictionary<int, Label> labels;
	public Dictionary<int, LabelCC> labelsCC;
	public Dictionary<int, TextInput> textInputs;
	public Dictionary<int, Dropdown> dropdowns;
	public Dictionary<int, Dim> dims;
	public Dictionary<int, ImageButton> imageButtons;
	public Dictionary<int, LabelButton> labelButtons;
	public Dictionary<int, Image> images;
	
	public enum Focus { NoFocus=0, Focus=1 }	
	public enum GUIAlignment { Left=0, Right=1, Center=2, Top=3, Bottom=4 }
	public enum GUIBackground { NinePatch=0, Quad=1, NinePatchWithCollider=2, QuadWithCollider=3, None=4 }
	public enum GUIState { Off=0, On=1 }	

//	private NinePatch background;
	private Focussable guiElementInFocus;
	private bool isGUIElementInFocus; // another gui element, not our MyGUI
	public bool isInDialogMode;
	
	private float zLevel;
		
	void Awake() {
		guiCamera = GetComponentInChildren<Camera>();
			
		containers = new Dictionary<int, Container>();
//		labels = new Dictionary<int, Label>();
		labelsCC = new Dictionary<int, LabelCC>();
		textInputs = new Dictionary<int, TextInput>();
		dropdowns = new Dictionary<int, Dropdown>();
		dims = new Dictionary<int, Dim>();
		imageButtons = new Dictionary<int, ImageButton>();
		labelButtons = new Dictionary<int, LabelButton>();
		images = new Dictionary<int, Image>();
		
//		background = GetComponent<NinePatch>();
		
		zLevel = transform.position.z;
		
		isGUIElementInFocus = false;
	}
	
	public void Initialize(Game g, GameInput input) {
		game = g;
		gameInput = input;
		gameInput.RegisterGUI(this);
		ResetGameInputZLevel();
		activeTextMaterial = GetTextMaterial();
		activeFont = GetBitmapFont();
		isInDialogMode = false;
	}
	
	// as child of this GUI
	public int AddContainer() {
		zLevel -= 2.0f;
		Container c = (GameObject.Instantiate(containerPrefab, new Vector3(transform.position.x, transform.position.y, zLevel), Quaternion.identity) as GameObject).GetComponent<Container>();
		c.Initialize(this, transform.localScale, true);
		containers.Add(c.gameObject.GetInstanceID(), c);
		c.transform.parent = transform;
		return c.gameObject.GetInstanceID();		
	}
	
	// Container clickable with image background
	public int AddContainer(int containerID, Vector3 size, Vector3 pos, TouchDelegate tD, GUIBackground background, int textureIx, Vector4 uvMap, bool isFixedSize) {
		int cID = AddContainer(containerID, size, pos, isFixedSize);
		containers[cID].Initialize(this, containerID, tD, CreateBackground(background, textureIx, uvMap));
		return cID;
	}
	
	// Container with absolute position (Vector3) and zLevel
	public int AddContainer(int containerID, Vector3 size, Vector3 pos, bool isFixedSize) {
		Container c = (GameObject.Instantiate(containerPrefab, pos, Quaternion.identity) as GameObject).GetComponent<Container>();
		c.Initialize(this, size, isFixedSize);
		containers.Add(c.gameObject.GetInstanceID(), c);
		c.transform.parent = containers[containerID].transform;
		return c.gameObject.GetInstanceID();		
	}
	
	// Container with absolute position (Vector2) and zLevel
	public int AddContainer(int containerID, Vector3 size, Vector2 pos, bool isFixedSize) {
		zLevel = containers[containerID].transform.position.z - 2.0f;
		return AddContainer (containerID, size, new Vector3(pos.x, pos.y, zLevel), isFixedSize);
	}
	
	// Container with floating position
	public int AddContainer(int containerID, Vector3 scale, bool isFixedSize, GUIAlignment alignLeftRightCenter, float borderLeftRight,
					GUIAlignment alignTopBottomCenter, float borderTopBottom) {		
		Container c = (GameObject.Instantiate(containerPrefab) as GameObject).GetComponent<Container>();
		c.Initialize(this, scale, isFixedSize);
		containers.Add(c.gameObject.GetInstanceID(), c);
		containers[containerID].AddElement(c.transform, c.GetSize(), alignLeftRightCenter, borderLeftRight, alignTopBottomCenter, borderTopBottom);
		return c.gameObject.GetInstanceID();
	}
	
	// Container Scrollable with absolute position (Vector2) and image backgrounds
	public int AddContainerScrollable(int containerID, Vector3 size, Vector2 pos, GUIBackground background, int textureIDBackground, Vector4 uvMap,
			int textureIDBlendTop, Vector4 uvMapBlendTop, int textureIDBlendBottom, Vector4 uvMapBlendBottom) {
		int cID = AddContainer(containerID, size, pos, false);
		containers[cID].InitializeAsScrollable(this, CreateBackground(background, textureIDBackground, uvMap),
			CreateBackground(GUIBackground.Quad, textureIDBlendTop, uvMapBlendTop),
			CreateBackground(GUIBackground.Quad, textureIDBlendBottom, uvMapBlendBottom));
		return cID;
	}
	
	// ImageButton clickeable with image background and floating position
	public int AddImageButton(int containerID, Vector3 scale, TouchDelegate tD, GUIAlignment alignLeftRightCenter, float borderLeftRight,
					GUIAlignment alignTopBottomCenter, float borderTopBottom, Vector4 uvMap, int textureID) {
		ImageButton b = (GameObject.Instantiate(imageButtonPrefab) as GameObject).GetComponent<ImageButton>();
		b.Initialize(this, tD, textureID, uvMap, scale);
		imageButtons.Add(b.gameObject.GetInstanceID(), b);
		containers[containerID].AddElement(b.transform, b.GetSize(), alignLeftRightCenter, borderLeftRight, alignTopBottomCenter, borderTopBottom);
		return b.gameObject.GetInstanceID();
	}	

	// Button clickeable with image background and floating position
	public int AddLabelButton(
					int containerID, Vector3 scale, TouchDelegate tD,
					string text, float textMargin, float size, int textureIDText, 
					GUIAlignment alignLeftRightCenter, float borderLeftRight, GUIAlignment alignTopBottomCenter, float borderTopBottom,
					Vector4 uvMapBackground, int textureIDBackground) {
		
		LabelButton b = (GameObject.Instantiate(labelButtonPrefab) as GameObject).GetComponent<LabelButton>();
		b.Initialize(this, tD, containerID, text, textMargin, size, alignLeftRightCenter, textureIDText, textureIDBackground, uvMapBackground, scale);
		labelButtons.Add(b.gameObject.GetInstanceID(), b);
		containers[containerID].AddElement(b.transform, b.GetSize(), alignLeftRightCenter, borderLeftRight, alignTopBottomCenter, borderTopBottom);
		
		return b.gameObject.GetInstanceID();
	}	
	
	// Image with floating position and scaled by bounding container
	public int AddImage(int containerID, GUIAlignment alignLeftRightCenter, float borderLeftRight,
					GUIAlignment alignTopBottomCenter, float borderTopBottom, Vector4 uvMap, int textureID) {
		Image i = (GameObject.Instantiate(imagePrefab) as GameObject).GetComponent<Image>();
		i.Initialize(this, textureID, uvMap, containers[containerID].GetSize());
		images.Add(i.gameObject.GetInstanceID(), i);
		containers[containerID].AddElement(i.transform, containers[containerID].GetSize(), alignLeftRightCenter, borderLeftRight, alignTopBottomCenter, borderTopBottom);
		return i.gameObject.GetInstanceID();
	}	

	// Image with floating position and fixed size
	public int AddImage(int containerID, Vector3 size, GUIAlignment alignLeftRightCenter, float borderLeftRight,
					GUIAlignment alignTopBottomCenter, float borderTopBottom, Vector4 uvMap, int textureID) {
		Image i = (GameObject.Instantiate(imagePrefab) as GameObject).GetComponent<Image>();
		i.Initialize(this, textureID, uvMap, size);
		images.Add(i.gameObject.GetInstanceID(), i);
		containers[containerID].AddElement(i.transform, i.GetSize(), alignLeftRightCenter, borderLeftRight, alignTopBottomCenter, borderTopBottom);
		return i.gameObject.GetInstanceID();
	}	
	
	//  Label with left/right alignment and floating position and image background
	public int AddLabel(string text, int containerID, GUIAlignment alignLeftRightCenter, GUIBackground background, float border, float textMargin,
					float size, int textureIDText, int textureIDBackground, Vector4 uvMap) {
			LabelCC l;
			l = (GameObject.Instantiate(labelBitmapPrefab) as GameObject).GetComponent<LabelCC>();
			l.Initialize(this, text, containerID, CreateBackground(background, textureIDBackground, uvMap), textMargin, size, alignLeftRightCenter, textureIDText);
			labelsCC.Add(l.gameObject.GetInstanceID(), l);
			containers[containerID].AddElement(l.transform, l.GetSize());
			return l.gameObject.GetInstanceID();
	}

	//  Label with left/right alignment and floating position and NO background
	public int AddLabel(string text, int containerID, GUIAlignment alignLeftRightCenter,
					float border, float textMargin,	float size, int textureIDText) {
			LabelCC l;
			l = (GameObject.Instantiate(labelBitmapPrefab) as GameObject).GetComponent<LabelCC>();
			l.Initialize(this, text, containerID, textMargin, size, alignLeftRightCenter, textureIDText);
			labelsCC.Add(l.gameObject.GetInstanceID(), l);
			containers[containerID].AddElement(l.transform, l.GetSize());
//			containers[containerID].AddElement(l.transform, l.GetSize(), alignLeftRightCenter, borderLeftRight, alignTopBottomCenter, borderTopBottom);
			return l.gameObject.GetInstanceID();
	}
	
	// Label with scale and absolute position and image background
	public int AddLabel(string text, int containerID, Vector3 scale, GUIAlignment alignLeftRightCenter, float borderLeftRight,
					GUIAlignment alignTopBottomCenter, float borderTopBottom, float textMargin, float size, int textureIDText, GUIBackground background,
					Vector4 uvMap, int textureIDBackground) {
		LabelCC l;
		l = (GameObject.Instantiate(labelBitmapPrefab) as GameObject).GetComponent<LabelCC>();
		l.Initialize(this, text, containerID, CreateBackground(background, textureIDBackground, uvMap), textMargin, size, alignLeftRightCenter, scale, textureIDText);
		labelsCC.Add(l.gameObject.GetInstanceID(), l);
		containers[containerID].AddElement(l.transform, l.GetSize(), alignLeftRightCenter, borderLeftRight, alignTopBottomCenter, borderTopBottom);
		return l.gameObject.GetInstanceID();
	}

	// add floating
/*	public int AddImageLabel(string text, int containerID, GUIAlignment alignLeftRightCenter, GUIBackground background, float border, Vector4 textMargin,
					float size, int textureIDText, int textureIDBackground, Vector4 uvMap, int textureIDImage, Vector4 uvMapImage, float scaleImage) {
		LabelCC l;
		l = (GameObject.Instantiate(labelBitmapPrefab) as GameObject).GetComponent<LabelCC>();
		l.Initialize(this, text, containerID, CreateBackground(background, textureIDBackground, uvMap), textMargin, size, alignLeftRightCenter,
			CreateBackground(GUIBackground.Quad, textureIDImage, uvMapImage), scaleImage, textureIDText);
		labelsCC.Add(l.gameObject.GetInstanceID(), l);
		containers[containerID].AddElement(l.transform, l.GetSize());
		return l.gameObject.GetInstanceID();
	}*/
	
	public int AddTextInput(string text, int containerID, GUIAlignment alignLeftRightCenter, GUIBackground background, float border, float textMargin,
					float size, int textureIDText,
					int textureIDBackgr, Vector4 uvMap,
					int textureIDBackgrEdit, Vector4 uvMapEdit,
					int textureIDCursor, Vector4 uvMapCursor,
					int textureIDButton, Vector4 uvMapButton,
					int maxLength, TextInputDelegate textInputUpdated) {
		TextInput tI = (GameObject.Instantiate(textInputPrefab) as GameObject).GetComponent<TextInput>();
		tI.Initialize(this, text, containerID,
				CreateBackground(background, textureIDBackgr, uvMap),
				CreateBackground(background, textureIDBackgrEdit, uvMapEdit),
				textMargin, size, alignLeftRightCenter, maxLength, textureIDCursor, uvMapCursor, textureIDText, textureIDButton, uvMapButton, textInputUpdated);
		textInputs.Add(tI.gameObject.GetInstanceID(), tI);
		containers[containerID].AddElement(tI.transform, tI.GetSize());
		return tI.gameObject.GetInstanceID();
	}
	
	public int AddDropdown(string[] options, int selectedOption, int containerID, GUIAlignment alignLeftRightCenter, GUIBackground background, float border, float textMargin,
							float size, int textureIDText, int textureIDBackground, Vector4 uvMap, DropdownDelegate dropdownSelect,
							OpenRadioBoxDelegate openDropdown, Vector4 uvMapDropdownOpenButton, int textureIDButton) {
		Dropdown d = (GameObject.Instantiate(dropdownPrefab) as GameObject).GetComponent<Dropdown>();
		d.Initialize(this, options, selectedOption, containerID, CreateBackground(background, textureIDBackground, uvMap),
				textMargin, size, alignLeftRightCenter, dropdownSelect, openDropdown, textureIDText, textureIDButton, uvMapDropdownOpenButton);
		dropdowns.Add(d.gameObject.GetInstanceID(), d);
		containers[containerID].AddElement(d.transform, d.GetSize());
		return d.gameObject.GetInstanceID();
	}
	
	public int AddRadio(string text, int containerID, GUIAlignment alignLeftRightCenter, GUIBackground background, float border, float textMargin,
						float size, int textureIDText, int textureIDBackground, Vector4 uvMap, DropdownDelegate dropdownSelect, int id, Vector4 uvMapRadioButtonOn, Vector4 uvMapRadioButtonOff
						, RadioBox rB) {
		Radio r = (GameObject.Instantiate(radioPrefab) as GameObject).GetComponent<Radio>();
		r.Initialize(this, text, containerID, CreateBackground(background, textureIDBackground, uvMap), textMargin, size, alignLeftRightCenter, dropdownSelect, id, uvMapRadioButtonOn, uvMapRadioButtonOff, textureIDText, textureIDBackground, rB);
		containers[containerID].AddElement(r.transform, r.GetSize());
		return r.gameObject.GetInstanceID();
	}

	public int AddDim(int containerID, TouchDelegate tD, GUIAlignment alignLeftRightCenter, float borderLeftRight,
					GUIAlignment alignTopBottomCenter, float borderTopBottom, Vector4 uvMap, int textureID) {
		Dim d = (GameObject.Instantiate(dimPrefab) as GameObject).GetComponent<Dim>();
		d.Initialize(this, tD, textureID, uvMap, containers[containerID].GetSize());
		dims.Add(d.gameObject.GetInstanceID(), d);
		containers[containerID].AddElement(d.transform, containers[containerID].GetSize(), alignLeftRightCenter, borderLeftRight, alignTopBottomCenter, borderTopBottom);
		return d.gameObject.GetInstanceID();
	}	
	
	public void AddCustomAnimation(Transform t) {
		t.gameObject.AddComponent<CustomAnimation>();
	}
	
	public void CenterOnScreen(Transform t) {
		// center x,y
		Vector3 pos = t.position;
		pos.x = guiCamera.transform.position.x;
		pos.y = guiCamera.transform.position.y;
		t.position = pos;
	}

	public void ResizeToScreenSize(Transform t) {
		Vector3 scale = t.localScale;
		scale.x *= guiCamera.aspect;
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
		if (background == GUIBackground.NinePatch || background == GUIBackground.NinePatchWithCollider || background == GUIBackground.None) {
			NinePatch nP = (GameObject.Instantiate(ninePatchPrefab) as GameObject).GetComponent<NinePatch>();
			nP.renderer.material = textureAtlas[textureIx];
			nP.Initialize(uvMap);
			if (background == GUIBackground.NinePatchWithCollider) {
				nP.gameObject.AddComponent<BoxCollider>();
			}
			if (background == GUIBackground.None) {
				nP.renderer.enabled = false;
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
			reposition.x = -sizeParentT.x/2f + sizeT.x/2f + (sizeParentT.x/2f) * borderLeftRight;
		} else if (alignLeftRightCenter == MyGUI.GUIAlignment.Right) {
			reposition.x = sizeParentT.x/2f - sizeT.x/2f - (sizeParentT.x/2f) * borderLeftRight;
		} else if (alignLeftRightCenter == MyGUI.GUIAlignment.Center) {
			reposition.x = borderLeftRight;
		}
		if (alignTopBottomCenter == MyGUI.GUIAlignment.Top) {
			reposition.y = sizeParentT.y/2f - sizeT.y/2f - (sizeParentT.y/2f) * borderTopBottom;
		} else if (alignTopBottomCenter == MyGUI.GUIAlignment.Bottom) {
			reposition.y = -sizeParentT.y/2f + sizeT.y/2f + (sizeParentT.y/2f) * borderTopBottom;
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
	
	public void SetActiveTextMaterial(int materialID) {
		activeTextMaterial = textureAtlas[materialID];
	}

	public void CloseDialog(int containerID) {
		GameObject.Destroy(containers[containerID].gameObject);
		ResetGameInputZLevel();
		DeleteGUIInFocus();
		isInDialogMode = false;
	}
	
	public void OpenDialog() {
		Screen.showCursor = true;
		Screen.lockCursor = false;
		isInDialogMode = true;
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
