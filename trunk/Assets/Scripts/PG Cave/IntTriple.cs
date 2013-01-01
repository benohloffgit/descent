using System;
using UnityEngine;

public struct IntTriple {
	public int x,y,z;
	
	public static IntTriple ZERO = new IntTriple(0,0,0);	
	public static IntTriple FORWARD = new IntTriple(0,0,1);
	public static IntTriple BACKWARD = new IntTriple(0,0,-1);
	public static IntTriple UP = new IntTriple(0,1,0);
	public static IntTriple DOWN = new IntTriple(0,-1,0);
	public static IntTriple LEFT = new IntTriple(-1,0,0);
	public static IntTriple RIGHT = new IntTriple(1,0,0);
	
	public IntTriple(int a, int b, int c) {
		x=a;
		y=b;
		z=c;
	}
	
	public IntTriple(Vector3 v) {
		x=Mathf.RoundToInt(v.x);
		y=Mathf.RoundToInt(v.y);
		z=Mathf.RoundToInt(v.z);
	}
	
	public Vector3 GetVector3() {
		return new UnityEngine.Vector3(x, y, z);
	}
}
