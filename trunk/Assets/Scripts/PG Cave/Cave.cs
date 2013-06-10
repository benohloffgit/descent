using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cave {
	public Zone zone;
	
	public IntTriple[] textureCombinations;
	
	private Game game;
	public Play play;
	
	private int dimZone;
	private List<RoomMiner> roomMiners;
	private Door[] doors;
	private Transform[] exitSigns;
	private SecretChamberDoor secretChamberDoor;
	public int secretCaveRoomID;
	private int exitDoorIndex;
	private GameObject zoneEntry;
	private GameObject zoneExit;
	public GameObject secretCave;
	private GameObject decorationParent;
	private int digCount;
	private int quitOnPercentThreshold;
	private int maxDig = Game.DIMENSION_ROOM_CUBED;

	public static int DENSITY_FILLED = 0;
	public static int DENSITY_EMPTY = 1;

	public static IntTriple[] ZONE_DIRECTIONS = new IntTriple[] { IntTriple.FORWARD, IntTriple.UP, IntTriple.DOWN, IntTriple.LEFT, IntTriple.RIGHT };
	public static IntTriple[] ROOM_DIRECTIONS = new IntTriple[] { IntTriple.FORWARD, IntTriple.BACKWARD, IntTriple.UP, IntTriple.DOWN, IntTriple.LEFT, IntTriple.RIGHT };
	
	private static Vector3 EXIT_SIGN_POSITION = new Vector3(-0.19f, 0f, -0.07f);
	
	public static int[] MINER_QUIT_ON_PERCENT_TYPES = new int[] { 100, 200, 300, 400, 500, 600, 700, 800 };
//	public static float[] MINER_QUIT_ON_PERCENT_TYPES = new float[] { 0.025f, 0.05f, 0.075f, 0.1f, 0.125f, 0.15f, 0.175f, 0.2f };
	public static int[] MINER_QUIT_ON_CONNECTION_TYPES = new int[] { 50, 75, 100, 125, 150, 175, 200 };
		
	public Cave(Play p) {
		play = p;
		game = play.game;
		doors = new Door[4];
		AddDoor(Door.TYPE_LAST_EXIT);
		AddDoor(Door.TYPE_ENTRY);
		AddDoor(Door.TYPE_EXIT);
		exitDoorIndex = 2;
		AddDoor(Door.TYPE_NEXT_ENTRY);
		AddSecretChamberDoor();
		AddExitSigns();
		CalculateMaterialCombinations();
	}
	
	private void CalculateMaterialCombinations() {
		textureCombinations = new IntTriple[90]; // with 9 textures we have 83 combinations
		int[] textureIDs = new int[] {0,1,2,3,4,5,6,7,8};
		int count = 0;
		for (int a=0; a<textureIDs.Length; a++) {
			for (int b=a+1; b<textureIDs.Length; b++) {
				for (int c=b+1; c<textureIDs.Length; c++) {
					textureCombinations[count] = new IntTriple(a,b,c);
					count++;
				}
			}
		}
		//Debug.Log ("Texture Combinations " + (count-1));	
	}
	
	public void AddZone(int id) {
		zone = new Zone(Game.DIMENSION_ZONE, this, id);
		DigRooms();
		DistributeDecoration();
	}

	public void RemoveZone() {
		foreach (GameObject gO in GameObject.FindGameObjectsWithTag(RoomMesh.TAG)) {
			GameObject.Destroy(gO);
		}
		foreach (GameObject gO in GameObject.FindGameObjectsWithTag(RoomMesh.TAG_ROOM_CONNECTOR)) {
			GameObject.Destroy(gO);
		}
		GameObject.Destroy(zoneEntry);
//		GameObject.Destroy(doors[Door.TYPE_LAST_EXIT].gameObject);
//		GameObject.Destroy(doors[Door.TYPE_ENTRY].gameObject);
		zone = null;
		RemoveDecoration();
	}
	
	public void DigRooms() {
		List<int> roomsAvailableForSecretCave = new List<int>();
		for (int i=2; i<zone.roomList.Count; i++) {  // only middle rooms considered
			if (zone.GetFilledNeighboursOfRoom(zone.roomList[i].pos).Count > 0) {
				roomsAvailableForSecretCave.Add(i);
			}
		}
		secretCaveRoomID = roomsAvailableForSecretCave[UnityEngine.Random.Range(0, roomsAvailableForSecretCave.Count)];
//		Debug.Log ("secretCaveRoomID "+ secretCaveRoomID);
		
		int[] connectorsSet = new int[] {0,0,0,0,0,0,0,0,0,0};
		quitOnPercentThreshold = UnityEngine.Random.Range(0, MINER_QUIT_ON_PERCENT_TYPES.Length);
		for (int i=0; i<zone.roomList.Count; i++) { // first in array is entry room
			roomMiners = new List<RoomMiner>();
			Room room = zone.roomList[i];
			List<Room> neighbours = zone.GetEmptyNeighboursOfRoom(room.pos);
			IntTriple startingCell;
			
//			Debug.Log ("Digging Room " + i + " on pos " +  room.pos);
			foreach (Room neighbour in neighbours) {
				IntTriple alignment = room.pos-neighbour.pos; // how we are positioned towards neighbor
//				Debug.Log ("room " + i + " has " + neighbours.Count + " neighbours, alignment is : " + alignment);
				if (neighbour.exits.ContainsKey(alignment)) {
					startingCell = GetOppositeCell(neighbour.exits[alignment], alignment);
				} else {
					startingCell = SetEntryExit(-1*alignment, 0, Game.DIMENSION_ROOM, 2);
				}
				if (connectorsSet[neighbour.id] == 0) {
					AddRoomConnector(new GridPosition(startingCell, room.pos), alignment);
				}
				if (room.id > 1) { // all rooms other than entry or exit room
					if (neighbours.Count == 1) { // dead end room with just one exit, we have to add a second miner
						int quitOnConnRandom = UnityEngine.Random.Range(0,7);
						roomMiners.Add(new RoomMiner(this, startingCell, -1*alignment, room, roomMiners.Count, RoomMiner.Type.QuitOnConnection, MINER_QUIT_ON_CONNECTION_TYPES[quitOnConnRandom]));
						startingCell = SetEntryExit(alignment, 2, Game.DIMENSION_ROOM-2, 2);
						// dead end miner
						roomMiners.Add(
							new RoomMiner(this, startingCell, room, roomMiners.Count, RoomMiner.Type.QuitOnConnection, MINER_QUIT_ON_CONNECTION_TYPES[quitOnConnRandom]));
					} else {
						// non dead-end middle rooms
						roomMiners.Add(new RoomMiner(this, startingCell, -1*alignment, room, roomMiners.Count, RoomMiner.Type.QuitOnPercent, MINER_QUIT_ON_PERCENT_TYPES[quitOnPercentThreshold]));
					}
				} else {
					roomMiners.Add(
						new RoomMiner(this, startingCell, -1*alignment, room, roomMiners.Count, RoomMiner.Type.QuitOnConnection, MINER_QUIT_ON_CONNECTION_TYPES[UnityEngine.Random.Range(0,3)]));
				}
			}
			if (room.id == 0) { // entry room
				startingCell = SetEntryExit(IntTriple.BACKWARD, 0, Game.DIMENSION_ROOM, 2);
				roomMiners.Add(new RoomMiner(this, startingCell, IntTriple.BACKWARD, room, roomMiners.Count, RoomMiner.Type.QuitOnConnection, MINER_QUIT_ON_CONNECTION_TYPES[UnityEngine.Random.Range(0,3)]));
//				Debug.Log ("Entry Room: " + startingCell);
				if (zoneExit == null) {
					zoneEntry = AddZoneEntryExit(new GridPosition(startingCell, room.pos), 0f);
				} else {
					zoneEntry = zoneExit;
					play.ship.transform.parent = zoneEntry.transform;
					zoneEntry.transform.position = GetPositionFromGrid(new GridPosition(startingCell-new IntTriple(0,0,8), room.pos));
					play.ship.transform.parent = null;
				}
				doors[Door.TYPE_LAST_EXIT].transform.position = GetPositionFromGrid(new GridPosition(startingCell-new IntTriple(0,0,7), room.pos));
				doors[Door.TYPE_ENTRY].transform.position = GetPositionFromGrid(new GridPosition(startingCell-new IntTriple(0,0,1), room.pos));
				exitSigns[0].parent = doors[Door.TYPE_ENTRY].doorL;
				exitSigns[0].localPosition = EXIT_SIGN_POSITION;
			} else if (room.id == 1) { // exit room
				startingCell = SetEntryExit(IntTriple.FORWARD, 0, Game.DIMENSION_ROOM, 2);
				roomMiners.Add(new RoomMiner(this, startingCell, IntTriple.FORWARD, room, roomMiners.Count, RoomMiner.Type.QuitOnConnection, MINER_QUIT_ON_CONNECTION_TYPES[UnityEngine.Random.Range(0,3)]));
				zoneExit = AddZoneEntryExit(new GridPosition(startingCell, room.pos), 180.0f);
				doors[Door.TYPE_EXIT].transform.position = GetPositionFromGrid(new GridPosition(startingCell+new IntTriple(0,0,1), room.pos));
				exitSigns[1].parent = doors[Door.TYPE_EXIT].doorL;
				exitSigns[1].localPosition = EXIT_SIGN_POSITION;
				doors[Door.TYPE_NEXT_ENTRY].transform.position = GetPositionFromGrid(new GridPosition(startingCell+new IntTriple(0,0,7), room.pos));
				play.placeShipBeforeExitDoor = new GridPosition(startingCell, room.pos);
//				Debug.Log ("Exit Room: " + startingCell);				
			} else if (room.id == secretCaveRoomID) {
				List<IntTriple> filledNeighbourPositions = zone.GetFilledNeighboursOfRoom(room.pos);
				IntTriple secretCaveRoomPos = filledNeighbourPositions[UnityEngine.Random.Range(0, filledNeighbourPositions.Count)];
				startingCell = SetEntryExit(secretCaveRoomPos - room.pos, 0, Game.DIMENSION_ROOM, 2);
				roomMiners.Add(new RoomMiner(this, startingCell, secretCaveRoomPos - room.pos, room, roomMiners.Count, RoomMiner.Type.QuitOnConnection, MINER_QUIT_ON_CONNECTION_TYPES[UnityEngine.Random.Range(0,3)]));
				if (secretCave == null) {
					secretCave = AddSecretCave(new GridPosition(startingCell, room.pos), (room.pos-secretCaveRoomPos).GetVector3());
				} else {
					secretCave.transform.position = GetPositionFromGrid(new GridPosition(startingCell, room.pos));
					secretCave.transform.forward = (room.pos-secretCaveRoomPos).GetVector3();
				}
				secretChamberDoor.transform.position = secretCave.transform.position;
				secretChamberDoor.transform.forward = secretCave.transform.forward;
				secretChamberDoor.transform.Rotate(Vector3.forward, 45f);
				CloseSecretChamberDoor();
				play.placeShipBeforeSecretChamberDoor = new GridPosition(startingCell, room.pos);
//				Debug.Log ("startingCell "+ startingCell);
			}
			
			connectorsSet[room.id] = 1;
//			Debug.Log ("setting connectorsSet forid " + room.id);
			
			digCount = 0;
//			bool isAtLeastOneMinerActive = true;
			bool allMinersConnectedToOneOther = false;
			bool oneMinerConnectedToTwoOthers = false;
			bool twoMinersConnectedToTwoOthers = false;
			bool oneMinerConnectedToThreeOthers = false;
			int idOfMinerConnectedToTwo = RoomMiner.NO_MINER;
			int j=0;
			while (j<10000) {
				if 
					(	(roomMiners.Count <=3 && allMinersConnectedToOneOther)
						|| (roomMiners.Count == 4 && allMinersConnectedToOneOther && oneMinerConnectedToTwoOthers)
						|| (roomMiners.Count == 5 && allMinersConnectedToOneOther && (oneMinerConnectedToThreeOthers || !twoMinersConnectedToTwoOthers))
					) {
					break;
				}
				j++;
//				isAtLeastOneMinerActive = false;
				allMinersConnectedToOneOther = true;
				foreach (RoomMiner miner in roomMiners) {
					if (miner.isActive) {
//						isAtLeastOneMinerActive = true;
						digCount += miner.Mine();
						if (roomMiners.Count <= 3) {
							if (miner.connectedToNumberOfOtherMiners == 1) {
								TestMinerForDeactivation(miner);
							} else {
								allMinersConnectedToOneOther = false;
							}
						} else if (roomMiners.Count == 4) {
							if (miner.connectedToNumberOfOtherMiners == 0) {
								allMinersConnectedToOneOther = false;
							} else if (miner.connectedToNumberOfOtherMiners == 1 && oneMinerConnectedToTwoOthers) {
								TestMinerForDeactivation(miner);
							} else if (miner.connectedToNumberOfOtherMiners == 2) {
								oneMinerConnectedToTwoOthers = true;
								TestMinerForDeactivation(miner);
							}								
						} else if (roomMiners.Count == 5) {
							if (miner.connectedToNumberOfOtherMiners == 0) {
								allMinersConnectedToOneOther = false;
							} else if (miner.connectedToNumberOfOtherMiners == 1 && (oneMinerConnectedToThreeOthers || twoMinersConnectedToTwoOthers)) {
								TestMinerForDeactivation(miner);
							} else if (miner.connectedToNumberOfOtherMiners == 2) {
								if (idOfMinerConnectedToTwo == RoomMiner.NO_MINER) {
									oneMinerConnectedToTwoOthers = true;
									idOfMinerConnectedToTwo = miner.id;
								} else if (miner.id != idOfMinerConnectedToTwo) {
									twoMinersConnectedToTwoOthers = true;
									TestMinerForDeactivation(miner);
								}
							} else if (miner.connectedToNumberOfOtherMiners == 3) {
								if (oneMinerConnectedToTwoOthers && miner.id == idOfMinerConnectedToTwo) {
									oneMinerConnectedToTwoOthers = false;
									idOfMinerConnectedToTwo = RoomMiner.NO_MINER;
								}
								oneMinerConnectedToThreeOthers = true;
							}								
						}
					}
				}
			}
			
//			if (j==10000) Debug.Log ("room miner count " + roomMiners.Count);
			Debug.Log ("Room " + room.id + " " + room.pos + " has cells: " + (digCount+roomMiners.Count) + " j=" + j + " " + MINER_QUIT_ON_PERCENT_TYPES[quitOnPercentThreshold] + " miners: " + roomMiners.Count);
//			if (i==2) room.TestRoomForSingleCells();
			CreateRoomMesh(room);
		}
		roomMiners.Clear();
		foreach (Door d in doors) {
			d.Reset();
		}
		zone.roomList[0].roomMesh.renderer.sharedMaterial.SetTexture("_TexWall", game.caveTextures[textureCombinations[Mathf.FloorToInt(play.zoneID/5)].x]);
		zone.roomList[0].roomMesh.renderer.sharedMaterial.SetTexture("_TexBase", game.caveTextures[textureCombinations[UnityEngine.Random.Range(0,13)].y]);
		zone.roomList[0].roomMesh.renderer.sharedMaterial.SetTexture("_TexCeil", game.caveTextures[textureCombinations[UnityEngine.Random.Range(0,13)].z]);
//		zone.roomList[0].roomMesh.renderer.sharedMaterial.SetTexture("_TexWall", game.caveTextures[3]);
	}
	
	private void TestMinerForDeactivation(RoomMiner miner) {
		if (miner.type == RoomMiner.Type.QuitOnConnection) {
			miner.isActive = false;
		} else if (miner.type == RoomMiner.Type.QuitOnPercent) {
			if (miner.mineCount > MINER_QUIT_ON_PERCENT_TYPES[quitOnPercentThreshold]) {
				//Debug.Log ("deactivated miner " + miner.id + " on digCount "+digCount);
				miner.isActive = false;
			}
		}
	}
	
	private IntTriple GetOppositeCell(Cell cell, IntTriple alignment) {
		IntTriple result = cell.pos;
		if (alignment == IntTriple.UP) {
			result.y = 0;
		} else if (alignment == IntTriple.DOWN) {
			result.y = Game.DIMENSION_ROOM-1;
		} else if (alignment == IntTriple.RIGHT) {
			result.x = 0;
		} else if (alignment == IntTriple.LEFT) {
			result.x = Game.DIMENSION_ROOM-1;
		} else if (alignment == IntTriple.FORWARD) {
			result.z = 0;
		} else if (alignment == IntTriple.BACKWARD) {
			result.z = Game.DIMENSION_ROOM-1;
		}
		return result;
	}
	
	private void CreateRoomMesh(Room room) {
		RoomMesh roomMesh = (GameObject.Instantiate(game.roomMeshPrefab) as GameObject).GetComponent<RoomMesh>();
		roomMesh.Initialize(room);
		room.roomMesh = roomMesh;
	}
	
	public void SetAllMinersInactive() {
		foreach (RoomMiner miner in roomMiners) {
			miner.isActive = false;
		}
	}

	public IntTriple GetPosOfMinerOtherThan(int minerId, List<int> alreadyConnectedToMiners) {
		IntTriple result = IntTriple.ZERO;
		foreach (RoomMiner miner in roomMiners) {
			if (miner.id != minerId && !alreadyConnectedToMiners.Contains(miner.id)) {
				result = miner.pos;
				break;
			}
		}
		return result;
	}

	private IntTriple SetEntryExit(IntTriple delta, int lowDim, int highDim, int borderLimit) {
		IntTriple entryExit;
		IntDouble random = new IntDouble(
			UnityEngine.Random.Range(lowDim+borderLimit, highDim-(borderLimit+1)),
			UnityEngine.Random.Range(lowDim+borderLimit, highDim-(borderLimit+1)));
		if (delta == IntTriple.UP) {
			entryExit = new IntTriple(random.x, highDim-1, random.y);
		} else if (delta == IntTriple.DOWN) {
			entryExit = new IntTriple(random.x, lowDim, random.y);
		} else if (delta == IntTriple.LEFT) {
			entryExit = new IntTriple(lowDim,random.x,random.y);
		} else if (delta == IntTriple.RIGHT) {
			entryExit = new IntTriple(highDim-1,random.x,random.y);
		} else if (delta == IntTriple.FORWARD) {
			entryExit = new IntTriple(random.x,random.y,highDim-1);
		} else {
			entryExit = new IntTriple(random.x,random.y,lowDim);
		}
		return entryExit;
	}
	
	public static Vector3 GetPositionFromCube(Vector3 cubePosition) {
		return cubePosition * RoomMesh.MESH_SCALE;
	}

	public Vector3 GetCaveEntryPosition() {
		Room entryRoom = zone.roomList[0];
//		Debug.Log ("entryRoom pos " +entryRoom.pos +" " + entryRoom.entryCell);
		IntTriple pos = entryRoom.exits[IntTriple.BACKWARD].pos + new IntTriple(0,0,-3);
		return (entryRoom.pos.GetVector3() * Game.DIMENSION_ROOM + pos.GetVector3()) * RoomMesh.MESH_SCALE;
//		return (entryRoom.pos.GetVector3() * Game.DIMENSION_ROOM + entryRoom.exits[IntTriple.BACKWARD].pos.GetVector3()+Game.CELL_CENTER) * RoomMesh.MESH_SCALE;
	}
	
	public int GetCellDensity(GridPosition gridPosition) {
		return zone.GetCellDensity(gridPosition);
	}

	public int GetRoomDensity(GridPosition gridPosition) {
		return zone.GetRoomDensity(gridPosition);
	}
	
	public GridPosition GetRandomEmptyGridPositionFrom(GridPosition gridPosition, int maxDistance) {
		GridPosition result = gridPosition;
		int currentDirection = UnityEngine.Random.Range(0, RoomMesh.DIRECTIONS.Length);
//		Debug.Log ("starting with direction " + currentDirection);
		for (int i=0; i<RoomMesh.DIRECTIONS.Length; i++) {
			// test up to max Distance in that direction
			for (int j=0; j<maxDistance; j++) {
				GridPosition newPosition = new GridPosition(RoomMesh.DIRECTIONS[currentDirection] * (j+1) + gridPosition.cellPosition, gridPosition.roomPosition);
				int cellDensity;
				try {
					cellDensity = GetCellDensity(newPosition);
				} catch (IndexOutOfRangeException e) {
					Game.DefNull(e);
					cellDensity = Cave.DENSITY_FILLED;
				}
				if (cellDensity == Cave.DENSITY_FILLED) {
					if (j > 0) {
						// exit
						i = RoomMesh.DIRECTIONS.Length;
					}
					// exit
					j = maxDistance;
				} else {
					result = newPosition;
//					Debug.Log ("result " + result);
				}
			}
			if (result != gridPosition) {
				// exit
				i = RoomMesh.DIRECTIONS.Length;
			} else {
				currentDirection++;
				if (currentDirection == RoomMesh.DIRECTIONS.Length) {
					currentDirection = 0;
				}
			}
		}
//		Debug.Log ("final direction " + currentDirection);
		return result;
	}

	public GridPosition GetNearestEmptyGridPositionFrom(GridPosition gridPosition) {
		GridPosition result = gridPosition;
		IntTriple cell = gridPosition.cellPosition;
		int dimension = Game.DIMENSION_ROOM;
		Room r = zone.GetRoom(gridPosition);
		for (int shells=0; shells<4; shells++) {
			int startDim = shells;
			for (int slices=0; slices<shells*2+1; slices++) {
				if (slices == 0 || slices == shells*2) { // full layer
					for (int nX=cell.x-startDim; nX<=cell.x+startDim; nX++) {
						if (nX >= 0 && nX<dimension) {
							for (int nY=cell.y-startDim; nY<=cell.y+startDim; nY++) {
								if (nY >= 0 && nY<dimension) {
									try {
										if (r.GetCellDensity(nX, nY, cell.z - startDim + slices) == Cave.DENSITY_EMPTY) {
											result.cellPosition = new IntTriple(nX, nY, cell.z - startDim + slices);
											return result;
										}
									} catch (IndexOutOfRangeException e) {
										Game.DefNull(e);
									}
								}
							}
						}
					}
				} else { // outer spiral
					for (int side=0; side<4; side++) {
						int x = cell.x-startDim;
						int y = cell.y-startDim;
						if (side == 1) {
							x += shells*2;
						} else if (side == 2) {
							x += shells*2;
							y += shells*2;
						} else if (side == 3) {
							y += shells*2;
						}
						for (int edge=0; edge<shells*2; edge++) {
							if (side == 0) {
								x++;
							} else if (side == 1) {
								y++;
							} else if (side == 2) {
								x--;
							} else if (side == 3) {
								y--;
							}
							try {
								if (r.GetCellDensity(x, y, cell.z - startDim + slices) == Cave.DENSITY_EMPTY) {
									result.cellPosition = new IntTriple(x, y, cell.z - startDim + slices);
									return result;
								}
							} catch (IndexOutOfRangeException e) {
								Game.DefNull(e);
							}
						}
					}
				}
			}
		}
		return result;
	}
	
	private void AddRoomConnector(GridPosition gP, IntTriple alignment) {
		GameObject rC = GameObject.Instantiate(game.roomConnectorPrefab) as GameObject;
		rC.transform.localScale *= RoomMesh.MESH_SCALE;
		rC.transform.position = GetPositionFromGrid(gP);
		Align(rC.transform, alignment);
	}
	
	public void Align(Transform t, IntTriple alignment) {
		if (alignment.x != 0) {
			t.Rotate(new Vector3(180.0f, 90.0f * alignment.x, 0));
		}
		if (alignment.y != 0) {
			t.Rotate(new Vector3(90.0f * alignment.y, 0, 0));
		}
		if (alignment.z != 0) {
			t.Rotate(new Vector3((alignment.z == 1) ? 180.0f : 0f, 0, 0));
		}
	}

	private GameObject AddZoneEntryExit(GridPosition gP, float rotation) {
		GameObject o = GameObject.Instantiate(game.roomEntryPrefab) as GameObject;
		o.transform.localScale *= RoomMesh.MESH_SCALE;
		o.transform.position = GetPositionFromGrid(gP);
		o.transform.Rotate(new Vector3(rotation, 0f, 0f));
		return o;
	}

	private GameObject AddSecretCave(GridPosition gP, Vector3 forward) {
		GameObject o = GameObject.Instantiate(game.secretCavePrefab) as GameObject;
		o.transform.localScale *= RoomMesh.MESH_SCALE;
		o.transform.position = GetPositionFromGrid(gP);
		o.transform.forward = forward;
		return o;
	}
	
	private void AddDoor(int doorType) {
		doors[doorType] = (GameObject.Instantiate(game.doorPrefab) as GameObject).GetComponent<Door>();
		doors[doorType].transform.localScale *= RoomMesh.MESH_SCALE;
		doors[doorType].Initialize(play, doorType);
	}
	
	private void AddSecretChamberDoor() {
		secretChamberDoor = (GameObject.Instantiate(game.secretChamberDoorPrefab) as GameObject).GetComponent<SecretChamberDoor>();
		secretChamberDoor.transform.localScale *= RoomMesh.MESH_SCALE;
		secretChamberDoor.Initialize(play);
	}
	
	private void AddExitSigns() {
		exitSigns = new Transform[2];
		exitSigns[0] = (GameObject.Instantiate(game.exitSignPrefab) as GameObject).transform;
		exitSigns[0].transform.localScale *= RoomMesh.MESH_SCALE;
		exitSigns[1] = (GameObject.Instantiate(game.exitSignPrefab) as GameObject).transform;
		exitSigns[1].transform.localScale *= RoomMesh.MESH_SCALE;
	}
	
	public GridPosition GetGridFromPosition(Vector3 position) {
		Vector3 unscaled = position / RoomMesh.MESH_SCALE;
		IntTriple cellPos = new IntTriple(Mathf.RoundToInt(unscaled.x),Mathf.RoundToInt(unscaled.y),Mathf.RoundToInt(unscaled.z));
		IntTriple roomPos = cellPos / Game.DIMENSION_ROOM;
		cellPos %= Game.DIMENSION_ROOM;
//		if (roomPos.x == 0 || cellPos.x < 0 || cellPos.x > 15 || cellPos.y < 0 || cellPos.y > 15 || cellPos.z < 0 || cellPos.z > 15) {
//			Debug.Log (position +" " +  cellPos + " " + roomPos);
//		}
		
/*		IntTriple roomPos = new IntTriple(Mathf.FloorToInt(roomVector.x), Mathf.FloorToInt(roomVector.y), Mathf.FloorToInt(roomVector.z));
		Vector3 cellVector = unscaled - (roomPos * Game.DIMENSION_ROOM).GetVector3();
		if (cellVector.z < 0) {
			Debug.Log (position);
		}
		// centered in cube
		IntTriple cellPos = new IntTriple(Mathf.RoundToInt(cellVector.x), Mathf.RoundToInt(cellVector.y), Mathf.RoundToInt(cellVector.z));*/
		return new GridPosition(cellPos, roomPos);
	}
		
	public Vector3 GetPositionFromGrid(GridPosition gP) {
		return (gP.roomPosition.GetVector3() * Game.DIMENSION_ROOM + gP.cellPosition.GetVector3()) * RoomMesh.MESH_SCALE;
//		return (gP.roomPosition.GetVector3() * Game.DIMENSION_ROOM + gP.cellPosition.GetVector3()+Game.CELL_CENTER) * RoomMesh.MESH_SCALE;
	}
	
	public static int GetRandomNumberFromPool(int[] pool) {
		return pool[UnityEngine.Random.Range(0, pool.Length-1)];
	}
	
	public static int ExtractRandomNumberFromPool(ref List<int> pool) {
		int random = UnityEngine.Random.Range(0, pool.Count);
		int result = pool[random];
		pool.RemoveAt(random);
		return result;
	}
	
	private void DistributeDecoration() {
		decorationParent = UnityEngine.GameObject.Instantiate(game.emptyPrefab) as UnityEngine.GameObject;
		decorationParent.name = "Decoration";
		decorationParent.layer = Game.LAYER_EFFECT;
		foreach (Room r in zone.roomList) {
			for (int i=0; i<UnityEngine.Random.Range(3,7); i++) {
				GameObject gO = UnityEngine.GameObject.Instantiate(game.crystalPrefab, Vector3.zero, Quaternion.identity) as UnityEngine.GameObject;
				PutDecorationOnWall(r, gO);
				gO.transform.parent = decorationParent.transform;
			}
			for (int i=0; i<UnityEngine.Random.Range(2,5); i++) {
				GameObject gO = UnityEngine.GameObject.Instantiate(game.flowerPrefab, Vector3.zero, Quaternion.identity) as UnityEngine.GameObject;
				PutDecorationOnWall(r, gO);
				gO.transform.parent = decorationParent.transform;
			}
		}
		MeshFilter[] meshFilters = decorationParent.GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int j = 0;
        while (j < meshFilters.Length) {
            combine[j].mesh = meshFilters[j].sharedMesh;
            combine[j].transform = meshFilters[j].transform.localToWorldMatrix;
            meshFilters[j].gameObject.active = false;
            j++;
        }
		MeshFilter mF = decorationParent.AddComponent<MeshFilter>();
		decorationParent.AddComponent<MeshRenderer>();
		decorationParent.renderer.material = meshFilters[0].renderer.material;
        mF.mesh = new Mesh();
        mF.mesh.CombineMeshes(combine);		
	}
	
	private void PutDecorationOnWall(Room r, UnityEngine.GameObject c) {
		Vector3 pos = r.GetRandomNonExitGridPosition().GetWorldVector3();
		c.transform.localScale *= (RoomMesh.MESH_SCALE/5f) * (2.0f * UnityEngine.Random.Range(0.5f,2.0f));
		c.transform.RotateAroundLocal(Vector3.forward, UnityEngine.Random.Range(0f, 360f));
		play.PlaceOnWall(pos, r, c.transform);
	}
	
	private void RemoveDecoration() {
		UnityEngine.GameObject.Destroy(decorationParent);
	}
	
	public void OpenExitDoor() {
		doors[exitDoorIndex].Open();
	}
	
	public void OpenSecretChamberDoor() {
		secretChamberDoor.Open();
	}

	private void CloseSecretChamberDoor() {
		secretChamberDoor.Close();
	}
	
}
