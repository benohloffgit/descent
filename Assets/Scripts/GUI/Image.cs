using System;
using UnityEngine;

public class Image : MonoBehaviour {
	
	public int id = -1; // set for id based event handling
		
	private Mesh mesh;
	public Renderer myRenderer;
//	private Game g;
//	private MyGUI myGUI;
		
	void Awake() {
		mesh = GetComponent<MeshFilter>().mesh;
		myRenderer = transform.renderer;
		id = gameObject.GetInstanceID();
	}
		
	public void Initialize(MyGUI mG, int textureID, Vector4 uvMap, Vector3 scale) {
//		myGUI = mG;
//		g = myGUI.game;
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

