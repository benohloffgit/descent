using UnityEngine;
	
public struct Zone {
	public Room2[,,] rooms;
	public IntTriple position;

	public Zone(int dim, IntTriple pos) {
		rooms = new Room2[dim,dim,dim];
		position = pos;
	}
	
	
}
