using System;
using UnityEngine;

public class ProgressBar : MonoBehaviour {
	
	public int id = -1; // set for id based event handling
		
//	public Renderer myRenderer;
	private Transform back;
	private Transform fore;
		
	void Awake() {
//		myRenderer = transform.renderer;
		id = gameObject.GetInstanceID();
	}
		
	public void Initialize(MyGUI mG, Vector3 size, Transform back_, Transform fore_) {
		back = back_;
		fore = fore_;
		back.position = new Vector3(transform.position.x, transform.position.y, transform.position.z-1f);
		back.parent = transform;
		fore.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		fore.parent = transform;
		transform.localScale = size;
	}
	
	public void SetScale(float scale) {
		transform.localScale = new Vector3(scale,scale,scale);
	}
	
	public Vector3 GetSize() {
		return new Vector3(transform.lossyScale.x, transform.lossyScale.y, 1.0f);
	}
	
	public void SetBar(float amount) { // between 0-1f
		fore.localScale = new Vector3(amount,1f,1f);
		fore.localPosition = new Vector3(-back.localScale.x/2f + amount/2f, 0, 0);
	}
	
	public void DisableRenderer() {
		back.renderer.enabled = false;
		fore.renderer.enabled = false;
	}
	
	public void EnableRenderer() {
		back.renderer.enabled = true;
		fore.renderer.enabled = true;
	}
	
}

