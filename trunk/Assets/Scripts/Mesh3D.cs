using System;
using UnityEngine;

public class Mesh3D : MonoBehaviour {
	
	public float borderTop;
	public float borderBottom;
	public float borderLeft;
	public float borderRight;
	
	public Vector2 uv0;
	public Vector2 uv1;
	
	private Transform mesh;
	
	void Awake() {
		mesh = transform.Find("Mesh");
	}
	
	void Start() {
		SetPosition();
		if (uv0 != new Vector2(-1f,-1f) && uv1 != new Vector2(-1f,-1f)) {
			Mesh3D.SetUVMapping((mesh.GetComponent<MeshFilter>()).mesh, uv0, uv1);
		}
	}
	
	public void SetPosition() {
		Vector3 pos = transform.position;

		if (borderTop != 0f) {
			pos.y = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane)).y - borderTop;
		} else if (borderBottom != 0f) {
			pos.y = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)).y + borderBottom;
		}
		if (borderLeft != 0f) {
			pos.x = Camera.main.ScreenToWorldPoint(new Vector3(0,0,Camera.main.nearClipPlane)).x + borderLeft;
		} else if (borderRight != 0f) {
			pos.x = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width,0,Camera.main.nearClipPlane)).x - borderRight;
		}
		transform.position = pos;
	}
	
	// uv1 : lower left, uv2 : upper right
	public static void SetUVMapping(Mesh m, Vector2 uv0, Vector2 uv1) {
		// order is clock wise 0,0 0,1 1,1 1,0
		Vector2[] uvs = new Vector2[m.vertices.Length];
		uvs[2] = uv0;
		uvs[3] = new Vector2(uv0.x, uv1.y);
		uvs[0] = uv1;
		uvs[1] = new Vector2(uv1.x, uv0.y);
		
		m.uv = uvs;
	}

	public static void SetUVMapping(Mesh m, Vector4 uv0) {
		Mesh3D.SetUVMapping(m, new Vector2(uv0.x, uv0.y), new Vector2(uv0.z, uv0.w));
	}
}
