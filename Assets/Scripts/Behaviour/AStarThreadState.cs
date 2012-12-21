using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	
public class AStarThreadState {
	public bool isProcessing = false;
	public bool isFinished = true;
	public LinkedList<AStarNode> path;
	private float startTime;

	public AStarThreadState() {
	}
	
	public void Start() {
		startTime = Time.realtimeSinceStartup;
		isProcessing = true;
		isFinished = false;
		path = new LinkedList<AStarNode>();
	}
	
	public void Finish() {
		isFinished = true;
	}
	
	public void Complete() {
		isProcessing = false;
		Debug.Log("AStar completed in " + (Time.realtimeSinceStartup-startTime));
	}
	
	public bool isFinishedNow() {
		return (isFinished && isProcessing);
	}
}

