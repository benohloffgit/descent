using UnityEngine;
using System.Collections;

public class ScreenSize : MonoBehaviour {

	void Awake() {
		ResizeToScreenSize();	
	}
	
	public void ResizeToScreenSize() {		
		Vector3 scale = transform.localScale;
		scale.x *= Camera.main.aspect;
		transform.localScale = scale;
	}
	
}
