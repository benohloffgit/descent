using UnityEngine;
	
public class Cell {
	public IntTriple pos;
	public int minerId;
	public bool isExit;

	public Cell(IntTriple p, int mId, bool iE) {
		pos = p;
		minerId = mId;
		isExit = iE;
	}
}
