using System;
using UnityEngine;
using System.Collections;

public class Container : MonoBehaviour {
	public Transform background;	
	public Vector3 nextPos; // absolute
	
	private ArrayList elements;
	private TouchDelegate touchDelegate;
	private MyGUI myGUI;
//	private int containerID;
	private bool isScrollableContainer;
	public SelectionMode mode;
	private Vector3 scrollStartPos;
	private Vector3 scrollEndPos;
	private int scrollFinger;
	private float realignTimer;
	private bool isFixedSize;
	private Transform blendTop;
	private Transform blendBottom;
	
	// virtual coords based on elements added
	private Vector3 bottomLeft;
	private Vector3 topRight;
	
	public enum SelectionMode { Off=0, On=1 }
		
	void Awake() {
		elements = new ArrayList();
		isScrollableContainer = false;
		mode = SelectionMode.Off;
//		sizeY = 0;
	}
	
	void Start() {
		if (isScrollableContainer) {
			// even though the scrolleable container is not fixed size we use lossy scale here to get its real size
			float amountToScroll = GetSize().y - transform.lossyScale.y;
			if (amountToScroll < 0) {
				amountToScroll = 0;
				blendBottom.renderer.enabled = false;
			} else {
				blendBottom.renderer.enabled = true;
			}
			// adapt background
			SetBackgroundScrollable();
			blendTop.renderer.enabled = false;
			scrollEndPos = scrollStartPos + new Vector3(0, amountToScroll, 0); 
//			Debug.Log("scrollStartPos " +  scrollStartPos + " scrollEndPos " + scrollEndPos);
			
		}
	}
	
	void LateUpdate() {
		if (mode == SelectionMode.On) {
			if (myGUI.gameInput.isTouchMoved[scrollFinger]) {
				// delta in pixels
				Vector3 delta = myGUI.gameInput.touchPositionDelta[scrollFinger];
				// since we have a orthogonal camera we just have to divide by Screen.height
				float deltaY = delta.y/Screen.height; //-Camera.main.ScreenToWorldPoint(new Vector3(delta.x, delta.y, Mathf.Abs(cameraZ-transform.position.z)));
				transform.position += new Vector3(0,deltaY,0);
			}
			if (myGUI.gameInput.isTouchUp[scrollFinger]) {
				mode = SelectionMode.Off;
			}
		}
		if (isScrollableContainer) {
			// if beyond start or end pos, slow down
			if (transform.position.y < scrollStartPos.y) {
				realignTimer += Time.deltaTime;
				transform.position = Vector3.Lerp(transform.position, scrollStartPos, realignTimer);
				blendTop.renderer.enabled = false;
			} else if (transform.position.y > scrollEndPos.y) {
				realignTimer += Time.deltaTime;
				transform.position = Vector3.Lerp(transform.position, scrollEndPos, realignTimer);
				blendBottom.renderer.enabled = false;
			} else {
				realignTimer = 0;
				if (transform.position.y > scrollStartPos.y && transform.position.y < scrollEndPos.y) {
					blendTop.renderer.enabled = true;
					blendBottom.renderer.enabled = true;
				} else if (transform.position.y == scrollStartPos.y) {
					blendTop.renderer.enabled = false;
				} else if (transform.position.y == scrollEndPos.y) {
					blendBottom.renderer.enabled = false;
				}
			}
		}
	}
	
	public void Initialize(Vector3 s, bool isFS) {
		isFixedSize = isFS;
		transform.localScale = s;
		if (isFixedSize) {
			bottomLeft = GetBottomLeftFixedSize(); //new Vector3(transform.position.x - s.x/2.0f, transform.position.y - s.y, transform.position.z);
			topRight = GetTopRightFixedSize(); //new Vector3(transform.position.x + s.x/2.0f, transform.position.y, transform.position.z);
		} else {
			bottomLeft = new Vector3(transform.position.x - s.x/2.0f, transform.position.y, transform.position.z);
			topRight = new Vector3(transform.position.x + s.x/2.0f, transform.position.y, transform.position.z);
		}
		nextPos = bottomLeft + new Vector3(0,0,-1f);
	}
	
	public void Initialize(MyGUI mG, int cID, TouchDelegate tD, Transform backgr) {
		myGUI = mG;
//		containerID = cID;
		touchDelegate = tD;
		
		SetBackground(backgr);
	}
	
	public void InitializeAsScrollable(MyGUI mG, Transform backgr, Transform blendT, Transform blendB) {
		myGUI = mG;
		background = backgr;					
		isScrollableContainer = true;
		mode = SelectionMode.Off;
		scrollStartPos = transform.position;
		realignTimer = 0;
		
		blendTop = blendT;
		blendTop.localScale = new Vector3(transform.lossyScale.x, transform.lossyScale.y * 0.15f, transform.lossyScale.z);
		blendTop.position = new Vector3(transform.position.x, transform.position.y - blendTop.lossyScale.y/2.0f, transform.position.z - 1.0f);
		blendTop.parent = transform.parent;
		blendBottom = blendB;
		blendBottom.localScale = new Vector3(transform.lossyScale.x, transform.lossyScale.y * 0.15f, transform.lossyScale.z);
		blendBottom.position = new Vector3(transform.position.x, transform.position.y - transform.lossyScale.y + blendTop.lossyScale.y/2.0f, transform.position.z - 1.0f);
		blendBottom.parent = transform.parent;	
	}
	
	private void SetBackground(Transform backgr) {
		background = backgr;
		background.transform.localScale = transform.lossyScale;
		background.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		background.transform.parent = transform;
	}
	
	private void SetBackgroundScrollable() {
		background.transform.localScale = GetSize();
		background.transform.position = new Vector3(transform.position.x, GetCenter().y, transform.position.z);
		background.transform.parent = transform;
	}

	public void AddElement(Transform t, Vector3 size) {
//		Debug.Log(nextPos + " " + size + " " +  background.GetTopLeft() + " " + t.lossyScale);
		elements.Add(t);
		if (!isFixedSize) {
			bottomLeft = new Vector3(bottomLeft.x, bottomLeft.y - size.y, bottomLeft.z);
		}
		
		t.parent = transform;
		t.position = nextPos + new Vector3(size.x/2.0f, -size.y/2.0f, 0);
		nextPos.y -= size.y;		
	}

	public void AddElement(Transform t, Vector3 scale, MyGUI.GUIAlignment alignLeftRightCenter, float borderLeftRight, MyGUI.GUIAlignment alignTopBottomCenter, float borderTopBottom) {
		elements.Add(t);
		MyGUI.Align(new Vector3(GetCenter().x, GetCenter().y, nextPos.z - 1f), scale, GetSize(), t, transform, alignLeftRightCenter, borderLeftRight, alignTopBottomCenter, borderTopBottom);
	}

	public Vector3 GetSize() {
		if (isFixedSize) {
			return transform.lossyScale;
		} else {
			return new Vector3(topRight.x-bottomLeft.x, topRight.y-bottomLeft.y, 1.0f);
		}
	}
	
	public Vector3 GetCenter() {
		if (isFixedSize) {
			return transform.position;
		} else {
			return new Vector3(transform.position.x, bottomLeft.y + GetSize().y/2.0f, transform.position.z);
		}
	}

	private Vector3 GetBottomLeftFixedSize() {
		return new Vector3(transform.position.x - transform.lossyScale.x/2, transform.position.y - transform.lossyScale.y/2, transform.position.z);
	}

	private Vector3 GetTopRightFixedSize() {
		return new Vector3(transform.position.x + transform.lossyScale.x/2, transform.position.y + transform.lossyScale.y/2, transform.position.z);
	}
	
	public void Select(int finger) {
		if (touchDelegate != null) {
			touchDelegate();
		} else if (isScrollableContainer) {
			mode = SelectionMode.On;
			scrollFinger = finger;
		}
	}

}