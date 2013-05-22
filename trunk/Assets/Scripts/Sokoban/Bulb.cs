using System;
using UnityEngine;

public class Bulb {
	private Board board;
	private Mesh mesh;
	
	public IntDouble pos;

	public Transform bulbTransform;
	
	private static float Z_POS = -5f;

	public Bulb(Board board_, IntDouble pos_) {
		pos = pos_;
		board = board_;
		bulbTransform = (GameObject.Instantiate(board.play.game.mesh3DPrefab) as GameObject).transform;
		bulbTransform.name = "SokobanBulb";
		mesh = bulbTransform.GetComponent<MeshFilter>().mesh;
		bulbTransform.renderer.material = board.play.game.sokobanMaterial;
		bulbTransform.gameObject.layer = Game.LAYER_SOKOBAN;
		bulbTransform.position = new Vector3(0,0,-5f);
		MoveTo(pos);
	}

	public void MoveTo(IntDouble newPos) {
		pos = newPos;
		bulbTransform.transform.position = Board.GetVector3Pos(pos, Z_POS);
	}
	
	public void SetToFull() {
		Mesh3D.SetUVMapping(mesh, Sokoban.UV_IMAGE_BULB_FULL);
	}

	public void SetToEmpty() {
		Mesh3D.SetUVMapping(mesh, Sokoban.UV_IMAGE_BULB_EMPTY);
	}
	
}
