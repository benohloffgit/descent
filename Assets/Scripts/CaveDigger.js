#pragma strict

public class CaveDiggerOld {
/*	
	public var cubeDensity : int[,,];
	public var cubeMinedBy : int[,,]; // 0=mined by nobody, minerst start from 1
	public var dimension : int[];
	
	public var entry : int[];
	private var entryWall : int;
	public var exit : int[];
	private var exitWall : int;
	
	private var miners : Miner[];

	public enum Dim {x=0, y=1, z=2}
	public enum Density {filled=0, empty=1}
	public enum Dir {xMin=0, xMax=1, yMin=2, yMax=3, zMin=4, zMax=5}
	
	public function CaveDigger(x : int, y : int, z : int, cD : int[,,]) {
		dimension = [x, y, z];
		cubeDensity = cD;
		cubeMinedBy = new int[x,y,z];
		miners = new Miner[2];
		
		// determine wall side ( 0:x=0, 1:x=dimX-1, 2:y=0, 3:y=dimY-1, 4:z=0, 5:z=dimZ-1)
		entryWall = GetRandomNumberFromPool([0,1,2,3,4,5]);
		entry = DigEntryExit(entryWall);
		cubeDensity[entry[Dim.x],entry[Dim.y],entry[Dim.z]] = Density.empty;

		var remainingWalls : int[] = new int[5];
		var index : int = 0;
		for (var i=0; i<6; i++) {
			if (i != entryWall) {
				remainingWalls[index] = i;
				index++;
			}
		}
		exitWall = GetRandomNumberFromPool(remainingWalls);
		exit = DigEntryExit(exitWall);
		cubeDensity[exit[Dim.x],exit[Dim.y],exit[Dim.z]] = Density.empty;
		
		DigCave();
	}
	
	private function DigEntryExit(wall : int) {
		var coordX : int = Random.Range(0, dimension[Dim.x]-1);
		var coordY : int = Random.Range(0, dimension[Dim.y]-1);
		var coordZ : int = Random.Range(0, dimension[Dim.z]-1);
		var result : int[];
		switch (wall) {
			case 0:
				result = [0,coordY,coordZ];
				break;
			case 1:
				result = [dimension[Dim.x]-1,coordY,coordZ];
				break;
			case 2:
				result = [coordX,0,coordZ];
				break;
			case 3:
				result = [coordX,dimension[Dim.y]-1,coordZ];
				break;
			case 4:
				result = [coordX,coordY,0];
				break;
			case 5:
				result = [coordX,coordY,dimension[Dim.z]-1];
				break;
		}
		return result;
	}
	
	private function DigCave() {				
		miners[0] = new Miner(entry, 1, this);
		miners[1] = new Miner(exit, 2, this);
		while (miners[0].isActive && miners[1].isActive) {
			miners[0].Mine();
			miners[1].Mine();
		}
		
	}
	
	public static function GetRandomNumberFromPool(pool : int[]) {
		return pool[Random.Range(0, pool.length-1)];
	}
}

public class Miner {
	public var pos : int[];
	public var x : int;
	public var y : int;
	public var z : int;
	public var id : int; // starting from 1
	public var isActive : boolean = true;
	
	public enum Dim {x=0, y=1, z=2}
	public enum Density {filled=0, empty=1}

	private var cD : CaveDigger;
	
	public function Miner(p : int[], i : int, caveDigger : CaveDigger) {
		pos = p;
		id = i;
		cD = caveDigger;
	}
	
	public function Mine() {
		var newPos : int[] = GetMineableNeighbour();
		if (newPos == pos) {
			isActive = false;
		} else {
			pos = newPos;
			if (pos[Dim.x] == 0 || pos[Dim.y] == 0 || pos[Dim.z] == 0 || pos[Dim.x] == cD.dimension[Dim.x]-1 || pos[Dim.y] == cD.dimension[Dim.y]-1 || pos[Dim.z] == cD.dimension[Dim.z]-1) {
				Debug.Log("should not be!" );
			}
		}
		cD.cubeDensity[pos[Dim.x],pos[Dim.y],pos[Dim.z]] = Density.empty;
	}
	
	private function GetMineableNeighbour() {
		var cubes = [[0,0,0],[0,0,0],[0,0,0],[0,0,0],[0,0,0],[0,0,0]];
		var possibilities : int = 0;
		
		for (var i=0; i<3; i++) {
			var ix : int = 0;
			var delta : int[] = [0,0,0];
			for (var step=-1; step<2; step+=2) {
				delta[i] = step;
				var stepPos = [pos[Dim.x]+delta[0],pos[Dim.y]+delta[1],pos[Dim.z]+delta[2]];
				if (		stepPos[Dim.x] > 0 && stepPos[Dim.x] < cD.dimension[Dim.x]-1 
						&& 	stepPos[Dim.y] > 0 && stepPos[Dim.y] < cD.dimension[Dim.y]-1 
						&& 	stepPos[Dim.z] > 0 && stepPos[Dim.z] < cD.dimension[Dim.z]-1 
						&& 	cD.cubeDensity[stepPos[Dim.x], stepPos[Dim.y], stepPos[Dim.z]] == Density.filled) {
					cubes[2*i+ix] = stepPos;
					ix++;
					possibilities++;
				}
			}
		}
		
		if (possibilities > 0) {
			var mineableNeighbours : int[] = new int[possibilities];
			var count : int = 0;
			for (i=0; i<6; i++) {
			if (cubes[i][0] != 0) {
					mineableNeighbours[count] = i;
					count++;
				}
			}
			
			// select randomly one of these
			return cubes[CaveDigger.GetRandomNumberFromPool(mineableNeighbours)];
		} else {
			return pos;
		}
	}
*/
}
