using UnityEngine;
using System.Collections.Generic;

public class Room {
	public int dimension;
	public Cell[,,] cells;
//	public int minerId;
	public IntTriple pos;
	public IntTriple entryCell;
	public IntTriple exitCell;
	public Dictionary<IntTriple, Cell> exits; // alignment, cell
	public RoomMesh roomMesh;
	
	private static float ISOVALUE_PER_NEIGHBOUR = 0.037f; // 1/27 neighbours 
	
//	private Cave cave;
	
	public Room(int dim, IntTriple p, Cave c) {
		dimension = dim;
		cells = new Cell[dim,dim,dim];
//		minerId = mId;
		pos = p;
//		cave = c;
		exits = new Dictionary<IntTriple, Cell>();
	}
	
	public void AddCell(IntTriple pos, int minerId) {
		cells[pos.x, pos.y, pos.z] = new Cell(pos, minerId);
	}

	public void AddExitCell(IntTriple pos, IntTriple alignment, int minerId) {
		cells[pos.x, pos.y, pos.z] = new Cell(pos, minerId);
		exits.Add (alignment, cells[pos.x, pos.y, pos.z]);
		//exitCell = pos;
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

	public float GetNewCellDensity(int x, int y, int z) {
		float result = 0f;
		if (x == entryCell.x && y == entryCell.y && z == entryCell.z) return 2.0f;
		if (x == exitCell.x && y == exitCell.y && z == exitCell.z) return 2.0f;
		if (x == 0 || x == dimension-1 || y == 0 || y == dimension-1 || z == 0 || z == dimension -1) return result;
		for (int nX=x-1; nX<=x+1; nX++) {
			if (nX >= 0 && nX<dimension) {
				for (int nY=y-1; nY<=y+1; nY++) {
					if (nY >= 0 && nY<dimension) {
						for (int nZ=z-1; nZ<=z+1; nZ++) {
							if (nZ >= 0 && nZ<dimension) {
								if (GetCellDensity(nX, nY, nZ) == Cave.DENSITY_EMPTY) {
									result += ISOVALUE_PER_NEIGHBOUR;
								}
							}
						}
					}
				}
			}
		}
		if (result == 0.999) result = 1.0f;
//		Debug.Log (result);
		return result;
	}
	
}