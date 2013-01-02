using UnityEngine;
	
public class Room2 {
	public Cell[,,] cells;
	public int minerId;

	public Room2(int dim, int mId) {
		cells = new Cell[dim,dim,dim];
		minerId = mId;
	}
	
}