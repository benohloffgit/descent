using System;
using UnityEngine;

public class Bulb {
	private Board board;
	private Mesh mesh;
	
	public IntDouble pos;
	public Vector3 newPos;
	public Vector3 currentPos;

	public Transform bulbTransform;
	
	private static float Z_POS = -5f;

	public Bulb(Board board_, IntDouble pos_) {
		pos = pos_;
		board = board_;
		bulbTransform = (GameObject.Instantiate(board.play.game.mesh3DPrefab) as GameObject).transform;
		bulbTransform.name = "SokobanBulb";
		mesh = bulbTransform.GetComponent<MeshFilter>().mesh;
		bulbTransform.renderer.material = board.play.game.gui.textureAtlas[1];
		bulbTransform.gameObject.layer = Game.LAYER_SOKOBAN;
		bulbTransform.position = new Vector3(0,0,-5f);
		SetTo(pos);
	}

	public void DispatchUpdate(float byLerp) {
		currentPos = Vector3.Lerp(currentPos, newPos, byLerp);
		bulbTransform.position = currentPos;
	}
	
	public void MoveTo(IntDouble p) {
		pos = p;
		newPos = Board.GetVector3Pos(p, Z_POS);
	}

	public void SetTo(IntDouble p) {
		pos = p;
		currentPos = Board.GetVector3Pos(p, Z_POS);
		bulbTransform.position = currentPos;
	}
	
	public void SetToFull() {
		Mesh3D.SetUVMapping(mesh, Sokoban.UV_IMAGE_BULB_FULL);
	}

	public void SetToEmpty() {
		Mesh3D.SetUVMapping(mesh, Sokoban.UV_IMAGE_BULB_EMPTY);
	}
	
}
