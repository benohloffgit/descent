using System;
using UnityEngine;

public struct Direction {
	public int id;
	public IntDouble iDouble;

	public static Direction UP = new Direction(ID_UP);
	public static Direction DOWN = new Direction(ID_DOWN);
	public static Direction LEFT = new Direction(ID_LEFT);
	public static Direction RIGHT = new Direction(ID_RIGHT);

	public const int ID_UP = 0;
	public const int ID_RIGHT = 1;
	public const int ID_DOWN = 2;
	public const int ID_LEFT = 3;
	
	public Direction(int a) {
		id = a;
		if (id == ID_UP) {
			iDouble = IntDouble.UP;
		} else if (id == ID_RIGHT) {
			iDouble = IntDouble.RIGHT;
		} else if (id == ID_DOWN) {
			iDouble = IntDouble.DOWN;
		} else {
			iDouble = IntDouble.LEFT;
		}
	}

	public Direction(IntDouble a) {
		iDouble = a;
		if (iDouble == IntDouble.UP) {
			id = ID_UP;
		} else if (iDouble == IntDouble.RIGHT) {
			id = ID_RIGHT;
		} else if (iDouble == IntDouble.DOWN) {
			id = ID_DOWN;
		} else {
			id = ID_LEFT;
		}
	}

	public static Direction operator *(Direction t1, int i) {
		return new Direction(t1.iDouble*i);
	}
}	