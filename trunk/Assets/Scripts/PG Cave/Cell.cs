using UnityEngine;
	
public class Cell {
	public IntTriple pos;
	public int minerId;
	public bool isExit;
	public bool isSpawn;

	public Cell(IntTriple p, int mId, bool iE, bool isSpawn_) {
		pos = p;
		minerId = mId;
		isExit = iE;
		isSpawn = isSpawn_;
	}
}
