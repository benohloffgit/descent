using System;
using UnityEngine;

public class Image : MonoBehaviour {
	
	public int id = -1; // set for id based event handling
		
	private Transform mesh;
	private Renderer myRenderer;
	private Game g;
	private MyGUI myGUI;
		
	void Awake() {
		mesh = transform.Find("Mesh");
		myRenderer = GetComponentInChildren<Renderer>();
		id = gameObject.GetInstanceID();
	}
	
	void Start() {
		g = GameObject.Find("/Game(Clone)").GetComponent<Game>();
	}
	
	public void Initialize(MyGUI mG, int textureID, Vector4 uvMapDropdownOpenButton, Vector3 scale) {
		myGUI = mG;
		transform.localScale = scale;
		myRenderer.material = mG.textureAtlas[textureID];
		Mesh3D.SetUVMapping((mesh.GetComponent<MeshFilter>()).mesh, new Vector2(uvMapDropdownOpenButton.x, uvMapDropdownOpenButton.y), new Vector2(uvMapDropdownOpenButton.z, uvMapDropdownOpenButton.w));
	}
	
	public void SetScale(float scale) {
		mesh.localScale = new Vector3(scale,scale,scale);
	}
	
	public Vector3 GetSize() {
		return new Vector3(myRenderer.bounds.size.x, myRenderer.bounds.size.y, 1.0f);
	}
	
}

