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
		SwitchOff();
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
		board.ResetUVs();
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
			play.SokobanSolved();
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
		levelDims[0] = new IntDouble(5,3);
		levels[0] = 
"#####" +
"#@$.#" +
"#####";

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
		
		levelDims[3] = new IntDouble(8,6);
		levels[3] = 
"########" +
"#      #" +
"# .**$@#" +
"#      #" +
"#####  #" +
"    ####";

		levelDims[4] = new IntDouble(8,7);
		levels[4] = 
" #######" +
" #     #" +
" # .$. #" +
"## $@$ #" +
"#  .$. #" +
"#      #" +
"########";

		levelDims[5] = new IntDouble(12,6);
		levels[5] = 
"###### #####" +
"#    ###   #" +
"# $$     #@#" +
"# $ #...   #" +
"#   ########" +
"#####-";
	
		levelDims[6] = new IntDouble(7,8);
		levels[6] = 
"#######" +
"#     #" +
"# .$. #" +
"# $.$ #" +
"# .$. #" +
"# $.$ #" +
"#  @  #" +
"#######";
		
		levelDims[7] = new IntDouble(8,11);
		levels[7] = 
"  ######" +
"  # ..@#" +
"  # $$ #" +
"  ## ###" +
"   # #-" +
"   # #-" +
"#### #-" +
"#    ##" +
"# #   #" +
"#   # #" +
"###   #" +
"  #####";

		levelDims[8] = new IntDouble(6,7);
		levels[8] = 
"#####-" +
"#.  ##" +
"#@$$ #" +
"##   #" +
" ##  #" +
"  ##.#" +
"   ###";
		
		levelDims[9] = new IntDouble(11,8);
		levels[9] = 
"      #####" +
"      #.  #" +
"      #.# #" +
"#######.# #" +
"# @ $ $ $ #" +
"# # # # ###" +
"#       #-" +
"#########-";

		levelDims[10] = new IntDouble(9,8);
		levels[10] = 
"  ######-" +
"  #    #-" +
"  # ##@##" +
"### # $ #" +
"# ..# $ #" +
"#       #" +
"#  ######" +
"####-";
		
		levelDims[11] = new IntDouble(9,8);
		levels[11] = 
"#####-" +
"#   ##" +
"# $  #-" +
"## $ ####" +
" ###@.  #" +
"  #  .# #" +
"  #     #" +
"  #######";

		levelDims[12] = new IntDouble(7,9);
		levels[12] = 
"####-" +
"#. ##-" +
"#.@ #-" +
"#. $#-" +
"##$ ###" +
" # $  #" +
" #    #" +
" #  ###" +
" ####-";

		levelDims[13] = new IntDouble(7,6);
		levels[13] = 
"#######" +
"#     #" +
"# # # #" +
"#. $*@#" +
"#   ###" +
"#####-";

		levelDims[14] = new IntDouble(9,7);
		levels[14] = 
"     ###-" +
"######@##" +
"#    .* #" +
"#   #   #" +
"#####$# #" +
"    #   #" +
"    #####";

		levelDims[15] = new IntDouble(10,8);
		levels[15] = 
" ####-" +
" #  ####-" +
" #     ##-" +
"## ##   #-" +
"#. .# @$##" +
"#   # $$ #" +
"#  .#    #" +
"##########";

		levelDims[16] = new IntDouble(6,7);
		levels[16] = 
"#####-" +
"# @ #-" +
"#...#-" +
"#$$$##" +
"#    #" +
"#    #" +
"######";

		levelDims[17] = new IntDouble(7,9);
		levels[17] = 
"#######" +
"#     #" +
"#. .  #" +
"# ## ##" +
"#  $ #-" +
"###$ #-" +
"  #@ #-" +
"  #  #-" +
"  ####-";

		levelDims[18] = new IntDouble(8,8);
		levels[18] = 
"########" +
"#   .. #" +
"#  @$$ #" +
"##### ##" +
"   #  #-" +
"   #  #-" +
"   #  #-" +
"   ####-";
		
		levelDims[19] = new IntDouble(8,8);
		levels[19] = 
"#######-" +
"#     ###" +
"#  @$$..#" +
"#### ## #" +
"  #     #" +
"  #  ####" +
"  #  #-" +
"  ####-";

		levelDims[20] = new IntDouble(7,6);
		levels[20] = 
"####-" +
"#  ####" +
"# . . #" +
"# $$#@#" +
"##    #" +
" ######";

		levelDims[21] = new IntDouble(7,9);
		levels[21] = 
"#####-" +
"#   ###" +
"#. .  #" +
"#   # #" +
"## #  #" +
" #@$$ #" +
" #    #" +
" #  ###" +
" ####-";

		levelDims[22] = new IntDouble(7,7);
		levels[22] = 
"#######" +
"#  *  #" +
"#     #" +
"## # ##" +
" #$@.#-" +
" #   #-" +
" #####-";

		levelDims[23] = new IntDouble(7,7);
		levels[23] = 
"# #####" +
"  #   #" +
"###$$@#" +
"#   ###" +
"#     #" +
"# . . #" +
"#######";

		levelDims[24] = new IntDouble(7,7);
		levels[24] = 
" ####-" +
" #  ###" +
" # $$ #" +
"##... #" +
"#  @$ #" +
"#   ###" +
"#####-";

		levelDims[25] = new IntDouble(6,8);
		levels[25] = 
" #####" +
" # @ #" +
" #   #" +
"###$ #" +
"# ...#" +
"# $$ #" +
"###  #" +
"  ####";

		levelDims[26] = new IntDouble(6,8);
		levels[26] = 
"######-" +
"#   .#-" +
"# ## ##" +
"#  $$@#" +
"# #   #" +
"#.  ###" +
"#####-";

		levelDims[27] = new IntDouble(7,7);
		levels[27] = 
"#####-" +
"#   #-" +
"# @ #-" +
"# $$###" +
"##. . #" +
" #    #" +
" ######";

		levelDims[28] = new IntDouble(11,9);
		levels[28] = 
"     #####-" +
"     #   ##" +
"     #    #" +
" ######   #" +
"##     #. #" +
"# $ $ @  ##" +
"# ######.#-" +
"#        #-" +
"##########-";
		
		levelDims[29] = new IntDouble(6,7);
		levels[29] = 
"####-" +
"#  ###" +
"# $$ #" +
"#... #" +
"# @$ #" +
"#   ##" +
"#####-";

		levelDims[30] = new IntDouble(7,7);
		levels[30] = 
"  ####-" +
" ##  #-" +
"##@$.##" +
"# $$  #" +
"# . . #" +
"###   #" +
"  #####";

		levelDims[31] = new IntDouble(7,7);
		levels[31] = 
" ####-" +
"##  ###" +
"#     #" +
"#.**$@#" +
"#   ###" +
"##  #-" +
" ####-";

		levelDims[32] = new IntDouble(7,7);
		levels[32] = 
"#######" +
"#. #  #" +
"#  $  #" +
"#. $#@#" +
"#  $  #" +
"#. #  #" +
"#######";

		levelDims[33] = new IntDouble(9,6);
		levels[33] = 
"  ####-" +
"###  ####" +
"#       #" +
"#@$***. #" +
"#       #" +
"#########";

		levelDims[34] = new IntDouble(7,10);
		levels[34] = 
"  ####-" +
" ##  #-" +
" #. $#-" +
" #.$ #-" +
" #.$ #-" +
" #.$ #-" +
" #. $##" +
" #   @#" +
" ##   #" +
"  #####";

		levelDims[35] = new IntDouble(15,5);
		levels[35] = 
"####-" +
"#  ############" +
"# $ $ $ $ $ @ #" +
"# .....       #" +
"###############";

		levelDims[36] = new IntDouble(9,8);
		levels[36] = 
"      ###" +
"##### #.#" +
"#   ###.#" +
"#   $ #.#" +
"# $  $  #" +
"#####@# #" +
"    #   #" +
"    #####";

		levelDims[37] = new IntDouble(10,7);
		levels[37] = 
"##########" +
"#        #" +
"# ##.### #" +
"# # $$ . #" +
"# . @$## #" +
"#####    #" +
"    ######";

		levelDims[38] = new IntDouble(10,9);
		levels[38] = 
"#####-" +
"#   ####-" +
"# # # .#-" +
"#    $ ###" +
"### #$.  #" +
"#   #@   #" +
"# # ######" +
"#   #-" +
"#####-";

		levelDims[39] = new IntDouble(7,6);
		levels[39] = 
" #####-" +
" #   #-" +
"##   ##" +
"# $$$ #" +
"# .+. #" +
"#######";

		levelDims[40] = new IntDouble(8,6);
		levels[40] = 
"#######-" +
"#     #-" +
"#@$$$ ##" +
"#  #...#" +
"##    ##" +
" ######-";

		levelDims[41] = new IntDouble(7,8);
		levels[41] = 
"   ####" +
"   #  #" +
"   #@ #" +
"####$.#" +
"#   $.#" +
"# # $.#" +
"#    ##" +
"######-";

		levelDims[42] = new IntDouble(9,9);
		levels[42] = 
"     ####" +
"     # @#" +
"     #  #" +
"###### .#" +
"#   $  .#" +
"#  $$# .#" +
"#    ####" +
"###  #-" +
"  ####-";

		levelDims[43] = new IntDouble(5,3);
		levels[43] = 
"####-" +
"# .#-" +
"#  ###" +
"#*@  #" +
"#  $ #" +
"#  ###" +
"####-";

		levelDims[44] = new IntDouble(6,7);
		levels[44] = 
"######" +
"#... #" +
"#  $ #" +
"# #$##" +
"#  $ #" +
"#  @ #" +
"######";

		levelDims[45] = new IntDouble(7,8);
		levels[45] = 
" ######" +
"##    #" +
"#  ## #" +
"# # $ #" +
"#  * .#" +
"## #@##" +
" #   #-" +
" #####-";

		levelDims[46] = new IntDouble(11,7);
		levels[46] = 
"  #######-" +
"###     #-" +
"# $ $   #-" +
"# ### #####" +
"# @ . .   #" +
"#   ###   #" +
"##### #####";

		levelDims[47] = new IntDouble(8,8);
		levels[47] = 
"######-" +
"#  @ #-" +
"#  # ##-" +
"# .#  ##" +
"# .$$$ #" +
"# .#   #" +
"####   #" +
"   #####";

		levelDims[48] = new IntDouble(8,10);
		levels[48] = 
"######-" +
"# @  #-" +
"# $# #-" +
"# $  #-" +
"# $ ##-" +
"### ####" +
" #  #  #" +
" #...  #" +
" #     #" +
" #######";
		
		levelDims[49] = new IntDouble(10,7);
		levels[49] = 
"  ####-" +
"###  #####" +
"#  $  @..#" +
"# $    # #" +
"### #### #" +
"  #      #" +
"  ########";

		levelDims[50] = new IntDouble(8,7);
		levels[50] = 
"####-" +
"#  ###-" +
"#    ###" +
"#  $*@ #" +
"### .# #" +
"  #    #" +
"  ######";

		levelDims[51] = new IntDouble(6,8);
		levels[51] = 
"  ####" +
"### @#" +
"#  $ #" +
"#  *.#" +
"#  *.#" +
"#  $ #" +
"###  #" +
"  ####";

		levelDims[52] = new IntDouble(7,7);
		levels[52] = 
" #####-" +
"##. .##" +
"# * * #" +
"#  #  #" +
"# $ $ #" +
"## @ ##" +
" #####-";

		levelDims[53] = new IntDouble(12,8);
		levels[53] = 
"      ######" +
"      #    #" +
"  ##### .  #" +
"###  ###.  #" +
"# $  $  . ##" +
"# @$$ # . #-" +
"##    #####-" +
" ######-";

		levelDims[54] = new IntDouble(10,8);
		levels[54] = 
"########-" +
"# @ #  #-" +
"#      #-" +
"#####$ #-" +
"    #  ###" +
" ## #$ ..#" +
" ## #  ###" +
"    ####-";

		levelDims[55] = new IntDouble(7,6);
		levels[55] = 
"#####-" +
"#   ###" +
"#  $  #" +
"##* . #" +
" #   @#" +
" ######";

		levelDims[56] = new IntDouble(8,9);
		levels[56] = 
"  ####-" +
"  #  #-" +
"  #@ #-" +
"  #  #-" +
"### ####" +
"#    * #" +
"#  $   #" +
"#####. #" +
"    ####";

		levelDims[57] = new IntDouble(7,7);
		levels[57] = 
"####-" +
"#  ####" +
"#.*$  #" +
"# .$# #" +
"## @  #" +
" #   ##" +
" #####-";

		levelDims[58] = new IntDouble(13,9);
		levels[58] = 
"############-" +
"#          #-" +
"# ####### @##" +
"# #         #" +
"# #  $   #  #" +
"# $$ #####  #" +
"###  # # ...#" +
"  #### #    #" +
"       ######";

		levelDims[59] = new IntDouble(10,10);
		levels[59] = 
" #########" +
" #       #" +
"##@##### #" +
"#  #   # #" +
"#  #   $.#" +
"#  ##$##.#" +
"##$##  #.#" +
"#   $  #.#" +
"#   #  ###" +
"########-";

		levelDims[60] = new IntDouble(9,10);
		levels[60] = 
"########-" +
"#      #-" +
"# #### #-" +
"# #...@#-" +
"# ###$###" +
"# #     #" +
"#  $$ $ #" +
"####   ##" +
"   #.###-" +
"   ###-";

		levelDims[61] = new IntDouble(12,6);
		levels[61] = 
"   ##########" +
"####    ##  #" +
"#  $$$....$@#" +
"#      ###  #" +
"#   #### ####" +
"#####-";

		levelDims[62] = new IntDouble(10,9);
		levels[62] = 
" ######-" +
"##    #-" +
"#   $ #-" +
"#  $$ #-" +
"### .#####" +
"  ##.# @ #" +
"   #.  $ #" +
"   #. ####" +
"   ####-";

		levelDims[63] = new IntDouble(9,9);
		levels[63] = 
"  ######-" +
"  #    #-" +
"  #  $ #-" +
" ####$ #-" +
"## $ $ #-" +
"#....# ##" +
"#     @ #" +
"##  #   #" +
" ########";
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
	}
	
}

