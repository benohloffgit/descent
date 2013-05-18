using System;
using UnityEngine;

public class Player {
	private Board board;
	
	public IntDouble pos;
	public int id;
	
	private GameObject playerObject;
		
	public Player(Board board_, IntDouble pos_, int id_) {
		board = board_;
		pos = pos_;
		id = id_;
//		playerObject = GameObject.Instantiate(board.play.game.playerPrefab) as GameObject;
//		playerObject.transform.position = Board.GetVector2Pos(pos);
//		playerObject.renderer.material = board.play.game.playerMaterials[id];
	}
	
	public void MoveTo(IntDouble newPos) {
		pos = newPos;
//		playerObject.transform.position = Board.GetVector2Pos(pos);
	}
}
