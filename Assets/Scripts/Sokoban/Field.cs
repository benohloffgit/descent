using System;
using UnityEngine;

public class Field {
	private Sokoban sokoban;
	
	public IntDouble pos;
	public int type;
	
	public static int EMPTY = 0;
	public static int WALL = 1;
	public static int GOAL = 2;
		
	public Field(Sokoban sokoban_, int type_, IntDouble pos_) {
		sokoban_ = sokoban_;
		type = type_;
		pos = pos_;
	}
		
}