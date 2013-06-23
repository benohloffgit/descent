using System;
using UnityEngine;

public class Dim : MonoBehaviour {
	public int id = -1; // set for id based event handling
	private TouchDelegate touchDelegate;		
	private Mesh mesh;
	public Renderer myRenderer;
//	private MyGUI myGUI;
	
	void Awake() {
		mesh = GetComponent<MeshFilter>().mesh;
		id = gameObject.GetInstanceID();
		myRenderer = transform.renderer;
	}
		
	public void Select() {
		touchDelegate();
	}

	public void Initialize(MyGUI mG, TouchDelegate tD, int textureID, Vector4 uvMap, Vector3 scale) {
//		myGUI = mG;
		touchDelegate = tD;
		transform.localScale = scale;
		myRenderer.material = mG.textureAtlas[textureID];
		SetUVMapping(uvMap);
	}
	
	public void SetScale(float scale) {
		transform.localScale = new Vector3(scale,scale,scale);
	}
	
	public Vector3 GetSize() {
		return new Vector3(myRenderer.bounds.size.x, myRenderer.bounds.size.y, 1.0f);
	}
	
	public void SetUVMapping(Vector4 uvMap) {
		Mesh3D.SetUVMapping(mesh, new Vector2(uvMap.x, uvMap.y), new Vector2(uvMap.z, uvMap.w));
	}

}
