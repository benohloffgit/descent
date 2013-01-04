using UnityEngine;
	
public class Cell {
	public IntTriple pos;
	public int minerId;

	public Cell(IntTriple p, int mId) {
		pos = p;
		minerId = mId;
	}
}
