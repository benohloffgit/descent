using System;
using UnityEngine;

public struct GridPosition {
	public IntTriple roomPosition;
	public IntTriple cellPosition;
		
	public GridPosition(IntTriple cP, IntTriple rP) {
		cellPosition = cP;
		roomPosition = rP;
	}

	public static bool operator ==(GridPosition t1, GridPosition t2) {
		return (t1.cellPosition == t2.cellPosition && t1.roomPosition == t2.roomPosition) ? true : false;
	}

	public static bool operator !=(GridPosition t1, GridPosition t2) {
		return (t1.cellPosition != t2.cellPosition || t1.roomPosition != t2.roomPosition) ? true : false;
	}

	public Vector3 GetVector3() {
		return roomPosition.GetVector3() * Game.DIMENSION_ROOM + cellPosition.GetVector3();
	}

	public override string ToString() {
    	return base.ToString() + ": Room (" + roomPosition.x + ", " + roomPosition.y + ", " + roomPosition.z + "), Cell (" + cellPosition.x + ", " + cellPosition.y + ", " + cellPosition.z + ")";
	}
	
	public override bool Equals (object obj) {
		return base.Equals(obj);
	}
	
	public override int GetHashCode () {
		return base.GetHashCode();
	}
	
}
