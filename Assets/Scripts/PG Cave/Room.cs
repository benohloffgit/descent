using UnityEngine;
using System.Collections.Generic;

public class Room {
	public int dimension;
	public Cell[,,] cells;
//	public int minerId;
	public IntTriple pos;
	public IntTriple entryCell;
	public Dictionary<IntTriple, Cell> exits; // alignment, cell
	public RoomMesh roomMesh;
	
	private Cave cave;

/*	public Room(int dim, IntTriple p) {
		dimension = dim;
		cells = new Cell[dim,dim,dim];
		pos = p;
		exits = new Dictionary<IntTriple, Cell>();
	}*/
	
	public Room(int dim, IntTriple p, Cave c) {
		dimension = dim;
		cells = new Cell[dim,dim,dim];
//		minerId = mId;
		pos = p;
		cave = c;
		exits = new Dictionary<IntTriple, Cell>();
	}
	
	public void AddCell(IntTriple pos, int minerId) {
		cells[pos.x, pos.y, pos.z] = new Cell(pos, minerId);
	}

	public void AddExitCell(IntTriple pos, IntTriple alignment, int minerId) {
		cells[pos.x, pos.y, pos.z] = new Cell(pos, minerId);
		exits.Add (alignment, cells[pos.x, pos.y, pos.z]);
	}

	public bool IsCellNotEmptiedByMiner(IntTriple pos, int minerId) {
		if (cells[pos.x, pos.y, pos.z] != null && cells[pos.x, pos.y, pos.z].minerId != minerId) {
			return true;
		} else {
			return false;
		}
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