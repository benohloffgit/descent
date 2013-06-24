using UnityEngine;
using System.Collections;

public class Quad : MonoBehaviour {
	private Renderer myRenderer;

	void Awake() {
		myRenderer = GetComponent<Renderer>();
	}
			
	public Vector3 GetSize() {
		return new Vector3(myRenderer.bounds.size.x, myRenderer.bounds.size.y, 1.0f);
	}
	
	public Vector3 GetBottomLeft() {
		return new Vector3(myRenderer.bounds.min.x, myRenderer.bounds.min.y, myRenderer.bounds.min.z);
	}
	
	public Vector3 GetTopRight() {
		return new Vector3(myRenderer.bounds.max.x, myRenderer.bounds.max.y, myRenderer.bounds.min.z);
	}
	
	public Vector3 GetTopLeft() {
		return new Vector3(myRenderer.bounds.min.x, myRenderer.bounds.max.y, myRenderer.bounds.min.z);
	}
	
	public Vector3 GetCenter() {
		return myRenderer.bounds.center;
	}
}

