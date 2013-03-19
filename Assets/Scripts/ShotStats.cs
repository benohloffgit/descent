using UnityEngine;
	
public struct ShotStats {
	public int val;
	public float time;
	
	public static int MISS = 0;
	public static int HIT = 1;

	public ShotStats(int val_, float time_) {
		val = val_;
		time = time_;
	}
}

