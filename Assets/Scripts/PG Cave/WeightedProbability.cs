using System;
using UnityEngine;

public struct WeightedProbability {
	public IntTriple baseValue;
	public int minProbability;
	public int maxProbability;
	
	public WeightedProbability(IntTriple bV, int minP, int maxP) {
		baseValue = bV;
		minProbability = minP;
		maxProbability = maxP;
	}
}