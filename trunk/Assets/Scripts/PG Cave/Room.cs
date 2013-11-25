using UnityEngine;
using System.Collections.Generic;
using System;

public class Room {
	public int id; // 0=entry, 1=exit, 2... inbetween
	public int dimension;
	public Cell[,,] cells;
	public List<Cell> emptyCells;
	public IntTriple pos;
	public IntTriple deadEndCell;
	public Dictionary<IntTriple, Cell> exits; // alignment, cell : alignment is other-me
	public List<Cell> exitCells;
	public RoomMesh roomMesh;
	public Zone zone;
	
	public static float ENTRY_EXIT_CELL_MARKER = 2.0f;
	
	private static float ISOVALUE_PER_NEIGHBOUR = 0.0148f; // 0.037 1/27 neighbours 
		
	public Room(int i, int dim, IntTriple p, Zone zone_) {
		id = i;
		zone = zone_;
		dimension = dim;
		cells = new Cell[dim,dim,dim];
		emptyCells = new List<Cell>();
		exitCells = new List<Cell>();
		pos = p;
		exits = new Dictionary<IntTriple, Cell>();
	}
	
	public void AddCell(IntTriple pos, int minerId) {
		cells[pos.x, pos.y, pos.z] = new Cell(pos, minerId, false);
		emptyCells.Add(cells[pos.x, pos.y, pos.z]);
	}

	public void AddExitCell(IntTriple pos, IntTriple alignment, int minerId) {
		cells[pos.x, pos.y, pos.z] = new Cell(pos, minerId, true);
		exits.Add(alignment, cells[pos.x, pos.y, pos.z]);
		exitCells.Add(cells[pos.x, pos.y, pos.z]);
		emptyCells.Add(cells[pos.x, pos.y, pos.z]);
		
/*		emptyCells.Add(cells[pos.x+1, pos.y, pos.z]);
		emptyCells.Add(cells[pos.x+1, pos.y+1, pos.z]);
		emptyCells.Add(cells[pos.x, pos.y+1, pos.z]);*/
	}
	
	public void AddDeadEndCell(IntTriple pos, int minerId) {
		AddCell(pos, minerId);
		deadEndCell = pos;
	}

	public bool IsCellNotEmptiedByMiner(IntTriple pos, int minerId) {
		if (cells[pos.x, pos.y, pos.z] != null && cells[pos.x, pos.y, pos.z].minerId != minerId) {
			return true;
		} else {
			return false;
		}
	}	
	
	public int GetMinerIDOfCell(IntTriple pos) {
		if (cells[pos.x, pos.y, pos.z] != null) {
			return cells[pos.x, pos.y, pos.z].minerId;
		} else {
			return RoomMiner.NO_MINER;
		}
	}	
	
	public int GetCellDensity(IntTriple pos) {
//		try {
		if (cells[pos.x, pos.y, pos.z] == null) {
			return Cave.DENSITY_FILLED;
		} else {
			return Cave.DENSITY_EMPTY;
		}
//		} catch (IndexOutOfRangeException e) {
//			Debug.Log ("Exception 01 " +pos);
//			return 2;
//		}
	}

	public int GetCellDensity(int x, int y, int z) {
		if (cells[x, y, z] == null) {
			return Cave.DENSITY_FILLED;
		} else {
			return Cave.DENSITY_EMPTY;
		}
	}

	public Cell GetCell(IntTriple cellIndex) {
		return cells[cellIndex.x, cellIndex.y, cellIndex.z];
	}

	public float GetIsovalueDensity(int x, int y, int z) {
		float result = 0.3f;
//		if (cells[x,y,z] != null && cells[x,y,z].isExit) return -0.3f;
		if (cells[x,y,z] != null && cells[x,y,z].isExit) return ENTRY_EXIT_CELL_MARKER;
//		if (cells[x,y,z] != null && cells[x,y,z].isExit) return result + ISOVALUE_PER_NEIGHBOUR * 3;
//		if (cells[x,y,z] != null && cells[x-1,y-1,z] != null && cells[x-1,y-1,z].isExit) return result + ISOVALUE_PER_NEIGHBOUR * 3;
		if (x == 0 || x == dimension-1 || y == 0 || y == dimension-1 || z == 0 || z == dimension -1) return result;
		result += ISOVALUE_PER_NEIGHBOUR * GetNumberOfNeighbourCellsWithDensity(x, y, z, Cave.DENSITY_EMPTY);
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
						if (GetNumberOfNeighbourCellsWithDensity(x, y, z, Cave.DENSITY_EMPTY) >= 24) {
//							Debug.Log ("single cell around: " + x+ ", " + y +", " + z);
							AddCell(new IntTriple(x,y,z), 0);
							singleCells++;
						}
					}
				}
			}
		}
		Debug.Log ("Room has Single Cells : " + singleCells);
	}
	
	private int GetNumberOfNeighbourCellsWithDensity(int x, int y, int z, int density) {
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
	
	public void SetCellToSpawn(IntTriple cellPos) {
		cells[cellPos.x, cellPos.y, cellPos.z].isSpawn = true;
	}

/*	public void SetCellToPowerUp(IntTriple cellPos) {
		cells[cellPos.x, cellPos.y, cellPos.z].isPowerUp = true;
	}*/

	public void SetCellToKey(IntTriple cellPos) {
		cells[cellPos.x, cellPos.y, cellPos.z].isKey = true;
		zone.keyCells.Add(new GridPosition(cells[cellPos.x, cellPos.y, cellPos.z].pos, pos));
	}
	
	public GridPosition GetRandomVoidGridPosition() {
		GridPosition result = GridPosition.ZERO;
		bool cont = true;
		while (cont) {
			Cell c = emptyCells[UnityEngine.Random.Range(0, emptyCells.Count)];
			if (!c.isSpawn && !c.isExit && !c.isKey) {
				result = new GridPosition(c.pos, pos);
				cont = false;
			}
		}
		return result;
	}	

	public GridPosition GetRandomNonExitGridPosition () {
		GridPosition result = GridPosition.ZERO;
		bool cont = true;
		while (cont) {
			Cell c = emptyCells[UnityEngine.Random.Range(0, emptyCells.Count)];
			if (!c.isExit) {
				result = new GridPosition(c.pos, pos);
				cont = false;
			}
		}
		return result;
	}
	
	public GridPosition GetRandomExitPosition() {
		return new GridPosition(exitCells[UnityEngine.Random.Range(0,exitCells.Count)].pos, pos);
	}
	
}