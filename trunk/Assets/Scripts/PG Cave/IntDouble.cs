using System;
using UnityEngine;

public struct IntDouble {
	public int x,y;
	
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
}

