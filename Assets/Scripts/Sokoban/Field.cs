using System;
using UnityEngine;

public class Field {
	public IntDouble pos;
	public int type;
	
//	private Sokoban sokoban;
	public Bulb bulb;
	
	public static int EMPTY = 0;
	public static int WALL = 1;
	public static int GOAL = 2;
		
	public Field(Sokoban sokoban_, int type_, IntDouble pos_) {
//		sokoban = sokoban_;
		type = type_;
		pos = pos_;
	}
	
	public void AddBulb(Bulb bulb_) {
		bulb = bulb_;
	}
	
	public void RemoveBulb() {
		bulb = null;
	}
	
	public bool ContainsBulb() {
		if (bulb != null) {
			return true;
		} else {
			return false;
		}
	}
		
}