using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EqualityComparerAStarNode : IEqualityComparer<AStarNode> {
	public EqualityComparerAStarNode() {
	}
	
	public bool Equals(AStarNode a, AStarNode b) {
		return true;
	}
	
	public int GetHashCode(AStarNode a) {
		return a.GetHashCode();
	}
}

