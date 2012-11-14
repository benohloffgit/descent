using System;
using UnityEngine;

public class Seed : MonoBehaviour {
	public GameObject gamePrefab;
	public Game.Mode mode;
	
	private Game game;
	
	void Awake() {
		GameObject o = GameObject.Find("Game(Clone)");
		if (o == null) {
			game = (Instantiate(gamePrefab) as GameObject).GetComponent<Game>();
		} else {
			game = o.GetComponent<Game>();
		}
		game.Initialize(mode);
	}
	
}

