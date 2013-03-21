using System;
using UnityEngine;
using System.Collections.Generic;

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

	public IntTriple(IntDouble d, int i) {
		x=d.x;
		y=d.y;
		z=i;
	}
	
	public Vector3 GetVector3() {
		//return new UnityEngine.Vector3(x+0.5f, y+0.5f, z+0.5f);
		return new UnityEngine.Vector3(x, y, z);
	}
	
	public float Magnitude() {
		return Mathf.Sqrt(Mathf.Pow(x, 2.0f) + Mathf.Pow(y, 2.0f) + Mathf.Pow(z, 2.0f));
	}
	
	public int GetFactor(int i) {
		if (i==0) return x;
		if (i==1) return y;
		return z;
	}
	
	public void SetFactor(int i, int v) {
		if (i==0) {
			x = v;
		} else if (i==1) {
			y = v;
		} else {
			z = v;
		}
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
	
	public static IntTriple operator *(IntTriple t1, IntTriple t2) {
		return new IntTriple(t1.x*t2.x, t1.y*t2.y, t1.z*t2.z);
	}

	public static IntTriple operator *(IntTriple t1, int i) {
		return new IntTriple(t1.x*i, t1.y*i, t1.z*i);
	}

	public static IntTriple operator *(int i, IntTriple t1) {
		return new IntTriple(t1.x*i, t1.y*i, t1.z*i);
	}
	
	public static IntTriple operator /(IntTriple t1, int i) {
		return new IntTriple(Mathf.FloorToInt(t1.x/i), Mathf.FloorToInt(t1.y/i), Mathf.FloorToInt(t1.z/i));
	}

	public static IntTriple operator %(IntTriple t1, int i) {
		return new IntTriple(t1.x%i, t1.y%i, t1.z%i);
	}

	public static IntTriple operator -(IntTriple t1, IntTriple t2) {
		return new IntTriple(t1.x-t2.x, t1.y-t2.y, t1.z-t2.z);
	}

	public static IntTriple operator +(IntTriple t1, IntTriple t2) {
		return new IntTriple(t1.x+t2.x, t1.y+t2.y, t1.z+t2.z);
	}

	public static IntTriple operator +(Vector3 t1, IntTriple t2) {
		return new IntTriple((int)t1.x+t2.x, (int)t1.y+t2.y, (int)t1.z+t2.z);
	}
	
	public static bool operator ==(IntTriple t1, IntTriple t2) {
		return (t1.x == t2.x && t1.y == t2.y && t1.z == t2.z) ? true : false;
	}

	public static bool operator !=(IntTriple t1, IntTriple t2) {
		return (t1.x != t2.x || t1.y != t2.y || t1.z != t2.z) ? true : false;
	}

	public static IntTriple ExtractRandomFromPool(ref List<IntTriple> pool) {
		int random = UnityEngine.Random.Range(0, pool.Count);
		IntTriple result = pool[random];
		pool.RemoveAt(random);
		return result;
	}
		
	public override bool Equals (object obj) {
		return base.Equals(obj);
	}
	
	public override int GetHashCode () {
		return base.GetHashCode();
	}

}
