using System;
using UnityEngine;

public class Player {
	private Board board;
	
	public IntDouble pos;
	public Vector3 newPos;
	public Vector3 currentPos;
	
	private Transform playerTransform;
//	private IntTriple pointingTo;
	
	private static float Z_POS = -5f;
		
	public Player(Board board_) {
		board = board_;
		playerTransform = (GameObject.Instantiate(board.play.game.mesh3DPrefab) as GameObject).transform;
		playerTransform.name = "SokobanPlayer";
		playerTransform.renderer.material = board.play.game.gui.textureAtlas[1];
		playerTransform.gameObject.layer = Game.LAYER_SOKOBAN;
		playerTransform.position = new Vector3(0,0,-5f);
//		pointingTo = IntTriple.RIGHT;
		Mesh3D.SetUVMapping(playerTransform.GetComponent<MeshFilter>().mesh, Sokoban.UV_IMAGE_PLAYER);
	}
	
	public void MoveTo(IntDouble p) {
		pos = p;
		newPos = Board.GetVector3Pos(p, Z_POS);
	}

	public void SetTo(IntDouble p) {
		pos = p;
		currentPos = Board.GetVector3Pos(p, Z_POS);
		playerTransform.position = currentPos;
	}
	
	public void DispatchUpdate(float byLerp) {
		currentPos = Vector3.Lerp(currentPos, newPos, byLerp);
		playerTransform.position = currentPos;
	}
	
	public void PointTo(IntDouble to) {
		IntDouble dir = to-pos;
		if (dir.x != 0) {
			playerTransform.rotation = Quaternion.Euler(0, 0, (dir.x-1) * -90f);
		} else {
			playerTransform.rotation = Quaternion.Euler(0, 0, (dir.y-2) * -90f);
		}
//		playerTransform.right = new Vector3(dir.x, dir.y, 0);
//		playerTransform.forward = Vector3.forward;
	}
}
