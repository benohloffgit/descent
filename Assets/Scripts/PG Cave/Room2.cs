using UnityEngine;
	
public struct Room2 {
	public Cell[,,] cells;

	public Room2(int dim) {
		cells = new Cell[dim,dim,dim];
	}
	
	
}