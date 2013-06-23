using System;
using UnityEngine;

public struct IntDouble {
	public int x,y;

	public static IntDouble UP = new IntDouble(0,1);
	public static IntDouble DOWN = new IntDouble(0,-1);
	public static IntDouble LEFT = new IntDouble(-1,0);
	public static IntDouble RIGHT = new IntDouble(1,0);
	public static IntDouble MINUSONE = new IntDouble(-1,-1);

	public IntDouble(int a, int b) {
		x=a;
		y=b;
	}
	
	public IntDouble(Vector2 v) {
		x=Mathf.RoundToInt(v.x);
		y=Mathf.RoundToInt(v.y);
	}
	
	public Vector2 GetVector2() {
		return new UnityEngine.Vector2(x, y);
	}

	public Vector3 GetVector3(float z) {
		return new UnityEngine.Vector3(x, y, z);
	}
	
	public static IntDouble operator *(IntDouble t1, int i) {
		return new IntDouble(t1.x*i, t1.y*i);
	}

	public static IntDouble operator *(int i, IntDouble t1) {
		return new IntDouble(t1.x*i, t1.y*i);
	}

	public static IntDouble operator +(IntDouble t1, IntDouble t2) {
		return new IntDouble(t1.x+t2.x, t1.y+t2.y);
	}
	
	public static IntDouble operator -(IntDouble t1, IntDouble t2) {
		return new IntDouble(t1.x-t2.x, t1.y-t2.y);
	}

	public static bool operator !=(IntDouble t1, IntDouble t2) {
		return (t1.x != t2.x || t1.y != t2.y) ? true : false;
	}
	
	public static bool operator ==(IntDouble t1, IntDouble t2) {
		return (t1.x == t2.x && t1.y == t2.y) ? true : false;
	}	
	
	public override string ToString() {
    	return base.ToString() + ": (" + x + ", " + y + ")";
	}

	public override bool Equals (object obj) {
		return base.Equals(obj);
	}

	public override int GetHashCode () {
		return base.GetHashCode();
	}

}

