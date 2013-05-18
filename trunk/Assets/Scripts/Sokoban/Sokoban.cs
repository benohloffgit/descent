using System;
using UnityEngine;

public class Sokoban {
	private Play play;
	private Board board;
		
	private Field[,] fields;
	private string[] levels;
	private IntDouble[] levelDims;
	
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
	
	public Sokoban(Play play_) {
		play = play_;
		levels = new string[64];
		levelDims = new IntDouble[64];
		ReadLevels();
		
		CreateBoard();
		board.SwitchCameraOff();
	}
	
	public void SwitchOn() {
		board.SwitchCameraOn();
	}

	public void SwitchOff() {
		board.SwitchCameraOff();
	}
	
	public void RenderLevel(int id) {
		CreateFields(id);
	}
	
	private void CreateBoard() {
		board = (UnityEngine.GameObject.Instantiate(play.game.sokobanBoardPrefab) as GameObject).GetComponent<Board>();
		board.CreateBoard(play, BOARD_SIZE);
		board.MoveCamera(board.center);
		board.ResizeCamera(BOARD_SIZE);
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
				Debug.Log ("x " + x);
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
	
	}
}

