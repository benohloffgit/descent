using UnityEngine;
	
public class Cell {
	public IntTriple pos;
	public int minerId;
	public bool isExit;
	public bool isSpawn = false;
	public bool isPowerUp = false;
	public bool isKey = false;
	public bool hasTransitionedBetweenRooms = false;

	public Cell(IntTriple p, int mId, bool iE) {
		pos = p;
		minerId = mId;
		isExit = iE;
	}
}
