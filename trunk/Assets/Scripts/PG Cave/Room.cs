using UnityEngine;
using System.Collections.Generic;

public class Room {
	public int id; // 0=entry, 1=exit, 2... inbetween
	public int dimension;
	public Cell[,,] cells;
//	public int minerId;
	public IntTriple pos;
	public IntTriple entryCell;
	public IntTriple exitCell;
	public Dictionary<IntTriple, Cell> exits; // alignment, cell
	public RoomMesh roomMesh;
	
	public static float ENTRY_EXIT_CELL_MARKER = 2.0f;
	
	private static float ISOVALUE_PER_NEIGHBOUR = 0.0148f; // 0.037 1/27 neighbours 
	
//	private Cave cave;
	
	public Room(int i, int dim, IntTriple p, Cave c) {
		id = i;
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

	public float GetIsovalueDensity(int x, int y, int z) {
		float result = 0.3f;
		if (x == entryCell.x && y == entryCell.y && z == entryCell.z) return ENTRY_EXIT_CELL_MARKER;
		if (x == exitCell.x && y == exitCell.y && z == exitCell.z) return ENTRY_EXIT_CELL_MARKER;
		if (x == 0 || x == dimension-1 || y == 0 || y == dimension-1 || z == 0 || z == dimension -1) return result;
		result += ISOVALUE_PER_NEIGHBOUR * GetNeighbourCells(x, y, z, Cave.DENSITY_EMPTY);
		if (result == 0.999) result = 1.0f;
//		Debug.Log (result);
		return result;
	}
	
	public void TestRoomForSingleCells() {
		int singleCells = 0;
		for (int x=0; x<dimension; x++) {
			for (int y=0; y<dimension; y++) {
				for (int z=0; z<dimension; z++) {
					if (GetCellDensity(x, y, z) == Cave.DENSITY_FILLED) {
						if (GetNeighbourCells(x, y, z, Cave.DENSITY_EMPTY) >= 24) {
							Debug.Log ("single cell around: " + x+ ", " + y +", " + z);
							AddCell(new IntTriple(x,y,z), 0);
							singleCells++;
						}
					}
				}
			}
		}
		Debug.Log ("Room has Single Cells : " + singleCells);
	}
	
	private int GetNeighbourCells(int x, int y, int z, int density) {
		int neighbours = 0;
		for (int nX=x-1; nX<=x+1; nX++) {
			if (nX >= 0 && nX<dimension) {
				for (int nY=y-1; nY<=y+1; nY++) {
					if (nY >= 0 && nY<dimension) {
						for (int nZ=z-1; nZ<=z+1; nZ++) {
							if (nZ >= 0 && nZ<dimension) {
								if (GetCellDensity(nX, nY, nZ) == density) {
									neighbours++;
								}
							}
						}
					}
				}
			}
		}
		return neighbours;
	}

}