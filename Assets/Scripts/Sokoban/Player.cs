using System;
using UnityEngine;

public class Player {
	private Board board;
	
	public IntDouble pos;
	
	private Transform playerTransform;
	private IntTriple pointingTo;
	
	private static float Z_POS = -5f;
		
	public Player(Board board_) {
		board = board_;
		playerTransform = (GameObject.Instantiate(board.play.game.mesh3DPrefab) as GameObject).transform;
		playerTransform.name = "SokobanPlayer";
		playerTransform.renderer.material = board.play.game.sokobanMaterial;
		playerTransform.gameObject.layer = Game.LAYER_SOKOBAN;
		playerTransform.position = new Vector3(0,0,-5f);
		pointingTo = IntTriple.RIGHT;
		Mesh3D.SetUVMapping(playerTransform.GetComponent<MeshFilter>().mesh, Sokoban.UV_IMAGE_PLAYER);
	}
	
	public void MoveTo(IntDouble newPos) {
		pos = newPos;
		playerTransform.transform.position = Board.GetVector3Pos(pos, Z_POS);
	}
	
	public void PointTo(IntDouble to) {
		IntDouble dir = to-pos;
		playerTransform.right = new Vector3(dir.x, dir.y, 0);
	}
}
