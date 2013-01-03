using UnityEngine;
using System.Collections.Generic;

public class Room2 {
	public int dimension;
	public Cell[,,] cells;
	public int minerId;
	public IntTriple pos;
//	public IntTriple entryCell;
	public Dictionary<IntTriple, Cell> exits; // alignment, cell

	public Room2(int dim, int mId, IntTriple p) {
		dimension = dim;
		cells = new Cell[dim,dim,dim];
		minerId = mId;
		pos = p;
		exits = new Dictionary<IntTriple, Cell>();
	}
	
	public void AddCell(IntTriple pos) {
		cells[pos.x, pos.y, pos.z] = new Cell(pos);
	}

	public void AddExitCell(IntTriple pos, IntTriple alignment) {
		cells[pos.x, pos.y, pos.z] = new Cell(pos);
		exits.Add (alignment, cells[pos.x, pos.y, pos.z]);
	}
	
	public int GetCellDensity(IntTriple pos) {
		if (cells[pos.x, pos.y, pos.z] == null) {
			return Cave.DENSITY_FILLED;
		} else {
			return Cave.DENSITY_EMPTY;
		}
	}

	public int GetCellDensity(int x, int y, int z) {
		if (cells[x, y, z] == null) {
			return Cave.DENSITY_FILLED;
		} else {
			return Cave.DENSITY_EMPTY;
		}
	}
	
}