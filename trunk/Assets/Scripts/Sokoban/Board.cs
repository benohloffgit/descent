using UnityEngine;
using System;
using System.Collections.Generic;

public class Board : MonoBehaviour {
	
	public Camera mapCamera;
	
	public Play play;
	private Mesh mesh;
	private Vector2[] newUVs;
	
	private Vector3 mapCenter;
	private int dimension;
	public IntDouble center;
	
	public static float FIELD_SIZE = 1.0f;
	public static Vector2 FIELD_CENTER = new Vector2(FIELD_SIZE/2, FIELD_SIZE/2);
	public static Vector3 FIELD_CENTER_3 = new Vector3(FIELD_SIZE/2, FIELD_SIZE/2, 0);
	
	private static float UV_BORDER = 0.01f;
		
	void Awake() {
		mesh = GetComponent<MeshFilter>().mesh;
		mapCamera = GetComponentInChildren<Camera>();		
	}
	
	public void CreateBoard(Play play_, int dimension_) {
		play = play_;
		dimension = dimension_;
		center = new IntDouble((dimension-1)/2 - 1, (dimension-1)/2 - 1);
		int fields = dimension*dimension;
		Vector3[] newVertices = new Vector3[fields*4];
		Vector3[] newNormals = new Vector3[fields*4];
		int[] newTriangles = new int[fields*6];
		newUVs = new Vector2[fields*4];
		
//		int vertexCount = 0;
		for (var y=0; y < dimension; y++) {
			for (var x=0; x < dimension; x++) {
				int i = y*dimension*4 + x*4;
				newVertices[i] = new Vector3(x*FIELD_SIZE, y*FIELD_SIZE, 0);
				newVertices[i +1] = new Vector3((x+1)*FIELD_SIZE, y*FIELD_SIZE, 0);
				newVertices[i +2] = new Vector3((x+1)*FIELD_SIZE, (y+1)*FIELD_SIZE, 0);
				newVertices[i +3] = new Vector3(x*FIELD_SIZE, (y+1)*FIELD_SIZE, 0);
				
				int k = y*dimension*6 + x*6;
				newTriangles[k] = i;
				newTriangles[k +1] = i+3;
				newTriangles[k +2] = i+1;
				newTriangles[k +3] = i+1;
				newTriangles[k +4] = i+3;
				newTriangles[k +5] = i+2;
				
				newNormals[i] = new Vector3(0,0,1);
				newNormals[i +1] = new Vector3(0,0,1);
				newNormals[i +2] = new Vector3(0,0,1);
				newNormals[i +3] = new Vector3(0,0,1);
			}
		}

		// set mesh to new values
		mesh.Clear();
		mesh.vertices = newVertices;
		mesh.uv = newUVs;
		mesh.triangles = newTriangles;
//		mesh.normals = newNormals;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		
//		meshCollider.sharedMesh = mesh;
		
	}
	
	public void ResetUVs() {
		for (var y=0; y < dimension; y++) {
			for (var x=0; x < dimension; x++) {
				int i = y*dimension*4 + x*4;
				newUVs[i] = new Vector2(Sokoban.UV_IMAGE_BLANK.x+UV_BORDER,Sokoban.UV_IMAGE_BLANK.y+UV_BORDER);
				newUVs[i +1] = new Vector2(Sokoban.UV_IMAGE_BLANK.z-UV_BORDER,Sokoban.UV_IMAGE_BLANK.y+UV_BORDER);
				newUVs[i +2] = new Vector2(Sokoban.UV_IMAGE_BLANK.z-UV_BORDER,Sokoban.UV_IMAGE_BLANK.w-UV_BORDER);
				newUVs[i +3] = new Vector2(Sokoban.UV_IMAGE_BLANK.x+UV_BORDER,Sokoban.UV_IMAGE_BLANK.w-UV_BORDER);				
			}
		}
		UpdateUVs();
	}
	
	public void SetFieldUV(int x, int y, Vector4 uv) {
		int i = y*dimension*4 + x*4;
		newUVs[i] = new Vector2(uv.x+UV_BORDER,uv.y+UV_BORDER);
		newUVs[i +1] = new Vector2(uv.z-UV_BORDER,uv.y+UV_BORDER);
		newUVs[i +2] = new Vector2(uv.z-UV_BORDER,uv.w-UV_BORDER);
		newUVs[i +3] = new Vector2(uv.x+UV_BORDER,uv.w-UV_BORDER);				
	}
	
	public void UpdateUVs() {
		mesh.uv = newUVs;
	}
	
	public void MoveCamera(IntDouble pos) {
//		cameraPos = pos;
		mapCamera.transform.position = new Vector3(
			(pos.x+1)*FIELD_SIZE+FIELD_CENTER.x,
			(pos.y+1)*FIELD_SIZE+FIELD_CENTER.y,
			mapCamera.transform.position.z);
	}
	
	public void SwitchCameraOff() {
		mapCamera.enabled = false;
	}

	public void SwitchCameraOn() {
		mapCamera.enabled = true;
	}
	
	public void ResizeCamera(int windowSize) {
		mapCamera.orthographicSize = windowSize / 2f;
	}
	
	public static Vector2 GetVector2Pos(IntDouble p) {
		return p.GetVector2() * Board.FIELD_SIZE + Board.FIELD_CENTER;
	}
	
	public static Vector3 GetVector3Pos(IntDouble p, float z) {
		return p.GetVector3(z) * Board.FIELD_SIZE + Board.FIELD_CENTER_3;
	}
	
}
