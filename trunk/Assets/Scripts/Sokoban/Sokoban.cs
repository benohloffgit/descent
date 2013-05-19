using System;
using System.Collections.Generic;
using UnityEngine;

public class Sokoban {
	private Play play;
	private Board board;
	private Player player;
		
	private Field[,] fields;
	private string[] levels;
	private IntDouble[] levelDims;
	private List<Bulb> bulbs;
	private int bulbGoal;
	private int bulbsInGoal;
	
	private static int BOARD_SIZE = 17;
	
	private static string FIELD_WALL = "#";
	private static string FIELD_PLAYER = "@";
	private static string FIELD_EMPTY_GOAL = ".";
	private static string FIELD_FULL_GOAL = "*";
	private static string FIELD_BULB = "$";
	private static string FIELD_EMPTY = " ";
	private static string LINE_END = "-";

	public static Vector4 UV_IMAGE_BLANK = new Vector4(0.5f, 0.5f, 1.0f, 1.0f);
	public static Vector4 UV_IMAGE_WALL = new Vector4(0f, 0.5f, 0.5f, 1.0f);
	public static Vector4 UV_IMAGE_GOAL = new Vector4(0f, 0f, 0.5f, 0.5f);
	public static Vector4 UV_IMAGE_PLAYER = new Vector4(0f, 0.5f, 0.5f, 1.0f);
	public static Vector4 UV_IMAGE_BULB_FULL = new Vector4(0.5f, 0.5f, 1.0f, 1.0f);
	public static Vector4 UV_IMAGE_BULB_EMPTY = new Vector4(0f, 0f, 0.5f, 0.5f);
	
	public Sokoban(Play play_) {
		play = play_;
		levels = new string[64];
		levelDims = new IntDouble[64];
		ReadLevels();
		
		CreateBoard();
		CreatePlayer();
		board.SwitchCameraOff();
	}
	
	public void SwitchOn() {
		board.SwitchCameraOn();
	}

	public void SwitchOff() {
		board.SwitchCameraOff();
	}
	
	public void RenderLevel(int id) {
		Reset();
		bulbs = new List<Bulb>();
		CreateFields(id);
	}
	
	private void Reset() {
		bulbGoal = 0;
		bulbsInGoal = 0;
		if (bulbs != null) {
			foreach (Bulb b in bulbs) {
				UnityEngine.GameObject.Destroy(b.bulbTransform.gameObject);
			}
		}
	}
	
	private void CreateBoard() {
		board = (UnityEngine.GameObject.Instantiate(play.game.sokobanBoardPrefab) as GameObject).GetComponent<Board>();
		board.CreateBoard(play, BOARD_SIZE);
		board.MoveCamera(board.center);
		board.ResizeCamera(BOARD_SIZE);
	}
	
	private void CreatePlayer() {
		player = new Player(board);
	}
	
	public void MovePlayer(IntDouble dir) {
		IntDouble newPos = player.pos + dir;
		player.PointTo(newPos);
		if (FieldEquals(newPos, Field.EMPTY) || FieldEquals(newPos, Field.GOAL)) {
			if (fields[newPos.x, newPos.y].ContainsBulb()) {
				IntDouble newBulbPos = newPos + dir;
				if ((FieldEquals(newBulbPos, Field.EMPTY) || FieldEquals(newBulbPos, Field.GOAL)) && !fields[newBulbPos.x, newBulbPos.y].ContainsBulb()) {
					player.MoveTo(newPos);
					Bulb b = fields[newPos.x, newPos.y].bulb;
					b.MoveTo(newBulbPos);
					fields[newBulbPos.x,newBulbPos.y].AddBulb(b);
					fields[newPos.x,newPos.y].RemoveBulb();
					if (FieldEquals(newPos, Field.GOAL)) {
						bulbsInGoal--;
						b.SetToEmpty();
					}
					if (FieldEquals(newBulbPos, Field.GOAL)) {
						bulbsInGoal++;
						b.SetToFull();
					}
				}
			} else {
				player.MoveTo(newPos);
			}
		}
		if (bulbsInGoal == bulbGoal) {
			Debug.Log ("Won");
		}
	}

	private void CreateFields(int id) {
		fields = new Field[BOARD_SIZE,BOARD_SIZE];
		int xOffset = Mathf.FloorToInt((BOARD_SIZE - levelDims[id].x) / 2f);
		int yOffset = Mathf.FloorToInt((BOARD_SIZE - levelDims[id].y) / 2f);
		IntDouble pos;
		int stringOffset = 0;
		string fieldChar;
//		Debug.Log ("yOffset " + yOffset + " xOffset" + xOffset);
		for (int y=BOARD_SIZE-1-yOffset; y>BOARD_SIZE-1-yOffset-levelDims[id].y; y--) {
//			Debug.Log ("y " + y);
			for (int x=xOffset; x<xOffset+levelDims[id].x; x++) {
//				Debug.Log ("x " + x);
				if (stringOffset < levels[id].Length) {
					fieldChar = levels[id].Substring(stringOffset, 1);
//					Debug.Log ("fieldChar:"+fieldChar+":");
					stringOffset++;
					if (fieldChar == LINE_END) {
						x = xOffset+levelDims[id].x;
					} else {
						pos = new IntDouble(x,y);
						if (fieldChar == FIELD_WALL) {
							fields[x,y] = new Field(this, Field.WALL, pos);
							board.SetFieldUV(x, y, UV_IMAGE_WALL);
						} else if (fieldChar == FIELD_EMPTY_GOAL) {
							fields[x,y] = new Field(this, Field.GOAL, pos);
							board.SetFieldUV(x, y, UV_IMAGE_GOAL);
						} else if (fieldChar == FIELD_FULL_GOAL) {
							fields[x,y] = new Field(this, Field.GOAL, pos);
							board.SetFieldUV(x, y, UV_IMAGE_GOAL);
							bulbs.Add(new Bulb(board, new IntDouble(x,y)));
							fields[x,y].AddBulb(bulbs[bulbs.Count-1]);
							bulbs[bulbs.Count-1].SetToFull();
							bulbsInGoal++;
						} else if (fieldChar == FIELD_PLAYER) {
							fields[x,y] = new Field(this, Field.EMPTY, pos);
							player.MoveTo(new IntDouble(x,y));
						} else if (fieldChar == FIELD_BULB) {
							fields[x,y] = new Field(this, Field.EMPTY, pos);
							bulbs.Add(new Bulb(board, new IntDouble(x,y)));
							fields[x,y].AddBulb(bulbs[bulbs.Count-1]);
							bulbs[bulbs.Count-1].SetToEmpty();
						} else if (fieldChar == FIELD_EMPTY) {
							fields[x,y] = new Field(this, Field.EMPTY, pos);
						}
					}
				} else {
					x = xOffset+levelDims[id].x;
					y = BOARD_SIZE;
				}
			}
		}
		board.UpdateUVs();
		bulbGoal = bulbs.Count;
	}
		
	private bool FieldEquals(IntDouble pos, int type) {
		if (fields[pos.x, pos.y] != null && fields[pos.x, pos.y].type == type) {
			return true;
		} else {
			return false;
		}
	}
		
	private void ReadLevels() {
		levelDims[0] = new IntDouble(6,7);	
		levels[0] = 
"####-" +
"# .#-" +
"#  ###" +
"#*@  #" +
"#  $ #" +
"#  ###" +
"####-";

		levelDims[1] = new IntDouble(6,7);	
		levels[1] = 
"######" +
"#    #" +
"# #@ #" +
"# $* #" +
"# .* #" +
"#    #" +
"######";

		levelDims[2] = new IntDouble(9,6);
		levels[2] = 
"  ####-" +
"###  ####" +
"#     $ #" +
"# #  #$ #" +
"# . .#@ #" +
"#########";
	}
	
}

