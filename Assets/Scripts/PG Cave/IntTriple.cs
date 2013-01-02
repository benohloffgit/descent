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
	
	public float Magnitude() {
		return Mathf.Sqrt(Mathf.Pow(x, 2.0f) + Mathf.Pow(y, 2.0f) + Mathf.Pow(z, 2.0f));
	}
	
	public int GetBiggestFactor() {
		if (x > y) {
			if (x > z) {
				return 0;
			} else {
				return 2;
			}
		} else {
			if (y > z) {
				return 1;
			} else {
				return 2;
			}
		}
	}
	
	public override string ToString() {
    	return base.ToString() + ": (" + x + ", " + y + ", " + z + ")";
	}
	
	public static IntTriple operator -(IntTriple t1, IntTriple t2) {
		return new IntTriple(t1.x-t2.x, t1.y-t2.y, t1.z-t2.z);
	}

	public static IntTriple operator +(IntTriple t1, IntTriple t2) {
		return new IntTriple(t1.x+t2.x, t1.y+t2.y, t1.z+t2.z);
	}

	public static bool operator ==(IntTriple t1, IntTriple t2) {
		return (t1.x == t2.x && t1.y == t2.y && t1.z == t2.z) ? true : false;
	}

	public static bool operator !=(IntTriple t1, IntTriple t2) {
		return (t1.x != t2.x || t1.y != t2.y || t1.z != t2.z) ? true : false;
	}
}
