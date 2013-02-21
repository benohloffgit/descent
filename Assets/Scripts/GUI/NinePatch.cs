using UnityEngine;
using System.Collections;

public class NinePatch : MonoBehaviour {
	private Mesh mesh;
	private Renderer myRenderer;
	private Vector4 uvMap = new Vector4(0f, 0f, 1.0f, 1.0f);

	void Awake() {
		mesh = (GetComponent<MeshFilter>()).mesh;	
		myRenderer = GetComponent<Renderer>();
	}
	
	public void Initialize(Vector4 uvs) {
		uvMap = uvs;
	}
	
	void Start () {		
		ScaleVertices();
		NinePatch.SetUVMapping(mesh, new Vector2(uvMap.x, uvMap.y), new Vector2(uvMap.z, uvMap.w));
/*		Vector3[] vertices = mesh.vertices;
		for (int i=0; i<vertices.Length; i++) {
			Debug.Log (vertices[i].x + " " + vertices[i].y);
		}*/
		
		/*
		 * Vertices order
		 * 
		 * 3  5  4  0 
		 * 11 12 13 8
		 * 10 14 15 9
		 * 2  7  6  1
		 * 
		 */
	}
	
	private void ScaleVertices() {
		Vector3[] vertices = mesh.vertices;
		float width = GetSize().x; //transform.TransformPoint(vertices[0]).x - transform.TransformPoint(vertices[2]).x;
		float height = GetSize().y; //transform.TransformPoint(vertices[0]).y - transform.TransformPoint(vertices[2]).y;
		
		float aspect = width/height;
		
		if (aspect < 1.0f) {		
			float newY = 0.25f * aspect;
			
			// y 0.25 vertices
			vertices[11] = new Vector3(vertices[11].x, 0.5f-newY, vertices[11].z);
			vertices[12] = new Vector3(vertices[12].x, 0.5f-newY, vertices[12].z);
			vertices[13] = new Vector3(vertices[13].x, 0.5f-newY, vertices[13].z);
			vertices[8] = new Vector3(vertices[8].x, 0.5f-newY, vertices[8].z);
			
			// y -0.25 vertices
			vertices[10] = new Vector3(vertices[10].x, -0.5f+newY, vertices[10].z);
			vertices[14] = new Vector3(vertices[14].x, -0.5f+newY, vertices[14].z);
			vertices[15] = new Vector3(vertices[15].x, -0.5f+newY, vertices[15].z);
			vertices[9] = new Vector3(vertices[9].x, -0.5f+newY, vertices[9].z);
		} else {
			float newX = 0.25f * height/width;
			
			// x 0.25 vertices
			vertices[4] = new Vector3(0.5f-newX, vertices[4].y, vertices[4].z);
			vertices[13] = new Vector3(0.5f-newX, vertices[13].y, vertices[13].z);
			vertices[15] = new Vector3(0.5f-newX, vertices[15].y, vertices[15].z);
			vertices[6] = new Vector3(0.5f-newX, vertices[6].y, vertices[6].z);
			
			// x -0.25 vertices
			vertices[5] = new Vector3(-0.5f+newX, vertices[5].y, vertices[5].z);
			vertices[12] = new Vector3(-0.5f+newX, vertices[12].y, vertices[12].z);
			vertices[14] = new Vector3(-0.5f+newX, vertices[14].y, vertices[14].z);
			vertices[7] = new Vector3(-0.5f+newX, vertices[7].y, vertices[7].z);			
		}
		
		mesh.vertices = vertices;
	}

	// uv1 : lower left, uv2 : upper right
	public static void SetUVMapping(Mesh m, Vector2 uv0, Vector2 uv1) {
		uv0 = MyGUI.RectifyUV(uv0, 0.001f);
		uv1 = MyGUI.RectifyUV(uv1, -0.001f);
		Vector2[] uvs = new Vector2[m.vertices.Length];
		float quarterX = (uv1.x-uv0.x)/4.0f;
		float quarterY = (uv1.y-uv0.y)/4.0f;
		uvs[3] = new Vector2(uv0.x, uv1.y);
		uvs[5] = new Vector2(uv0.x + quarterX , uv1.y);
		uvs[4] = new Vector2(uv1.x - quarterX , uv1.y);
		uvs[0] = uv1;
		uvs[11] = new Vector2(uv0.x, uv1.y - quarterY);
		uvs[12] = new Vector2(uv0.x + quarterX, uv1.y - quarterY);
		uvs[13] = new Vector2(uv1.x - quarterX, uv1.y - quarterY);
		uvs[8] = new Vector2(uv1.x, uv1.y - quarterY);
		uvs[10] = new Vector2(uv0.x, uv0.y + quarterY);
		uvs[14] = new Vector2(uv0.x + quarterX, uv0.y + quarterY);
		uvs[15] = new Vector2(uv1.x - quarterX, uv0.y + quarterY);
		uvs[9] = new Vector2(uv1.x, uv0.y + quarterY);
		uvs[2] = uv0;
		uvs[7] = new Vector2(uv0.x + quarterX, uv0.y);
		uvs[6] = new Vector2(uv1.x - quarterX, uv0.y);
		uvs[1] = new Vector2(uv1.x, uv0.y);
		
		m.uv = uvs;
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
